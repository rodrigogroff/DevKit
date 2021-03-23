using Dapper;
using Master.Data.Database;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface ILogTransacaoDapperRepository
    {
        List<LogTransacao> GetLogTransacaoLista(NpgsqlConnection db, List<long> ids);
    }

    public class LogTransacaoDapperRepository : ILogTransacaoDapperRepository
    {
        public List<LogTransacao> GetLogTransacaoLista(NpgsqlConnection db, List<long> ids)
        {
            var strIds = "(";

            for (int i = 0; i < ids.Count; i++)
                strIds += ids[i] + ",";

            strIds = strIds.TrimEnd(',') + ")";

            return db.Query<LogTransacao>("select * from \"LogTransacao\" where \"id\" in " + strIds ).ToList();
        }
    }
}
