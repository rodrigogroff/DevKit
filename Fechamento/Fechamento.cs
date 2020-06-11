using System;
using System.Threading;
using RestSharp;

namespace Fechamento
{
    public partial class Fechamento
    {
        public string hostAPI = "http://localhost:80";
        
        public void Start()
        {
            BatchService_Fechamento();
            BatchService_ConfirmacaoAuto();
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
    }
}
