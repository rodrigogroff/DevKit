using LinqToDB;
using SyCrafEngine;
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

            if (vrMaxProcSemAut != null)    svrMaxProcSemAut = new money().setMoneyFormat((long)vrMaxProcSemAut);
            if (vrCobMensalidade != null)   svrCobMensalidade = new money().setMoneyFormat((long)vrCobMensalidade);
            if (vrCobCartaoBase != null)    svrCobCartaoBase = new money().setMoneyFormat((long)vrCobCartaoBase);
            if (vrCobCartaoAtivo != null)   svrCobCartaoAtivo = new money().setMoneyFormat((long)vrCobCartaoAtivo);
            if (vrCobAutorizacao != null)   svrCobAutorizacao = new money().setMoneyFormat((long)vrCobAutorizacao);
            if (vrCobNovoCartao != null)    svrCobNovoCartao = new money().setMoneyFormat((long)vrCobNovoCartao);
            if (vrCobServBloq != null)      svrCobServBloq = new money().setMoneyFormat((long)vrCobServBloq);
            if (vrCobServDesbloq != null)   svrCobServDesbloq = new money().setMoneyFormat((long)vrCobServDesbloq);

            secoes = LoadSecoes(db);
            telefones = LoadTelefones(db);
            emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);
            consultas = LoadConsultas(db);

            return this;
		}

        List<EmpresaSecao> LoadSecoes(DevKitDB db)
        {
            return (from e in db.EmpresaSecao where e.fkEmpresa == id select e).
                OrderByDescending(t => t.id).
                ToList();
        }

        List<EmpresaConsultaAno> LoadConsultas(DevKitDB db)
        {
            var lst = (from e in db.EmpresaConsultaAno where e.fkEmpresa == id select e).
                OrderByDescending(t => t.nuAno ).
                ToList();

            var mon = new money();

            foreach (var item in lst)
            {
                item.svrPreco1 = mon.setMoneyFormat((long)item.vrPreco1);
                item.svrPreco2 = mon.setMoneyFormat((long)item.vrPreco2);
                item.svrPreco3 = mon.setMoneyFormat((long)item.vrPreco3);
                item.svrPreco4 = mon.setMoneyFormat((long)item.vrPreco4);
                item.svrPreco5 = mon.setMoneyFormat((long)item.vrPreco5);
                item.svrPreco6 = mon.setMoneyFormat((long)item.vrPreco6);
                item.svrPreco7 = mon.setMoneyFormat((long)item.vrPreco7);
                item.svrPreco8 = mon.setMoneyFormat((long)item.vrPreco8);
                item.svrPreco9 = mon.setMoneyFormat((long)item.vrPreco9);
            }

            return lst;
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
