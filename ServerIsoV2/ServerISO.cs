using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerIsoV2
{
    public class Command
    {
        public string Text { get; set; }

        public Socket handler { get; set; }
    }
    
    public class ServerISO
    {
        public List<Command> Commands = new List<Command>();
                
        public void Start()
        {            
            var permission = new SocketPermission( NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts );             
            permission.Demand();
            
            IPHostEntry ipHost = Dns.GetHostEntry("");            
            IPAddress ipAddr = ipHost.AddressList[1];
            
            var ipEndPoint = new IPEndPoint(ipAddr, 4510);            
            var sListener = new Socket( ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            sListener.Bind(ipEndPoint);
            sListener.Listen(1000);
            
            AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
            sListener.BeginAccept(aCallback, sListener);

            Console.WriteLine( "Server is now listening on " + ipEndPoint.Address + " port: " + ipEndPoint.Port );

            while (true)
            {
                if (Commands.Count > 0 )
                {
                    Console.WriteLine("Queue: " + Commands.Count);

                    var cmd = Commands[0];
                    Commands.RemoveAt(0);

                    Thread thread = new Thread(() => ProcessCommand(cmd));
                    thread.Start();
                }

                Thread.Sleep(100);
            }                
        }

        public void ProcessCommand (Command cmd)
        {
            Console.WriteLine("CMD> " + cmd.Text);
            Send("210 " + cmd.Text, cmd.handler);
        }

        public byte[] GetBuffer()
        {
            return new byte[999999];
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                byte[] buffer = GetBuffer();
                
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);

                handler.NoDelay = false;

                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                handler.BeginReceive( buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), obj );
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }
            catch (Exception exc)
            {
                Console.WriteLine("*** AcceptCallback + " + exc.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            var obj = ar.AsyncState as object[];
            byte[] buffer = obj[0] as byte[];
            var handler = obj[1] as Socket;

            string content = string.Empty;

            try
            {
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("RECV: " + content);

                    Commands.Add(new Command
                    {
                        Text = content,
                        handler = handler
                    });                    
                }      
                else
                {
                    Listen(handler);
                }
            }
            catch (SocketException exc)
            {
                Console.WriteLine(exc);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        public void Listen(Socket handler)
        {
            Console.WriteLine("Listen....");

            object[] obj = new object[2];
            byte[] buffernew = GetBuffer();
            obj[0] = buffernew;
            obj[1] = handler;

            handler.BeginReceive(buffernew, 0, buffernew.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), obj);
        }

        public void Send(string tbMsgToSend, Socket handler)
        {
            try
            {
                var obj = new object[2];
                byte[] byteData = Encoding.ASCII.GetBytes(tbMsgToSend);
                obj[0] = byteData;
                obj[1] = handler;

                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), obj);                
            }
            catch (SocketException exc)
            {
                Console.WriteLine(exc);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                var obj = ar.AsyncState as object[];
                var handler = obj[1] as Socket;
                int bytesSend = handler.EndSend(ar);

                Listen(handler);
            }
            catch (SocketException exc)
            {
                Console.WriteLine(exc);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }        
    }
}
