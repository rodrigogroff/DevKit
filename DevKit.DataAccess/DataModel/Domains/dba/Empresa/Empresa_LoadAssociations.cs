using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Empresa
	{
		public Empresa LoadAssociations(DevKitDB db)
		{
            var setup = db.GetSetup();

            sqtdCartoes = db.Associado.
                            Where(y => y.fkEmpresa == id).
                                Count().
                                ToString();

            secoes = LoadSecoes(db);
            telefones = LoadTelefones(db);
            emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);

            return this;
		}

        List<EmpresaSecao> LoadSecoes(DevKitDB db)
        {
            return (from e in db.EmpresaSecao where e.fkEmpresa == id select e).
                OrderByDescending(t => t.id).
                ToList();
        }

        List<EmpresaTelefone> LoadTelefones(DevKitDB db)
        {
            return (from e in db.EmpresaTelefone where e.fkEmpresa == id select e).
                OrderBy(t => t.id).
                ToList();
        }

        List<EmpresaEmail> LoadEmails(DevKitDB db)
        {
            return (from e in db.EmpresaEmail where e.fkEmpresa == id select e).
                OrderByDescending(t => t.id).
                ToList();
        }

        List<EmpresaEndereco> LoadEnderecos(DevKitDB db)
        {
            var lst = (from e in db.EmpresaEndereco where e.fkEmpresa == id select e).
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
