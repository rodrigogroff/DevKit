    
namespace Master.Data.Const
{
    public class StatusExpedicao
    {
        public const int NaoExpedido = 0,
                            EmExpedicao = 1,
                            Expedido = 2;

        public string Convert(int vlr)
        {
            var sit = "";

            switch (vlr)
            {
                case NaoExpedido: sit = "Não Expedido"; break;
                case EmExpedicao: sit = "Em Expedição"; break;
                case Expedido: sit = "Expedido"; break;
            }

            return sit;
        }
    }
}
