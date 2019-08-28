using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace ServerIsoV2
{
    public class IsoCommand
    {
        public bool Ended { get; set; }

        public bool Running { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public Socket handler { get; set; }

        public bool ChannelOpen { get; set; }
    }

    public class ServerISO
    {
        public Encoding myEnconding = Encoding.ASCII;
        public const int portHost = 4510;
        public char sepPacotes = '\0';
        
        public List<IsoCommand> GlobalCommands = new List<IsoCommand>();

        public byte[] GetBuffer() { return new byte[99999]; }
        public string GenerateId() { return Guid.NewGuid().ToString(); }

        public void Start()
        {
            #region - setup listener -

            var permission = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
            permission.Demand();

            IPHostEntry ipHost = Dns.GetHostEntry("");
            IPAddress ipAddr = ipHost.AddressList[1];

            var ipEndPoint = new IPEndPoint(ipAddr, portHost);
            var sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sListener.Bind(ipEndPoint);
            sListener.Listen(1000);

            AsyncCallback aCallback = new AsyncCallback(BeginClientAccept);
            sListener.BeginAccept(aCallback, sListener);

            Console.WriteLine("Server on " + ipEndPoint.Address + " port: " + ipEndPoint.Port);

            ProcessGlobalQueue();

            #endregion
        }

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

                cmd.ChannelOpen = false;

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

                if (cmd != null)
                    cmd.ChannelOpen = true;
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

        public void Listen(IsoCommand cmd)
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

        public void ProcessGlobalQueue()
        {
            while (true)
            {
                Thread.Sleep(1000);

                var lstCmdsToExecute = GlobalCommands.Where(y => y.Running == false).ToList();
                var count = lstCmdsToExecute.Count();

                if (DateTime.Now.Second % 5 == 0)
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] >> Queue: " + count);

                if (count > 0)
                {
                    Console.WriteLine("Queue: " + count);

                    foreach (var cmd in lstCmdsToExecute)
                        new Thread(() => ProcessaLote(cmd)).Start();
                }

                GlobalCommands.RemoveAll(y => y.Ended == true);                
            }
        }
        public void ProcessaLote(IsoCommand cmd)
        {
            cmd.Ended = false;
            cmd.Running = true;

            foreach (var iso in cmd.Text.Split(sepPacotes))
            {
                Console.WriteLine("[Processing] " + iso);

                if (iso.StartsWith("0200"))
                {
                    #region - processa venda -

                    #endregion

                    cmd.ChannelOpen = false;

                    Send("210 ", cmd);

                    while (!cmd.ChannelOpen)
                        Thread.Sleep(10);

                    Listen(cmd);
                }
                else if (iso.StartsWith("0202"))
                {
                    #region - processa confirmação - 

                    #endregion
                }
                else if (iso.StartsWith("0400"))
                {
                    #region - processa cancelamnto - 

                    cmd.ChannelOpen = false;

                    Send("0410 ", cmd);

                    while (!cmd.ChannelOpen)
                        Thread.Sleep(10);

                    #endregion
                }
                else if (iso.StartsWith("0420"))
                {
                    #region - processa desfazimento - 

                    cmd.ChannelOpen = false;

                    Send("0430 ", cmd);

                    while (!cmd.ChannelOpen)
                        Thread.Sleep(10);

                    #endregion
                }
            }
        }        
    }
}
