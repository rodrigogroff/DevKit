using Dapper;
using Master.Data.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface ICartaoDapperRepository
    {
        Cartao GetCartao(NpgsqlConnection db, long? id);

        Cartao GetCartao(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula, long? fkTitularidade);

        List<long> GetListaCartoes(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula);

        void InsertCartao(NpgsqlConnection db, Cartao mdl);        
    }

    public class CartaoDapperRepository : ICartaoDapperRepository
    {
        public Cartao GetCartao(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula, long? nuTitularidade)
        {
            return db.QueryFirstOrDefault<Cartao>("select * from \"Cartao\" where \"fkEmpresa\"=" + fkEmpresa + " and \"nuMatricula\"=" + nuMatricula + " and \"nuTitularidade\"=" + nuTitularidade);
        }

        public Cartao GetCartao(NpgsqlConnection db, long? id)
        {
            return db.QueryFirstOrDefault<Cartao>("select * from \"Cartao\" where \"id\" = " + id);
        }

        public List<long> GetListaCartoes(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula)
        {
            return db.Query<Cartao>("select * from \"Cartao\" where \"fkEmpresa\"=" + fkEmpresa + " and \"nuMatricula\"=" + nuMatricula ).
                        ToList().
                        Select(y=> y.id).
                        ToList();
        }

        public void InsertCartao(NpgsqlConnection db, Cartao mdl)
        {
            #region - code - 

            using (var cmd = new NpgsqlCommand("INSERT INTO \"Cartao\" (\"fkEmpresa\",\"nuMatricula\",\"nuTitularidade\",\"stSenha\",\"nuTipoCartao\"," +
                                                                    "\"stVenctoCartao\",\"nuStatus\",\"nuSenhaErrada\",\"dtInclusao\",\"dtBloqueio\",\"nuMotivoBloqueio\",\"stBanco\"," +
                                                                    "\"stAgencia\",\"stConta\",\"stMatExtra\",\"stCelCartao\",\"stCpf\",\"stNome\",\"stEndereco\",\"stNumero\",\"stCompl\"," +
                                                                    "\"stBairro\",\"stEstado\",\"stCidade\",\"stCEP\",\"stDDD\",\"stTelefone\",\"dtNasc\",\"stEmail\",\"vrRenda\",\"nuViaCartao\"," +
                                                                    "\"vrLimiteTotal\",\"vrLimiteMensal\",\"vrLimiteRotativo\",\"vrCotaExtra\",\"nuEmitido\",\"bConvenioComSaldo\",\"vrSaldoConvenio\"," +
                                                                    "\"dtPedidoCartao\" ) " +
                                                                    "VALUES ( @fkEmpresa,@nuMatricula,@nuTitularidade,@stSenha,@nuTipoCartao,@stVenctoCartao,@nuStatus,@nuSenhaErrada," +
                                                                    "@dtInclusao,@dtBloqueio,@nuMotivoBloqueio,@stBanco,@stAgencia,@stConta,@stMatExtra,@stCelCartao," +
                                                                    "@stCpf,@stNome,@stEndereco,@stNumero,@stCompl,@stBairro,@stEstado,@stCidade,@stCEP,@stDDD,@stTelefone,@dtNasc,@stEmail,@vrRenda," +
                                                                    "@nuViaCartao,@vrLimiteTotal,@vrLimiteMensal,@vrLimiteRotativo,@vrCotaExtra,@nuEmitido,@bConvenioComSaldo,@vrSaldoConvenio,@dtPedidoCartao);", db))
            {
                cmd.Parameters.AddWithValue("fkEmpresa", ((object)mdl.fkEmpresa) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuMatricula", ((object)mdl.nuMatricula) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTitularidade", ((object)mdl.nuTitularidade) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stSenha", ((object)mdl.stSenha) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTipoCartao", ((object)mdl.nuTipoCartao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stVenctoCartao", ((object)mdl.stVenctoCartao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuStatus", ((object)mdl.nuStatus) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuSenhaErrada", ((object)mdl.nuSenhaErrada) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtInclusao", ((object)mdl.dtInclusao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtBloqueio", ((object)mdl.dtBloqueio) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuMotivoBloqueio", ((object)mdl.nuMotivoBloqueio) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stBanco", ((object)mdl.stBanco) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stAgencia", ((object)mdl.stAgencia) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stConta", ((object)mdl.stConta) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stMatExtra", ((object)mdl.stMatExtra) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCelCartao", ((object)mdl.stCelCartao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCpf", ((object)mdl.stCpf) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stNome", ((object)mdl.stNome) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEndereco", ((object)mdl.stEndereco) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stNumero", ((object)mdl.stNumero) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCompl", ((object)mdl.stCompl) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stBairro", ((object)mdl.stBairro) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEstado", ((object)mdl.stEstado) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCidade", ((object)mdl.stCidade) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCEP", ((object)mdl.stCEP) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stDDD", ((object)mdl.stDDD) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stTelefone", ((object)mdl.stTelefone) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtNasc", ((object)mdl.dtNasc) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEmail", ((object)mdl.stEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrRenda", ((object)mdl.vrRenda) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuViaCartao", ((object)mdl.nuViaCartao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrLimiteTotal", ((object)mdl.vrLimiteTotal) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrLimiteMensal", ((object)mdl.vrLimiteMensal) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrLimiteRotativo", ((object)mdl.vrLimiteRotativo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrCotaExtra", ((object)mdl.vrCotaExtra) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuEmitido", ((object)mdl.nuEmitido) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bConvenioComSaldo", ((object)mdl.bConvenioComSaldo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrSaldoConvenio", ((object)mdl.vrSaldoConvenio) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtPedidoCartao", ((object)mdl.dtPedidoCartao) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            #endregion
        }
    }
}
