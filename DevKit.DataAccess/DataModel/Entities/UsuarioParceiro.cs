using LinqToDB;

namespace DataModel
{
    public partial class UsuarioParceiro
    {
        public string _stParceiro { get; set; }
        public string _sbAtivo { get; set; }
        public string _dtCadastro { get; set; }
        public string _dtLastLogin { get; set; }

        public bool Create(AutorizadorCNDB db, ref string apiError)
        {
            try
            {
                db.Insert(this);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(AutorizadorCNDB db, ref string apiError)
        {
            try
            {
                db.Update(this);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
