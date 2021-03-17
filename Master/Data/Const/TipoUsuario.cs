
namespace Master.Data.Const
{
    public class TipoUsuario
    {
        public const int Lojista = 1,
                            Associado = 2,
                            LojistaFinanceiro = 3,
                            Emissor = 4,
                            Admin = 5;

        public string Convert(int vlr)
        {
            var sit = "";

            switch (vlr)
            {
                case Lojista: sit = "Lojista"; break;
                case Associado: sit = "Associado"; break;
                case LojistaFinanceiro: sit = "LojistaFinanceiro"; break;
                case Emissor: sit = "Emissor"; break;
                case Admin: sit = "Admin"; break;
            }

            return sit;
        }
    }
}
