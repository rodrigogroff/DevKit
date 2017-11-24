
namespace DataModel
{
    public class Context
    {
        public const string TRUE = "1",
                            FALSE = "0",
                            NOT_SET = "0",
                            EMPTY = "",
                            OPEN = "0",
                            CLOSED = "1",
                            NONE = "0";
    }

    public class TipoCartao
    {
        public const string empresarial = "0",
                            presente = "1",
                            educacional = "2";
    }

    public class StatusExpedicao
    {
        public const string NaoExpedido = "0",
                            EmExpedicao = "1",
                            Expedido = "2";

        public string Convert(int? vlr)
        {
            if (vlr == null)
                return "";

            var sit = "";

            switch (vlr.ToString())
            {
                case NaoExpedido: sit = "Não expedido"; break;
                case EmExpedicao: sit = "Em expedição"; break;
                case Expedido: sit = "Expedido"; break;
            }

            return sit;
        }
    }

    public class CartaoStatus
    {
        public const string Habilitado = "0",
                            Bloqueado = "1",
                            EmDesativacao = "2";
    }

    public class TipoOperacao
    {
        public const string CadNovoOper = "1";
        public const string AlterOper = "2";
        public const string CadEmpresa = "3";
        public const string AlterEmpresa = "4";
        public const string CadLoja = "5";
        public const string AlterLoja = "6";
        public const string CadTerminal = "7";
        public const string AlterTerminal = "8";
        public const string CadCartao = "9";
        public const string TrocaSenha = "10";
        public const string RemoverTerminal = "11";
        public const string Login = "12";
        public const string Logoff = "13";
        public const string AlterCartao = "14";
        public const string AlterSenha = "15";
        public const string CadPayFoneCliente = "16";
        public const string CadPayFoneLojista = "17";
        public const string NovaAgenda = "18";
        public const string RemAgenda = "19";
        public const string AlterEduDiario = "20";
        public const string CadDespesa = "21";
        public const string RemDespesa = "22";
        public const string RemProdGift = "23";
        public const string RemQuiosqueGift = "24";
        public const string AlterChamConvey = "25";
        public const string AltDadosPropCart = "26";
        public const string CancChequeGift = "27";
        public const string CancCadEmpresa = "28";
        public const string CompChequeGift = "29";
        public const string ConfCartConv = "30";
        public const string ProcArqBanConvey = "31";
        public const string RecargaGift = "32";
        public const string RepasseLojaGift = "33";
        public const string ReqSegViaCart = "34";
        public const string VincVendQuiosque = "35";
        public const string CadChamadoConvey = "36";
        public const string CadDepenCart = "37";
        public const string CadProdExtraGift = "38";
        public const string CadNovoQuiosque = "39";
        public const string GeraArqGrafica = "40";
        public const string BloqueioCartao = "41";
        public const string DesbloqueioCartao = "42";
        public const string BloqueioLoja = "43";
        public const string DesbloqueioLoja = "44";
        public const string CancelLoja = "45";
        public const string RemoveConvenio = "46";
        public const string ResolvePend = "47";
        public const string CancelamentoCartao = "48";
        public const string VendaGift = "49";
        public const string CotaExtraMensal = "50";
    }
}
