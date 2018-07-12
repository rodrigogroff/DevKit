﻿using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class TUSSFilter : BaseFilter
    {
        public string   codigo,
                        codigoCred,
                        procedimento,
                        emp;

        public bool? aut;
    }

    public partial class TUSS
    {
        public TUSSReport ComposedFilters(DevKitDB db, TUSSFilter filter)
        {
            if (filter.procedimento != null)
                filter.procedimento = filter.procedimento.ToUpper();

            var lstTUSSCred = new List<long?>();

            if (filter.codigoCred != null && (filter.aut != null && filter.aut == true))
            {
                var cred = db.Credenciado.
                            Where(y => y.nuCodigo == Convert.ToInt64(filter.codigoCred)).
                            FirstOrDefault();

                lstTUSSCred = db.CredenciadoEmpresaTuss.
                                Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                Where(y => y.fkCredenciado == cred.id).
                                Select(y => y.nuTUSS).
                                ToList();
            }

            var query = from e in db.TUSS select e;

            if (!string.IsNullOrEmpty(filter.busca))
            {
                query = from e in query
                        where e.stProcedimento.Contains(filter.busca)
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.codigo))
            {
                query = from e in query
                        where e.nuCodTUSS == Convert.ToInt64(filter.codigo)
                        select e;
            }

            if (lstTUSSCred.Any())
            {
                query = from e in query
                        where lstTUSSCred.Contains(e.nuCodTUSS)
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.procedimento))
            {
                query = from e in query
                        where e.stProcedimento.Contains(filter.procedimento)
                        select e;
            }

            if (filter.aut != null && filter.aut == true)
            {
                if (db.currentCredenciado != null)
                {
                    var secTb = db.EmpresaSecao.
                                    Where(y => y.nuEmpresa == Convert.ToInt32(filter.emp)).
                                    FirstOrDefault();
                    
                    if (secTb != null)
                    {
                        var lst = db.CredenciadoEmpresaTuss.
                                Where(y => y.fkEmpresa == secTb.fkEmpresa &&
                                           y.fkCredenciado == db.currentCredenciado.id).
                                Select(y => y.nuTUSS).
                                ToList();

                        query = from e in query
                                where lst.Contains(e.nuCodTUSS)
                                select e;
                    }                    
                }
            }

            var count = query.Count();

            query = query.OrderBy(y => y.stProcedimento);

            return new TUSSReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
