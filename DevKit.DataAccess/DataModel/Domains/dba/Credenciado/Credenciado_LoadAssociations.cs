using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Credenciado
    {
		public Credenciado LoadAssociations(DevKitDB db)
		{
            if (fkEspecialidade != null)
                sfkEspecialidade = db.Especialidade.
                                      Where(y => y.id == fkEspecialidade).
                                      FirstOrDefault().
                                      stNome;

            switch (nuTipo)
            {
                case TipoCredenciado.Medico: snuTipo = "Médico"; break;
                case TipoCredenciado.Clinica: snuTipo = "Clínica"; break;
                case TipoCredenciado.Laboratorio: snuTipo = "Laboratorio"; break;
            }
            
            procedimentos = LoadProcedimentos(db);
            empresas = LoadEmpresas(db);
            phones = LoadPhones(db);
            emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);

            return this;
		}

        List<CredenciadoEmpresaTuss> LoadProcedimentos(DevKitDB db)
        {
            var query = (from e in db.CredenciadoEmpresaTuss
                         where e.fkCredenciado == id
                         select e);

            if (db.currentUser != null)
            {
                query = from e in query
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;
            }

            var lst = query.ToList();

            var mon = new money();
            
            // associations
            foreach (var item in lst)
            {
                item.stProcedimento = db.TUSS.Where(y => y.nuCodTUSS == item.nuTUSS).
                                        FirstOrDefault().
                                        stProcedimento;

                if (item.vrCoPart != null)
                    item.svrCoPart = mon.setMoneyFormat((long)item.vrCoPart);

                if (item.vrProcedimento != null)
                    item.svrProcedimento = mon.setMoneyFormat((long)item.vrProcedimento);

                if (item.tgCob == true)
                    item.stgCob = "Sim";
                else if (item.tgCob == false)
                    item.stgCob = "Não";
            }

            return lst;
        }

        List<CredenciadoEmpresa> LoadEmpresas(DevKitDB db)
        {
            var lst = (from e in db.CredenciadoEmpresa where e.fkCredenciado == id select e).
                ToList();

            foreach (var item in lst)
            {
                item.sfkEmpresa = db.Empresa.Where(y => y.id == item.fkEmpresa).FirstOrDefault().stNome;
            }

            return lst;
        }

        List<CredenciadoTelefone> LoadPhones(DevKitDB db)
        {
            return (from e in db.CredenciadoTelefone where e.fkCredenciado == id select e).
                OrderBy(t => t.stPhone).
                ToList();
        }

        List<CredenciadoEmail> LoadEmails(DevKitDB db)
        {
            return (from e in db.CredenciadoEmail where e.fkCredenciado == id select e).
                OrderByDescending(t => t.id).
                ToList();
        }

        List<CredenciadoEndereco> LoadEnderecos(DevKitDB db)
        {
            var lst = (from e in db.CredenciadoEndereco where e.fkCredenciado == id select e).
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
