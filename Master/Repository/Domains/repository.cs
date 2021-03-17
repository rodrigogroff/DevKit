using Dapper;
using Master.Infra.Entity.Database;
using Npgsql;
using System;

namespace Master.Repository
{
    public interface IDapperRepository
    {
        Cartao GetCartao(NpgsqlConnection db, long? id);

        Cartao GetCartao(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula, long? fkTitularidade);

        Empresa GetEmpresa(NpgsqlConnection db, long? id);

        Empresa GetEmpresaNum(NpgsqlConnection db, long? nuEmpresa);

        void InsertCartao(NpgsqlConnection db, Cartao mdl);

        void InsertEmpresa(NpgsqlConnection db, Empresa mdl);        
    }

    public class DapperRepository : IDapperRepository
    {
        public Cartao GetCartao(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula, long? nuTitularidade)
        {
            return db.QueryFirstOrDefault<Cartao>("select * from \"Cartao\" where \"fkEmpresa\"=" + fkEmpresa + " and \"nuMatricula\"=" + nuMatricula + " and \"nuTitularidade\"=" + nuTitularidade);
        }

        Cartao IDapperRepository.GetCartao(NpgsqlConnection db, long? id)
        {
            return db.QueryFirstOrDefault<Cartao>("select * from \"Cartao\" where \"id\" = " + id);
        }

        public Empresa GetEmpresa(NpgsqlConnection db, long? id)
        {
            return db.QueryFirstOrDefault<Empresa>("select * from \"Empresa\" where \"id\" = " + id);
        }

        public Empresa GetEmpresaNum(NpgsqlConnection db, long? nuEmpresa)
        {
            return db.QueryFirstOrDefault<Empresa>("select * from \"Empresa\" where \"nuEmpresa\" = " + nuEmpresa);
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

        public void InsertEmpresa(NpgsqlConnection db, Empresa mdl)
        {
            #region - code -

            using (var cmd = new NpgsqlCommand("INSERT INTO \"Empresa\" (\"nuEmpresa\",\"stCNPJ\",\"stFantasia\",\"stSocial\",\"stEndereco\",\"stCidade\"," +
                                                                    "\"stEstado\",\"stCEP\",\"stTelefone\",\"nuParcelas\",\"bBlocked\",\"fkAdmin\",\"stContaDeb\",\"vrMensalidade\",\"nuPctValor\",\"vrTransacao\"," +
                                                                    "\"vrMinimo\",\"nuFranquiaTrans\",\"nuPeriodoFat\",\"nuDiaVenc\",\"stBancoFat\",\"vrCartaoAtivo\",\"bIsentoFat\",\"stObs\"," +
                                                                    "\"stHomepage\",\"nuDiaFech\",\"stHoraFech\",\"bConvenioSaldo\",\"fkParceiro\",\"stEmailPlastico\" ) " +
                                                                    "VALUES ( @nuEmpresa,@stCNPJ,@stFantasia,@stSocial,@stEndereco,@stCidade," +
                                                                    "@stEstado,@stCEP,@stTelefone,@nuParcelas,@bBlocked,@fkAdmin,@stContaDeb,@vrMensalidade,@nuPctValor,@vrTransacao," +
                                                                    "@vrMinimo,@nuFranquiaTrans,@nuPeriodoFat,@nuDiaVenc,@stBancoFat,@vrCartaoAtivo,@bIsentoFat,@stObs," +
                                                                    "@stHomepage,@nuDiaFech,@stHoraFech,@bConvenioSaldo,@fkParceiro,@stEmailPlastico );", db))
            {
                cmd.Parameters.AddWithValue("nuEmpresa", ((object)mdl.nuEmpresa) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCNPJ", ((object)mdl.stCNPJ) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stFantasia", ((object)mdl.stFantasia) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stSocial", ((object)mdl.stSocial) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEndereco", ((object)mdl.stEndereco) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCidade", ((object)mdl.stCidade) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEstado", ((object)mdl.stEstado) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCEP", ((object)mdl.stCEP) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stTelefone", ((object)mdl.stTelefone) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuParcelas", ((object)mdl.nuParcelas) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bBlocked", ((object)mdl.bBlocked) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkAdmin", ((object)mdl.fkAdmin) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stContaDeb", ((object)mdl.stContaDeb) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrMensalidade", ((object)mdl.vrMensalidade) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuPctValor", ((object)mdl.nuPctValor) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrTransacao", ((object)mdl.vrTransacao) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrMinimo", ((object)mdl.vrMinimo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuFranquiaTrans", ((object)mdl.nuFranquiaTrans) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuPeriodoFat", ((object)mdl.nuPeriodoFat) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuDiaVenc", ((object)mdl.nuDiaVenc) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stBancoFat", ((object)mdl.stBancoFat) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrCartaoAtivo", ((object)mdl.vrCartaoAtivo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bIsentoFat", ((object)mdl.bIsentoFat) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stObs", ((object)mdl.stObs) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stHomepage", ((object)mdl.stHomepage) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuDiaFech", ((object)mdl.nuDiaFech) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stHoraFech", ((object)mdl.stHoraFech) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bConvenioSaldo", ((object)mdl.bConvenioSaldo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkParceiro", ((object)mdl.fkParceiro) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEmailPlastico", ((object)mdl.stEmailPlastico) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            #endregion
        }
    }
}
