using System;
using System.Threading;
using RestSharp;

namespace Fechamento
{
    public partial class Fechamento
    {
        public string hostAPI = "http://localhost:8081";
        
        public void Start()
        {
            BatchService_Fechamento();
        }

        public void BatchService_Fechamento()
        {
            #region - code - 

            while (true)
            {
                {
                    Console.WriteLine("FechamentoServerISO..." + DateTime.Now);

                    var serviceClient = new RestClient(hostAPI);
                    var serviceRequest = new RestRequest("api/FechamentoServerISO", Method.GET);

                    serviceClient.Execute(serviceRequest);
                }
                
                {
                    Console.WriteLine("ConfirmacaoAutoServerISO..." + DateTime.Now);

                    var serviceClient = new RestClient(hostAPI);
                    var serviceRequest = new RestRequest("api/ConfirmacaoAutoServerISO", Method.GET);

                    serviceClient.Execute(serviceRequest);
                }                

                Thread.Sleep(1000 * 60);
            }

            #endregion
        }
    }
}
