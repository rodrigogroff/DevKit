namespace Master.Infra.Entity.Database
{
    public class CartaoStatus
    {
        public const int Habilitado = 0,
                            Bloqueado = 1,
                            EmDesativacao = 2;

        public string Convert(int vlr)
        {
            var sit = "";

            switch (vlr)
            {
                case Habilitado: sit = "Habilitado"; break;
                case Bloqueado: sit = "Bloqueado"; break;
                case EmDesativacao: sit = "Em desativacao"; break;
            }

            return sit;
        }
    }
}
