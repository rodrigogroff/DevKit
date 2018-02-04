using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Medico
    {
		public Medico LoadAssociations(DevKitDB db)
		{
            sfkEspecialidade = db.Especialidade.
                                  Where(y => y.id == fkEspecialidade).
                                  FirstOrDefault().
                                  stNome;

            procedimentos = LoadProcedimentos(db);
            empresas = LoadEmpresas(db);
            phones = LoadPhones(db);
            emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);

            return this;
		}

        List<MedicoEmpresaTuss> LoadProcedimentos(DevKitDB db)
        {
            var query = (from e in db.MedicoEmpresaTuss
                         where e.fkMedico == id
                         select e);

            if (db.currentUser != null)
            {
                query = from e in query
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;
            }

            var lst = query.ToList();

            foreach (var item in lst)
            {
                item.stProcedimento = db.TUSS.Where(y => y.nuCodTUSS == item.nuTUSS).
                                        FirstOrDefault().
                                        stProcedimento;
            }

            return lst;
        }

        List<MedicoEmpresa> LoadEmpresas(DevKitDB db)
        {
            var lst = (from e in db.MedicoEmpresa where e.fkMedico == id select e).
                ToList();

            foreach (var item in lst)
            {
                item.sfkEmpresa = db.Empresa.Where(y => y.id == item.fkEmpresa).FirstOrDefault().stNome;
            }

            return lst;
        }

        List<MedicoPhone> LoadPhones(DevKitDB db)
        {
            return (from e in db.MedicoPhone where e.fkMedico == id select e).
                OrderBy(t => t.stPhone).
                ToList();
        }

        List<MedicoEmail> LoadEmails(DevKitDB db)
        {
            return (from e in db.MedicoEmail where e.fkMedico == id select e).
                OrderByDescending(t => t.id).
                ToList();
        }

        List<MedicoAddress> LoadEnderecos(DevKitDB db)
        {
            var lst = (from e in db.MedicoAddress where e.fkMedico == id select e).
                OrderByDescending(t => t.id).
                ToList();

            foreach (var item in lst)
            {
                item.sfkEstado = db.Estado.Where(y => y.id == item.fkEstado).FirstOrDefault().stNome;
                item.sfkCidade = db.Cidade.Where(y => y.id == item.fkCidade).FirstOrDefault().stNome;
            }

            return lst;
        }
    }
}
