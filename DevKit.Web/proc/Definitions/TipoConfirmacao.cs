
namespace DevKit.Web.Controllers
{
    public class TipoConfirmacao
    {
        public const string Pendente = "0",
                            Confirmada = "1",
                            Negada = "2",
                            Erro = "3",
                            Registro = "4",
                            Cancelada = "5",
                            Desfeita = "6";
    }

    public class TipoCaptura
    {
        public const string SITEF = "1",
                            PORTAL = "2";
    }
}