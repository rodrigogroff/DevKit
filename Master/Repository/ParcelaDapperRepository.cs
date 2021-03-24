using Dapper;
using Master.Data.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface IParcelaDapperRepository
    {
        List<Parcela> GetParcelasDeCartao(NpgsqlConnection db, long? nuParcela, List<long> ids);

        void InsertParcela(NpgsqlConnection db, Parcela novo);
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

        public void InsertParcela(NpgsqlConnection db, Parcela novo)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Parcela\" (\"dtInclusao\",\"fkCartao\",\"fkEmpresa\",\"fkLogTransacao\",\"fkLoja\",\"fkTerminal\",\"nuIndice\",\"nuNsu\",\"nuParcela\",\"nuTotParcelas\",\"vrValor\" ) " +
                                                                "VALUES (@dtInclusao,@fkCartao,@fkEmpresa,@fkLogTransacao,@fkLoja,@fkTerminal,@nuIndice,@nuNsu,@nuParcela,@nuTotParcelas,@vrValor );", db))
            {
                cmd.Parameters.AddWithValue("dtInclusao", ((object)novo.dtInclusao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkCartao", ((object)novo.fkCartao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkLogTransacao", ((object)novo.fkLogTransacao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkTerminal", ((object)novo.fkTerminal) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuIndice", ((object)novo.nuIndice) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuNsu", ((object)novo.nuNsu) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuParcela", ((object)novo.nuParcela) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTotParcelas", ((object)novo.nuTotParcelas) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrValor", ((object)novo.vrValor) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
