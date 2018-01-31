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

                            return true;
                        }
                        else
                        {
                            resp = "Senha não confere.";
                            return false;
                        }                        
                    }

                case "newPhone":
                    {
                        var ent = JsonConvert.DeserializeObject<MedicoPhone>(anexedEntity.ToString());

                        ent.fkMedico = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.MedicoPhone where ne.fkMedico == id && ne.stPhone == ent.stPhone select ne).Any())
                            {
                                resp = "Telefone já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            var oldPhone = db.MedicoPhone.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removePhone":
                    {
                        var ent = JsonConvert.DeserializeObject<MedicoPhone>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "newEmail":
                    {
                        var ent = JsonConvert.DeserializeObject<MedicoEmail>(anexedEntity.ToString());

                        ent.fkMedico = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.MedicoEmail where ne.fkMedico == id && ne.stEmail == ent.stEmail select ne).Any())
                            {
                                resp = "Email já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            var oldEmail = db.MedicoEmail.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();
                            
                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEmail":
                    {
                        var ent = JsonConvert.DeserializeObject<MedicoEmail>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "newEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<MedicoAddress>(anexedEntity.ToString());

                        ent.fkMedico = id;

                        if (ent.bPrincipal == null)
                            ent.bPrincipal = false;

                        if (!db.MedicoAddress.Where(y => y.fkMedico == id).Any())
                            ent.bPrincipal = true;

                        if (ent.id == 0)
                        {
                            db.Insert(ent);
                        }
                        else
                        {
                            var oldEnd = db.MedicoAddress.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<MedicoAddress>(anexedEntity.ToString());

                        db.Delete(ent);

                        if (db.MedicoAddress.Where(y => y.fkMedico == id).Count() == 1)
                        {
                            var pa = db.MedicoAddress.Where(y => y.fkMedico == id).FirstOrDefault();

                            pa.bPrincipal = true;

                            db.Update(pa);
                        }

                        return true;
                    }
            }

            db.Update(this);

            return true;
		}
	}
}
