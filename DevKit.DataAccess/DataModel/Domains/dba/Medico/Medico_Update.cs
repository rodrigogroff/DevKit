using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Medico
    {
		public bool Update(DevKitDB db, ref string resp)
		{            
			switch (updateCommand)
			{
                case "changePassword":
                    {
                        var ent = JsonConvert.DeserializeObject<UserPasswordChange>(anexedEntity.ToString());

                        var medico = db.currentMedico;

                        if (medico.stSenha == null)
                            medico.stSenha = medico.nuCodigo.ToString();

                        if (medico.stSenha == ent.stCurrentPassword )
                        {
                            medico.stSenha = ent.stNewPassword;

                            db.Update(medico);
                        }
                        else
                        {
                            resp = "Senha não confere.";
                            return false;
                        }                        

                        break;
                    }
			}

            db.Update(this);

            return true;
		}
	}
}
