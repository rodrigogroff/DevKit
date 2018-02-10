using DevKit.DataAccess;
using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Associado
    {
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			switch (updateCommand)
			{
                default: return false;

                case "entity":
					{
                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonUpdate,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, "", "");

                        break;
					}

                case "altSenha":
                    {
                        var dbAssoc = db.Associado.Where(y => y.id == this.id).FirstOrDefault();

                        dbAssoc.stSenha = anexedEntity.ToString();

                        db.Update(dbAssoc);

                        return true;
                    }

                case "novaVia":
                    {
                        var dbAssoc = db.Associado.Where(y => y.id == this.id).FirstOrDefault();

                        dbAssoc.nuViaCartao++;
                        dbAssoc.tgExpedicao = TipoExpedicaoCartao.Requerido;

                        db.Update(dbAssoc);

                        return true;
                    }

                case "bloqueio":
                    {
                        var dbAssoc = db.Associado.Where(y => y.id == this.id).FirstOrDefault();

                        dbAssoc.tgStatus = TipoSituacaoCartao.Bloqueado;

                        db.Update(dbAssoc);

                        return true;
                    }

                case "desbloqueio":
                    {
                        var dbAssoc = db.Associado.Where(y => y.id == this.id).FirstOrDefault();

                        dbAssoc.tgStatus = TipoSituacaoCartao.Habilitado;

                        db.Update(dbAssoc);

                        return true;
                    }

                case "newDep":
                    {
                        var ent = JsonConvert.DeserializeObject<AssociadoDependente>(anexedEntity.ToString());

                        ent.fkEmpresa = this.fkEmpresa;
                        ent.fkAssociado = this.id;
                        ent.stNome = ent.stNome.ToUpper().TrimEnd();

                        if (ent.id == 0)
                        {
                            if ((from ne in db.AssociadoDependente where ne.stNome == ent.stNome select ne).Any())
                            {
                                resp = "Dependente já utilizado!";
                                return false;
                            }

                            ent.fkAssociado = Convert.ToInt64 (
                                db.InsertWithIdentity(new Associado
                                {
                                    fkEmpresa = this.fkEmpresa,
                                    fkUserAdd = user.id,
                                    nuMatricula = this.nuMatricula,
                                    tgStatus =  TipoSituacaoCartao.Habilitado,
                                    tgExpedicao = TipoExpedicaoCartao.Requerido,
                                    stSenha = this.stSenha,
                                    stName = ent.stNome,
                                    stCPF = ent.stCPF,
                                    nuTitularidade = db.AssociadoDependente.Where (y=> y.fkAssociado == id).Count() + 2,
                                    nuViaCartao = 1,
                                    dtStart = DateTime.Now,
                                }) );

                            db.Insert(ent);
                        }
                        else
                        {
                            var cart = db.Associado.Where(y => y.id == ent.fkAssociado).FirstOrDefault();

                            if (cart.tgExpedicao == 0)
                            {
                                cart.stName = ent.stNome;
                                cart.stCPF = ent.stCPF;

                                db.Update(cart);
                            }                                

                            db.Update(ent);
                        }

                        return true;
                    }

                case "newPhone":
					{
						var ent = JsonConvert.DeserializeObject<AssociadoTelefone>(anexedEntity.ToString());

						ent.stPhone = GetMaskedValue(db, ent.stPhone);
						ent.fkAssociado = id;

						if (ent.id == 0)
						{
							if ((from ne in db.AssociadoTelefone where ne.fkAssociado == id && ne.stPhone == ent.stPhone select ne).Any())
							{
								resp = "Telefone já utilizado!";
								return false;
							}

							db.Insert(ent);

                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddPhone,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, ent.stPhone, "");

                        }
						else
                        {
                            var oldPhone = db.AssociadoTelefone.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();
                                                        
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonEditPhone,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, oldPhone.stPhone + " => " + ent.stPhone, "");

                            db.Update(ent);
                        }

                        return true;
                    }

				case "removePhone":
					{
						var ent = JsonConvert.DeserializeObject<AssociadoTelefone>(anexedEntity.ToString());

                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonRemovePhone,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, ent.stPhone, "");

                        db.Delete(ent);

                        return true;
                    }

				case "newEmail":
					{
						var ent = JsonConvert.DeserializeObject<AssociadoEmail>(anexedEntity.ToString());

                        ent.fkAssociado = id;

                        if (ent.id == 0)
						{
							if ((from ne in db.AssociadoEmail where ne.fkAssociado == id && ne.stEmail == ent.stEmail select ne).Any())
							{
								resp = "Email já utilizado!";
								return false;
							}

							db.Insert(ent);

                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddEmail,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, ent.stEmail, "");

                        }
                        else
                        {
                            var oldEmail = db.AssociadoEmail.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            if (oldEmail != null)
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddEmail,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, oldEmail.stEmail + " => " + ent.stEmail, "");

                            db.Update(ent);
                        }

                        return true;
                    }

				case "removeEmail":
					{
						var ent = JsonConvert.DeserializeObject<AssociadoEmail>(anexedEntity.ToString());

                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonRemoveEmail,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, ent.stEmail, "");

                        db.Delete(ent);

                        return true;
                    }

                case "newEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<AssociadoEndereco>(anexedEntity.ToString());

                        ent.fkAssociado = id;

                        if (ent.bPrincipal == null)
                            ent.bPrincipal = false;

                        if (!db.AssociadoEndereco.Where(y=>y.fkAssociado == id).Any())
                            ent.bPrincipal = true;

                        if (ent.id == 0)
                        {
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddAddress,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, ent.stRua + " "+ ent.stNumero + " " + ent.stReferencia, "");

                            db.Insert(ent);
                        }                            
                        else
                        {
                            var oldEnd = db.AssociadoEndereco.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            if (oldEnd != null)
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonEditAddress,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, oldEnd.stRua + " " + oldEnd.stNumero + " " + oldEnd.stReferencia + " => " +
                                       ent.stRua + " " + ent.stNumero + " " + ent.stReferencia, "");

                            db.Update(ent);
                        }

                        return true;
                    }

                case "removeEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<AssociadoEndereco>(anexedEntity.ToString());

                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonRemoveAddress,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, ent.stRua + " " + ent.stNumero + " " + ent.stReferencia, "");

                        db.Delete(ent);

                        if (db.AssociadoEndereco.Where(y => y.fkAssociado == id).Count() == 1)
                        {
                            var pa = db.AssociadoEndereco.Where(y => y.fkAssociado == id).FirstOrDefault();

                            pa.bPrincipal = true;

                            db.Update(pa);
                        }

                        return true;
                    }
            }
            
            dtLastUpdate = DateTime.Now;
            fkUserLastUpdate = user.id;

            db.Update(this);

            return true;
		}
	}
}
