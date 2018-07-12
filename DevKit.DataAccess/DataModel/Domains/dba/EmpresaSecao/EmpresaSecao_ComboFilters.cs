using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
    public partial class EmpresaSecao
    {
        public ComboReport ComboFilters(DevKitDB db, BaseFilter filter)
        {
            var query = from e in db.EmpresaSecao
                        where e.fkEmpresa == filter.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.busca))
            {
                //query = from e in query
                  //      where e.nuEmpresa == Convert.ToInt64(filter.busca)
                    //    select e;
            }

            query = from e in query orderby e.nuEmpresa select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query
                           select new BaseComboResponse
                           {
                               id = e.id,
                               stName = e.nuEmpresa.ToString() + " - " + e.stDesc
                           }).
                    ToList()
            };
        }
    }
}
