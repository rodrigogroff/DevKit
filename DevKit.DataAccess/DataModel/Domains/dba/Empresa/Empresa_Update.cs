using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Empresa
    {
		public bool Update(DevKitDB db, ref string resp)
		{
			switch (updateCommand)
			{
                case "newSecao":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaSecao>(anexedEntity.ToString());

                        ent.fkEmpresa = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.EmpresaSecao where ne.fkEmpresa == id && ne.nuEmpresa == ent.nuEmpresa select ne).Any())
                            {
                                resp = "Número já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeSecao":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaSecao>(anexedEntity.ToString());

                        if (db.Associado.Any (y=> y.fkSecao == ent.id))
                        {
                            resp = "Seção possui associados!";
                            return false;
                        }
                        else
                            db.Delete(ent);

                        return true;
                    }

                case "newPhone":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaTelefone>(anexedEntity.ToString());

                        ent.fkEmpresa = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.EmpresaTelefone where ne.fkEmpresa == id && ne.stTelefone == ent.stTelefone select ne).Any())
                            {
                                resp = "Telefone já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            var oldPhone = db.EmpresaTelefone.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removePhone":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaTelefone>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "newEmail":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaEmail>(anexedEntity.ToString());

                        ent.fkEmpresa = id;

                        if (ent.id == 0)
                        {
                            if ((from ne in db.EmpresaEmail where ne.fkEmpresa == id && ne.stEmail == ent.stEmail select ne).Any())
                            {
                                resp = "Email já utilizado!";
                                return false;
                            }

                            db.Insert(ent);
                        }
                        else
                        {
                            var oldEmail = db.EmpresaEmail.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEmail":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaEmail>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                case "newEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaEndereco>(anexedEntity.ToString());

                        ent.fkEmpresa = id;
                        
                        if (ent.id == 0)
                        {
                            db.Insert(ent);
                        }
                        else
                        {
                            var oldEnd = db.EmpresaEndereco.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<EmpresaEndereco>(anexedEntity.ToString());

                        db.Delete(ent);

                        return true;
                    }

                default: break;
			}
            
            db.Update(this);

            return true;
		}
	}
}
