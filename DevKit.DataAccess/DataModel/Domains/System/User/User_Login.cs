using System.Linq;

namespace DataModel
{
	public partial class User
	{
		public User Login(DevKitDB db, string emp, string login, string password)
		{
            empresa = (from e in db.Empresa
                            where e.nuEmpresa.ToString() == emp
                            select e).
                            FirstOrDefault();

            if (empresa == null)
                return null;
            
			var user = (from e in db.User
                        where e.fkEmpresa == empresa.id
						where e.stLogin.ToUpper() == login.ToUpper()
						where e.stPassword.ToUpper() == password.ToUpper()
						where e.bActive == true
						select e).
					    FirstOrDefault();

			if (user != null)
			{
                user.empresa = empresa;

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.Login,
					nuType = EnumAuditType.User,
					fkTarget = this.id
				}.
				Create(db, "", "");

				return user;
			}
			
			return null;
		}
	}
}
