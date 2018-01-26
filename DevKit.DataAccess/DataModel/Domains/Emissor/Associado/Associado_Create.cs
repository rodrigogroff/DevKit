using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Associado
    {
		bool CheckDuplicate(Associado item, DevKitDB db)
		{
			var query = from e in db.Associado select e;

			if (item.stName != null)
				query = from e in query where e.stName.ToUpper().Contains(item.stName) select e;
				
			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (stName == "")
			{
				resp = "Nome inválido";
				return false;
			}

            fkEmpresa = user.fkEmpresa;

            stName = stName.ToUpper().Trim();

            nuTitularidade = 1;
            nuViaCartao = 1;
            tgStatus = 0;
            tgExpedicao = 0;
            stSenha = "1111";
            dtStart = DateTime.Now;
            fkUserAdd = user.id;
            
            id = Convert.ToInt64(db.InsertWithIdentity(this));

            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.PersonCreate,
                nuType = EnumAuditType.Person,
                fkTarget = this.id
            }.
            Create(db, "", "");

            return true;
		}
	}
}
