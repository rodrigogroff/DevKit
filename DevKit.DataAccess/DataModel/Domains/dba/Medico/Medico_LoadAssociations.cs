using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Medico
    {
		public Medico LoadAssociations(DevKitDB db)
		{
            this.sfkEspecialidade = db.Especialidade.
                                    Where(y => y.id == this.fkEspecialidade).
                                    FirstOrDefault().
                                    stNome;

            empresas = LoadEmpresas(db);
            phones = LoadPhones(db);
            emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);

            return this;
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
