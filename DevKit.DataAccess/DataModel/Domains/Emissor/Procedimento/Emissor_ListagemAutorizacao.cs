using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class ListagemEmissorAutorizacaoFilter
    {
        public int skip, 
                   take,
                   situacao;

        public string   tuss, 
                        espec,
                        nomeAssociado, 
                        nomeCredenciado, 
                        codMedico, 
                        matricula, 
                        dtInicial, 
                        dtFim;
    }

    public class EmissorAutorizacaoReport
    {
        public int count = 0;
        public List<Autorizacao> results = new List<Autorizacao>();
    }

    public partial class Empresa
    {
		public EmissorAutorizacaoReport ListagemAutorizacao ( DevKitDB db, ListagemEmissorAutorizacaoFilter filter )
		{
            var ret = new EmissorAutorizacaoReport();

            var query = from e in db.Autorizacao
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.tuss))
            {
                query = from e in query
                        join proc in db.TUSS on e.fkProcedimento equals proc.id
                        where proc.nuCodTUSS.ToString() == filter.tuss
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.nomeAssociado))
            {
                query = from e in query
                        join assoc in db.Associado on e.fkAssociado equals assoc.id
                        where assoc.stName.ToUpper().Contains (filter.nomeAssociado.ToUpper())
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.nomeCredenciado))
            {
                query = from e in query
                        join med in db.Medico on e.fkMedico equals med.id
                        where med.stNome.ToUpper().Contains(filter.nomeCredenciado.ToUpper())
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.codMedico))
            {
                query = from e in query
                        join med in db.Medico on e.fkMedico equals med.id
                        where med.nuCodigo.ToString() == filter.codMedico
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.matricula))
            {
                query = from e in query
                        join assoc in db.Associado on e.fkAssociado equals assoc.id
                        where assoc.nuMatricula.ToString() == filter.matricula
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.dtInicial) && filter.dtInicial.Length==8)
            {
                #region - code -

                int nu_dia = Convert.ToInt32(filter.dtInicial.Substring(0, 2)),
                    nu_mes = Convert.ToInt32(filter.dtInicial.Substring(2, 2)),
                    nu_ano = Convert.ToInt32(filter.dtInicial.Substring(4, 4));

                try
                {
                    var dtInicial = new DateTime(nu_ano, nu_mes, nu_dia);

                    query = from e in query
                            where e.dtSolicitacao > dtInicial
                            select e;
                }
                catch (System.Exception ex) { }

                #endregion
            }

            if (!string.IsNullOrEmpty(filter.dtFim) && filter.dtFim.Length == 8)
            {
                #region - code -

                int nu_dia = Convert.ToInt32(filter.dtFim.Substring(0, 2)),
                    nu_mes = Convert.ToInt32(filter.dtFim.Substring(2, 2)),
                    nu_ano = Convert.ToInt32(filter.dtFim.Substring(4, 4));

                try
                {
                    var dtFim = new DateTime(nu_ano, nu_mes, nu_dia).AddDays(1);

                    query = from e in query
                            where e.dtSolicitacao < dtFim
                            select e;
                }
                catch (System.Exception ex) { }

                #endregion
            }

            if (!string.IsNullOrEmpty(filter.espec))
            {
                query = from e in query
                        join med in db.Medico on e.fkMedico equals med.id
                        join espec in db.Especialidade on med.fkEspecialidade equals espec.id
                        where espec.stNome.ToUpper() == filter.espec.ToUpper()
                        select e;
            }

            var count = query.Count();

            query = query.OrderByDescending(y => y.dtSolicitacao);

            return new EmissorAutorizacaoReport
            {
                count = count,
                results = LoaderAutorizacao(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
		}

        public List<Autorizacao> LoaderAutorizacao(DevKitDB db, List<Autorizacao> results)
        {
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
