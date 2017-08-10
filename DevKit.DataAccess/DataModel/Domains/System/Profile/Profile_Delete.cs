using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Profile
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if (stName.ToUpper() == "ADMINISTRATOR")
			{
				resp = "Perfil de Administrador não pode ser removido";
				return false;
			}
			
			if ((from e in db.User where e.fkProfile == this.id select e).Any())
			{
				resp = "Perfil ainda associado com outros usuários";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.currentUser;

			db.Delete(this);

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemProfileRemove,
				nuType = EnumAuditType.Profile,
				fkTarget = this.id
			}.
			Create(db, "", "");
		}
	}
}
