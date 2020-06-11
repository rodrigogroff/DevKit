using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using RestSharp;

namespace ServerIsoV2
{
    public partial class ServerISO
    {
        public bool Simulation = false;

        public Encoding myEnconding = Encoding.ASCII;

        public string hostAPI = "http://localhost:80";
        public string hostMachine = "10.11.0.41";

        public const int portHostSITEF = 3700,
                         maxQueue = 999,
                         maxPckSize = 99999;

        public List<IsoCommand> GlobalCommands = new List<IsoCommand>();

        public byte[] GetBuffer() { return new byte[maxPckSize]; }

        public string GenerateId() { return Guid.NewGuid().ToString(); }

        public void Start()
        {
            #region - setup listener -

            Console.WriteLine("x 1.2");

            IPHostEntry ipHost = Dns.GetHostEntry("");

            int index = 0;

            for (int i = 0; i < ipHost.AddressList.Length; i++)
            {
                Console.WriteLine(i + " " + ipHost.AddressList[i].ToString());

                if (ipHost.AddressList[i].ToString().Contains("."))
                {
                    index = i;
                    break;
                }
            }                   

            IPAddress ipAddr = ipHost.AddressList[0];

            ipAddr = IPAddress.Parse(hostMachine);

            Console.WriteLine("ip addr " + ipAddr.ToString());

            var ipEndPoint = new IPEndPoint(ipAddr, portHostSITEF);

            Console.WriteLine(ipEndPoint.ToString());

            var sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            sListener.Bind(ipEndPoint);
            sListener.Listen(maxQueue);

            AsyncCallback aCallback = new AsyncCallback(BeginClientAccept);
            sListener.BeginAccept(aCallback, sListener);

            #endregion

            Console.WriteLine("Server on " + ipEndPoint.Address + " port: " + ipEndPoint.Port);

            int last = -1;

            while (true)
            {
                Thread.Sleep(1000);

                var lstCmdsToExecute = GlobalCommands.Where(y => y.Running == false).ToList();
                var count = lstCmdsToExecute.Count();

                if (last != count)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] >> Queue: " + count);
                    last = count;
                }

                if (count > 0)
                {
                    Console.WriteLine("Queue: " + count);

                    foreach (var cmd in lstCmdsToExecute)
                    {
                        Console.WriteLine("Start! "+ cmd.Text );
                        new Thread(() => ProcessaLote(cmd)).Start();
                    }                        
                }

                GlobalCommands.RemoveAll(y => y.Ended == true);
            }
        }

        public void BatchService_Fechamento()
        {
            #region - code - 

            Console.WriteLine("BatchService_Fechamento...");

            while (true)
            {
                var serviceClient = new RestClient(hostAPI);
                var serviceRequest = new RestRequest("api/FechamentoServerISO", Method.GET);

                serviceClient.Execute(serviceRequest);

                Thread.Sleep(1000 * 60);
            }

            #endregion
        }

        public void BatchService_ConfirmacaoAuto()
        {
            #region - code - 

            Console.WriteLine("BatchService_ConfirmacaoAuto...");

            while (true)
            {
                var serviceClient = new RestClient(hostAPI);
                var serviceRequest = new RestRequest("api/ConfirmacaoAutoServerISO", Method.GET);

                serviceClient.Execute(serviceRequest);

                Thread.Sleep(5000);
            }

            #endregion
        }

        #region - socket methods - 

        public void BeginClientAccept(IAsyncResult ar)
        {
            #region - code - 

            try
            {
                byte[] buffer = GetBuffer();
                
                var listener = ar.AsyncState as Socket;
                var handler = listener.EndAccept(ar);
               
                handler.NoDelay = false;

                object[] obj = new object[3];
                obj[0] = buffer;
                obj[1] = handler;
                obj[2] = GenerateId();

                Console.WriteLine("> New client connected!");

                handler.BeginReceive( buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMsg), obj );
                listener.BeginAccept(new AsyncCallback(BeginClientAccept), listener);
            }
            catch (Exception exc)
            {
                Console.WriteLine("*** AcceptCallback + " + exc.ToString());
            }

            #endregion
        }

        public void ReceiveMsg(IAsyncResult ar)
        {
            #region - code - 

            var obj = ar.AsyncState as object[];
            byte[] buffer = obj[0] as byte[];
            var handler = obj[1] as Socket;
            var myGuid = obj[2] as string;

            string content = string.Empty;

            try
            {
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += myEnconding.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("[Received] " + content);

                    GlobalCommands.Add(new IsoCommand
                    {
                        Text = content,
                        handler = handler,
                        Id = myGuid
                    });
                }
            }
            catch (SocketException exc)
            {
                var cmd = GlobalCommands.Where(y => y.Id == myGuid).FirstOrDefault();

                Console.WriteLine("[Client Exit]");

                if (cmd != null)
                    cmd.Ended = true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            #endregion
        }

        public void SendSincrono(string theMessageToSend, IsoCommand cmd)
        {
            #region - code - 

            Console.WriteLine("Send >> " + theMessageToSend);
            try
            {
                string str = string.Format("{0:X2}", (object)theMessageToSend.Length).PadLeft(4, '0');
                byte[] numArray = new byte[2];
                for (int index = 0; index < str.Length / 2; ++index)
                    numArray[index] = (byte)Convert.ToInt32(str.Substring(index * 2, 2), 16);
                byte[] bytes = Encoding.ASCII.GetBytes("00" + theMessageToSend);
                bytes[0] = numArray[1];
                bytes[1] = numArray[0];

                // Sends data to a connected Socket. 
                int bytesSend = cmd.handler.Send(bytes);

                cmd.Log("Envio completo!");
            }
            catch (Exception exc)
            {
                cmd.LogFalha(exc.ToString());
            }

            #endregion
        }

        public void Send(string tbMsgToSend, IsoCommand cmd)
        {
            #region - code -

            Console.WriteLine("[Sending] " + tbMsgToSend);

            try
            {
                var obj = new object[3];
                byte[] byteData = myEnconding.GetBytes(tbMsgToSend);
                obj[0] = byteData;
                obj[1] = cmd.handler;
                obj[2] = cmd.Id;

                cmd.handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), obj);                
            }
            catch (SocketException exc)
            {
                Console.WriteLine(exc);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            #endregion
        }

        public void SendCallback(IAsyncResult ar)
        {
            #region - code - 

            try
            {
                var obj = ar.AsyncState as object[];
                var handler = obj[1] as Socket;
                var myGuid = obj[2] as string;

                int bytesSend = handler.EndSend(ar);

                var cmd = GlobalCommands.Where(y => y.Id == myGuid).FirstOrDefault();
            }
            catch (SocketException exc)
            {
                Console.WriteLine(exc);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            #endregion
        }

        public void WaitMessage(IsoCommand cmd)
        {
            #region - code - 

            object[] obj = new object[3];
            byte[] buffernew = GetBuffer();
            obj[0] = buffernew;
            obj[1] = cmd.handler;
            obj[2] = cmd.Id;

            cmd.handler.BeginReceive(buffernew, 0, buffernew.Length, SocketFlags.None, new AsyncCallback(ReceiveMsg), obj);

            #endregion
        }

        #endregion
    }
}
