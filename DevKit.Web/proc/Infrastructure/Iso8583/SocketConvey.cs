using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevKit.Web.Controllers
{
    public class SocketConvey
    {
        public Socket connectSocket(string server, int port)
        {
            Socket socketRet = null;

            try
            {
                foreach (IPAddress address in Dns.Resolve(server).AddressList)
                {
                    IPEndPoint ipEndPoint = new IPEndPoint(address, port);

                    Socket socketNew = new Socket ( ipEndPoint.AddressFamily, 
                                                  SocketType.Stream, 
                                                  ProtocolType.Tcp );

                    socketNew.Connect(ipEndPoint);

                    if (socketNew.Connected)
                    {
                        socketRet = socketNew;
                        break;
                    }
                }                
            }
            catch (SocketException ex)
            {
                //?
            }
            catch (Exception ex)
            {
                //?
            }

            return socketRet;
        }

        public bool socketEnvia(Socket s, string registro)
        {
            try
            {                
                byte[] bytes = Encoding.ASCII.GetBytes(registro);

                s.Send(bytes, bytes.Length, SocketFlags.None);

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public string socketRecebe(Socket s)
        {
            try
            {
                //Encoding ascii = Encoding.ASCII;
                byte[] numArray = new byte[256];
                int bytes = s.Receive(numArray, numArray.Length, SocketFlags.None);
                string registro = this.ConvertBytetoString(numArray, bytes);

                return registro;
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }

        public string ConvertBytetoString(byte[] recebido, int bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int index = 0; index < recebido.Length; ++index)
            {
                char ch = Convert.ToChar(recebido[index]);
                stringBuilder.Append(string.Format("{0:s}", ch.ToString()));
            }

            return stringBuilder.ToString().Substring(0, bytes);
        }
    }
}
