using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ClientISOv2
{
    public class ClientISO
    {
        char sep = '\0';
        Socket senderSock;

        public const string TerminalTeste = "123456",   // ????????
                            codLoja = "5000",           // ????????
                            trilha2 = "000000" + "000002" + "000001" + "01";

        #region - obter pacotes - 

        public string DESCript(string dados, string chave = "12345678")
        {
            #region - code -

            dados = dados.PadLeft(8, '*');

            byte[] key = Encoding.ASCII.GetBytes(chave);
            byte[] data = Encoding.ASCII.GetBytes(dados);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = key;
            des.Mode = CipherMode.ECB;

            ICryptoTransform DESt = des.CreateEncryptor();
            DESt.TransformBlock(data, 0, 8, data, 0);

            string retorno = "";
            for (int n = 0; n < 8; n++)
            {
                retorno += String.Format("{0:X2}", data[n]);
            }

            return retorno;

            #endregion
        }
        
        public ISO8583 Get0200(string valor, string nsuOrigem)
        {
            #region - code - 

            return new ISO8583
            {
                codigo= "0200",
                codProcessamento = "002000",
                nsuOrigem = nsuOrigem,
                terminal = ClientISO.TerminalTeste,
                codLoja = ClientISO.codLoja,
                trilha2 = ClientISO.trilha2,
                senha = DESCript ("1234"),
                valor = valor.PadLeft(12, '0')
            };

            #endregion
        }

        public ISO8583 Get0202(string nsu)
        {
            #region - code - 

            return new ISO8583
            {
                codigo = "0202",
                codProcessamento = "002000",
                codLoja = ClientISO.codLoja,
                terminal = ClientISO.TerminalTeste,
                bit62 = ClientISO.trilha2,
                bit127 = nsu
            };

            #endregion
        }

        public ISO8583 Get0400(string nsu)
        {
            #region - code - 
            return new ISO8583
            {
                codigo = "0400",
                codProcessamento = "002000",
                codLoja = ClientISO.codLoja,
                terminal = ClientISO.TerminalTeste,
                nsuOrigem = "",
                bit125 = nsu
            };
            #endregion
        }

        public ISO8583 Get0420(string nsu)
        {
            #region - code - 

            return new ISO8583
            {
                codigo = "0420",
                codProcessamento = "002000",
                nsuOrigem = nsu,
                codLoja = ClientISO.codLoja,
                terminal = ClientISO.TerminalTeste,
                valor = "0100"
            };

            #endregion
        }
        
        #endregion

        public void Start()
        {
            try
            {
                #region - listener - 

                SocketPermission permission = new SocketPermission( NetworkAccess.Connect, TransportType.Tcp, "", SocketPermission.AllPorts );

                permission.Demand();

                var ipHost = Dns.GetHostEntry("177.85.160.41");

                for (int i = 0; i < ipHost.AddressList.Length; i++)
                    Console.WriteLine(i + " " + ipHost.AddressList[i].ToString());

                var ipAddr = ipHost.AddressList[0];

                //var ipHost = Dns.GetHostEntry("");
                //var ipAddr = ipHost.AddressList[1];
                var ipEndPoint = new IPEndPoint(ipAddr, 2700);
                var nsuRec = "";

                ISO8583 isoReg;

                senderSock = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = false   // Using the Nagle algorithm 
                };

                senderSock.Connect(ipEndPoint);

                #endregion

                Console.WriteLine ( "Socket connected to " + senderSock.RemoteEndPoint.ToString() );

                //  while (true) { if (DateTime.Now.Second == 1) break; Thread.Sleep(10); }

                for (int i = 0; i < 1; i++)
                {
                    ExecutaLote();
                    //ExecutaVendaPendente();
                    //ExecutaCancelamentoSimples("99");
                    //ExecutaDesfazimentoSimples("110");
                }

                Disconnect();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }
        
        #region - funções de teste - 

        public void ExecutaLote()
        {
            #region - code - 

            SendSincrono(Get0200("100", "1").registro + sep +
                                   Get0200("101", "1").registro + sep +
                                   Get0200("102", "1").registro);

            Thread.Sleep(100);

            var isoReg = new ISO8583(ReceiveDataFromServer());

            var nsuRec = isoReg.bit127;

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

            #endregion
        }

        public void ExecutaVendaPendente()
        {
            #region - code - 

            SendSincrono(Get0200("103", "1").registro);

            Thread.Sleep(100);

            var isoReg = new ISO8583(ReceiveDataFromServer());

            #endregion
        }

        public void ExecutaCancelamentoSimples(string nsu)
        {
            #region - code - 

            SendSincrono(Get0400(nsu.PadLeft(6,'0')).registro);
            Thread.Sleep(100);
            var isoReg = new ISO8583(ReceiveDataFromServer());
            Console.WriteLine("Rec from server: " + isoReg.registro);

            #endregion
        }

        public void ExecutaDesfazimentoSimples(string nsu)
        {
            #region - code - 

            SendSincrono(Get0420(nsu.PadLeft(6, '0')).registro);
            Thread.Sleep(100);
            var isoReg = new ISO8583(ReceiveDataFromServer());
            Console.WriteLine("Rec from server: " + isoReg.registro);

            #endregion
        }

        #endregion

        #region - socket handlers -

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
                byte[] bytes = new byte[999999];

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
