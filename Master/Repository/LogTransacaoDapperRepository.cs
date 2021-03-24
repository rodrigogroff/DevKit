using Dapper;
using Master.Data.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface ILogTransacaoDapperRepository
    {
        List<LogTransacao> GetLogTransacaoLista(NpgsqlConnection db, List<long> ids);

        void InsertLogTransacao(NpgsqlConnection db, LogTransacao mdl);
    }

    public class LogTransacaoDapperRepository : ILogTransacaoDapperRepository
    {
        public List<LogTransacao> GetLogTransacaoLista(NpgsqlConnection db, List<long> ids)
        {
            if (ids.Count == 0)
                return new List<LogTransacao>();

            var strIds = "(";

            for (int i = 0; i < ids.Count; i++)
                strIds += ids[i] + ",";

            strIds = strIds.TrimEnd(',') + ")";

            return db.Query<LogTransacao>("select * from \"LogTransacao\" where \"id\" in " + strIds ).ToList();
        }

        public void InsertLogTransacao(NpgsqlConnection db, LogTransacao novo)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"LogTransacao\" (\"bContabil\",\"dtTransacao\",\"fkCartao\",\"fkEmpresa\",\"fkLoja\",\"fkTerminal\",\"nuCodErro\"," +
                                                                    "\"nuConfirmada\",\"nuNsu\",\"nuNsuOrig\",\"nuOperacao\",\"nuParcelas\",\"stDoc\",\"stMsg\",\"vrTotal\"  ) " +
                                                                    "VALUES ( @bContabil,@dtTransacao,@fkCartao,@fkEmpresa,@fkLoja,@fkTerminal,@nuCodErro," +
                                                                    "@nuConfirmada,@nuNsu,@nuNsuOrig,@nuOperacao,@nuParcelas,@stDoc,@stMsg,@vrTotal );", db))
            {
                cmd.Parameters.AddWithValue("bContabil", ((object)novo.bContabil) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtTransacao", ((object)novo.dtTransacao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkCartao", ((object)novo.fkCartao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkEmpresa", ((object)novo.fkEmpresa) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkTerminal", ((object)novo.fkTerminal) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuCodErro", ((object)novo.nuCodErro) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuConfirmada", ((object)novo.nuConfirmada) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuNsu", ((object)novo.nuNsu) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuNsuOrig", ((object)novo.nuNsuOrig) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuOperacao", ((object)novo.nuOperacao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuParcelas", ((object)novo.nuParcelas) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stDoc", ((object)novo.stDoc) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stMsg", ((object)novo.stMsg) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrTotal", ((object)novo.vrTotal) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
