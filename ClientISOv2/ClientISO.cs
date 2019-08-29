using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientISOv2
{
    public class ClientISO
    {
        byte[] bytes = new byte[999999];
        Socket senderSock;

        public const string TerminalTeste = "",
                            codLoja = "",
                            trilha2 = ""; //????????????

        public ISO8583 Get0200(string valor, string nsuOrigem)
        {
            return new ISO8583
            {
                codProcessamento = "002000",
                nsuOrigem = nsuOrigem,
                terminal = ClientISO.TerminalTeste,
                codLoja = ClientISO.codLoja,
                trilha2 = ClientISO.trilha2,
                senha = "", //????????????
                valor = valor.PadLeft(12, '0')
            };
        }

        public ISO8583 Get0400(string nsu)
        {
            return new ISO8583
            {
                codProcessamento = "002000",
                codLoja = ClientISO.codLoja,
                terminal = ClientISO.TerminalTeste,
                nsuOrigem = "",

                bit125 = nsu
            };
        }

        public ISO8583 Get0420(string nsu)
        {
            return new ISO8583
            {
                codProcessamento = "002000",
                nsuOrigem = nsu,
                codLoja = ClientISO.codLoja,
                terminal = ClientISO.TerminalTeste,
                valor = ""
            };
        }

        public ISO8583 Get0202(string nsu)
        {
            return new ISO8583
            {
                codProcessamento = "002000",
                codLoja = ClientISO.codLoja,
                terminal = ClientISO.TerminalTeste,
                bit127 = nsu
            };
        }

        public void Start()
        {
            try
            {                
                SocketPermission permission = new SocketPermission( NetworkAccess.Connect, TransportType.Tcp, "", SocketPermission.AllPorts );

                permission.Demand();

                var ipHost = Dns.GetHostEntry("");
                var ipAddr = ipHost.AddressList[1];
                var ipEndPoint = new IPEndPoint(ipAddr, 2700);
                var nsuRec = "";

                ISO8583 isoReg;

                senderSock = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = false   // Using the Nagle algorithm 
                };

                senderSock.Connect(ipEndPoint);

                Console.WriteLine ( "Socket connected to " + senderSock.RemoteEndPoint.ToString() );

                while (true) { if (DateTime.Now.Second == 1) break; Thread.Sleep(10); }

                for (int i = 0; i < 1; i++)
                {
                    char sep = '\0';

                    SendSincrono ( Get0200("100", "1").registro + sep +
                                   Get0200("101", "1").registro + sep +
                                   Get0200("102", "1").registro );

                    Thread.Sleep(100);

                    isoReg = new ISO8583(ReceiveDataFromServer());

                    nsuRec = isoReg.bit127;

                    Console.WriteLine("Rec from server: " + isoReg.registro);                    

                    SendSincrono(Get0202(nsuRec).registro);

                    Thread.Sleep(100);

                    isoReg = new ISO8583(ReceiveDataFromServer());

                    nsuRec = isoReg.bit127;

                    Console.WriteLine("Rec from server: " + isoReg.registro);

                    SendSincrono(Get0202(nsuRec).registro);

                    Thread.Sleep(100);

                    isoReg = new ISO8583(ReceiveDataFromServer());

                    nsuRec = isoReg.bit127;

                    Console.WriteLine("Rec from server: " + isoReg.registro);

                    SendSincrono(Get0202(nsuRec).registro);

                    Thread.Sleep(100);
                }

                Disconnect();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }

        #region - socket -

        public void SendToServer(string tbMsgToSend)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(tbMsgToSend);
                senderSock.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), senderSock);
            }
            catch (SocketException exc)
            {
                
            }
            catch (Exception exc)
            {
                
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
                
            }
            catch (Exception exc)
            {
                
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
            catch (Exception exc)
            {
                
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

        #endregion
    }
}
