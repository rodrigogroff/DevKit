using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Associado
    {
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.AssociadoTelefone where e.fkAssociado == id select e))
				db.Delete(item);

			foreach (var item in (from e in db.AssociadoEmail where e.fkAssociado == id select e))
				db.Delete(item);

			db.Delete(this);
		}
	}
}
