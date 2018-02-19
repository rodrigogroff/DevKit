using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public partial class Associado
    {
		public Associado LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

            sdtLastUpdate = dtLastUpdate?.ToString(setup.stDateFormat);
            sdtLastContact = dtLastContact?.ToString(setup.stDateFormat);
            sdtStart = dtStart?.ToString(setup.stDateFormat);

            if (fkUserAdd != null)
                sfkUserAdd = db.GetUser(fkUserAdd).stLogin;

            if (fkUserLastContact != null)
                sfkUserLastContact = db.GetUser(fkUserLastContact).stLogin;

            if (fkUserLastUpdate != null)
                sfkUserLastUpdate = db.GetUser(fkUserLastUpdate).stLogin;

            if (fkSecao != null)
                sfkSecao = db.EmpresaSecao.
                                Where(y => y.id == fkSecao).
                                Select (y=> y.nuEmpresa.ToString() + " - " + y.stDesc).
                                FirstOrDefault();

            if (nuYearBirth != null)
                snuAge = (DateTime.Now.Year - nuYearBirth).ToString();

            switch (this.tgExpedicao)
            {
                case 0: stgExpedicao = "Requerido"; break;
                case 1: stgExpedicao = "Em produção"; break;
                case 2: stgExpedicao = "Entregue"; break;
            }

            switch (this.tgStatus)
            {
                case 0: stgStatus = "Habilitado"; break;
                case 1: stgStatus = "Bloqueado"; break;
            }

            phones = LoadPhones(db);
			emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);
            dependentes = LoadDependentes(db);

            return this;
		}

        List<AssociadoDependente> LoadDependentes(DevKitDB db)
        {
            var tmpLst = (from e in db.AssociadoDependente where e.fkAssociado == id select e).
                OrderBy(t => t.id).
                ToList();

            foreach (var item in tmpLst)
                item.snuTit = db.Associado.
                                Where(y => y.id == item.fkAssociado).
                                FirstOrDefault().
                                nuTitularidade.
                                ToString();
            
            return tmpLst;
        }

        List<AssociadoTelefone> LoadPhones(DevKitDB db)
		{
			return (from e in db.AssociadoTelefone where e.fkAssociado == id select e).
				OrderBy(t => t.stPhone).
				ToList();
		}

		List<AssociadoEmail> LoadEmails(DevKitDB db)
		{
			return (from e in db.AssociadoEmail where e.fkAssociado == id select e).
				OrderByDescending(t => t.id).
				ToList();
		}

        List<AssociadoEndereco> LoadEnderecos(DevKitDB db)
        {
            var lst = (from e in db.AssociadoEndereco where e.fkAssociado == id select e).
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
