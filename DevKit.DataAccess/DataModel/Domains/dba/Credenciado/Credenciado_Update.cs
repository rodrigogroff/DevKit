using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Credenciado
    {
		public bool Update(DevKitDB db, ref string resp)
		{            
			switch (updateCommand)
			{
                case "newEmpresa":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEmpresa>(anexedEntity.ToString());

                        if (!db.CredenciadoEmpresa.Any(y=>y.fkCredenciado == this.id && y.fkEmpresa == ent.fkEmpresa))
                        {
                            ent.fkCredenciado = this.id;
                            db.Insert(ent);
                        }
                        
                        return true;
                    }

                case "newProcedimentoViaEmissor":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEmpresaTuss>(anexedEntity.ToString());

                        ent.fkEmpresa = db.currentUser.fkEmpresa;
                        ent.fkCredenciado = this.id;

                        db.Insert(ent);

                        return true;
                    }

                case "removeProcedimentoViaEmissor":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEmpresaTuss>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "changePassword":
                    {
                        var ent = JsonConvert.DeserializeObject<UserPasswordChange>(anexedEntity.ToString());

                        var cred = db.currentCredenciado;

                        if (cred.stSenha == null)
                            cred.stSenha = cred.nuCodigo.ToString();

                        if (cred.stSenha == ent.stCurrentPassword )
                        {
                            cred.stSenha = ent.stNewPassword;

                            db.Update(cred);

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
                        var ent = JsonConvert.DeserializeObject<CredenciadoTelefone>(anexedEntity.ToString());

                        ent.fkCredenciado = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.CredenciadoTelefone where ne.fkCredenciado == id && ne.stPhone == ent.stPhone select ne).Any())
                            {
                                resp = "Telefone já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            var oldPhone = db.CredenciadoTelefone.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removePhone":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoTelefone>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "newEmail":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEmail>(anexedEntity.ToString());

                        ent.fkCredenciado = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.CredenciadoEmail where ne.fkCredenciado == id && ne.stEmail == ent.stEmail select ne).Any())
                            {
                                resp = "Email já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            var oldEmail = db.CredenciadoEmail.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();
                            
                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEmail":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEmail>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "newEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEndereco>(anexedEntity.ToString());

                        ent.fkCredenciado = id;

                        if (ent.bPrincipal == null)
                            ent.bPrincipal = false;

                        if (!db.CredenciadoEndereco.Where(y => y.fkCredenciado == id).Any())
                            ent.bPrincipal = true;

                        if (ent.id == 0)
                        {
                            db.Insert(ent);
                        }
                        else
                        {
                            var oldEnd = db.CredenciadoEndereco.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<CredenciadoEndereco>(anexedEntity.ToString());

                        db.Delete(ent);

                        if (db.CredenciadoEndereco.Where(y => y.fkCredenciado == id).Count() == 1)
                        {
                            var pa = db.CredenciadoEndereco.Where(y => y.fkCredenciado == id).FirstOrDefault();

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
