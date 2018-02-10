using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class ListagemCredenciadoAutorizacaoFilter
    {
        public int skip, take, situacao;
        public string tuss, nomeAssociado;
    }

    public class CredenciadoAutorizacaoReport
    {
        public int count = 0;
        public List<Autorizacao> results = new List<Autorizacao>();
    }

    public partial class Credenciado
    {
		public CredenciadoAutorizacaoReport ListagemAutorizacao ( DevKitDB db, ListagemCredenciadoAutorizacaoFilter filter )
		{
            var ret = new CredenciadoAutorizacaoReport();

            var query = from e in db.Autorizacao
                        where e.fkCredenciado == db.currentCredenciado.id
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

            var count = query.Count();

            query = query.OrderByDescending(y => y.dtSolicitacao);

            return new CredenciadoAutorizacaoReport
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
