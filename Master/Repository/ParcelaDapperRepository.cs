using Dapper;
using Master.Data.Database;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface IParcelaDapperRepository
    {
        List<Parcela> GetParcelasDeCartao(NpgsqlConnection db, long? nuParcela, List<long> ids);
    }

    public class ParcelaDapperRepository : IParcelaDapperRepository
    {
        public List<Parcela> GetParcelasDeCartao(NpgsqlConnection db, long? nuParcela, List<long> ids)
        {
            var strIds = "(";

            for (int i = 0; i < ids.Count; i++)
                strIds += ids[i] + ",";

            strIds = strIds.TrimEnd(',') + ")";

            return db.Query<Parcela>("select * from \"Parcela\" where \"nuParcela\" >= @nuParcela and \"fkCartao\" IN " + strIds, new { nuParcela }).ToList();
        }
    }
}
