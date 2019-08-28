using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientISOv2
{
    public class ClientISO
    {
        byte[] bytes = new byte[1024];
        Socket senderSock;

        public void Start()
        {
            try
            {
                // Create one SocketPermission for socket access restrictions 
                SocketPermission permission = new SocketPermission(
                    NetworkAccess.Connect,    // Connection permission 
                    TransportType.Tcp,        // Defines transport types 
                    "",                       // Gets the IP addresses 
                    SocketPermission.AllPorts // All ports 
                    );

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance            
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost 
                IPAddress ipAddr = ipHost.AddressList[1];

                // Creates a network endpoint 
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 2700);

                // Create one Socket object to setup Tcp connection 
                senderSock = new Socket(
                    ipAddr.AddressFamily,// Specifies the addressing scheme 
                    SocketType.Stream,   // The type of socket  
                    ProtocolType.Tcp     // Specifies the protocols  
                    );

                senderSock.NoDelay = false;   // Using the Nagle algorithm 

                // Establishes a connection to a remote host 
                senderSock.Connect(ipEndPoint);

                Console.WriteLine ( "Socket connected to " + senderSock.RemoteEndPoint.ToString() );

                while (true) { if (DateTime.Now.Second == 1) break; Thread.Sleep(10); }

                for (int i = 0; i < 1; i++)
                {
                    var resp = "";

                    var x = Guid.NewGuid().ToString();
                    var x2 = Guid.NewGuid().ToString();
                    var x3 = Guid.NewGuid().ToString();
                    var x4 = Guid.NewGuid().ToString();

                    char sep = '\0';

                    SendSincrono(   "0200 " + x + sep +
                                    "0200 " + x2 + sep +
                                    "0400 " + x3 + sep +
                                    "0420 " + x3 + sep +
                                    "0200 " + x4 );

                    Thread.Sleep(100);

                    resp = ReceiveDataFromServer();
                    Console.WriteLine("Rec from server: " + resp);

                    SendSincrono("0202 xxxx");

                    Thread.Sleep(100);

                    resp = ReceiveDataFromServer();
                    Console.WriteLine("Rec from server: " + resp);

                    SendSincrono("0202 xxx");

                    Thread.Sleep(100);

                    resp = ReceiveDataFromServer();
                    Console.WriteLine("Rec from server: " + resp);

                    Thread.Sleep(100);

                    resp = ReceiveDataFromServer();
                    Console.WriteLine("Rec from server: " + resp);

                    Thread.Sleep(100);

                    resp = ReceiveDataFromServer();
                    Console.WriteLine("Rec from server: " + resp);

                    SendSincrono("0202 xxx ");

                    Thread.Sleep(100);
                }

                Disconnect();
            }
            catch (Exception exc) {
                Console.WriteLine(exc.ToString());
            }
        }

        public void SendToServer(string tbMsgToSend)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(tbMsgToSend);
                senderSock.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), senderSock);
            }
            catch (SocketException exc)
            {
                //Console.WriteLine("Não conseguiu enviar!");
            }
            catch (Exception exc)
            {
                //Console.WriteLine("*** Send + " + exc.ToString());
            }
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSend = handler.EndSend(ar);
            }
            catch (SocketException exc)
            {
                //Console.WriteLine("Não conseguiu enviar!");
            }
            catch (Exception exc)
            {
                //Console.WriteLine("*** SendCallback + " + exc.ToString());
            }
        }

        public void SendSincrono(string theMessageToSend)
        {
            Console.WriteLine("Send >> " + theMessageToSend);
            try
            {
                byte[] msg = Encoding.ASCII.GetBytes(theMessageToSend);
                
                // Sends data to a connected Socket. 
                int bytesSend = senderSock.Send(msg);
            }
            catch (Exception exc) {
                Console.WriteLine(exc.ToString());
            }
        }

        private string ReceiveDataFromServer()
        {
            try
            {
                // Receives data from a bound Socket. 
                int bytesRec = senderSock.Receive(bytes);

                // Converts byte array to string 
                String theMessageToReceive = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                // Continues to read the data till data isn't available 
                while (senderSock.Available > 0)
                {
                    bytesRec = senderSock.Receive(bytes);
                    theMessageToReceive += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }

                Console.WriteLine("Rec >> " + theMessageToReceive);

                return theMessageToReceive;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());

                return null;                
            }
        }

        private void Disconnect()
        {
            try
            {
                senderSock.Shutdown(SocketShutdown.Both);
                senderSock.Close();

                Console.WriteLine("Saiu!");
            }
            catch (Exception exc) {
                Console.WriteLine(exc.ToString());
            }
        }
    }
}
