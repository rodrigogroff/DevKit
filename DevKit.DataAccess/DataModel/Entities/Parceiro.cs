using LinqToDB;

namespace DataModel
{
    public partial class Parceiro
    {
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
