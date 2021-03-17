using Dapper;
using Master.Data.Database;
using Npgsql;
using System;

namespace Master.Repository
{
    public interface IEmpresaDapperRepository
    {
        Empresa GetEmpresa(NpgsqlConnection db, long? id);

        Empresa GetEmpresaNum(NpgsqlConnection db, long? nuEmpresa);

        void InsertEmpresa(NpgsqlConnection db, Empresa mdl);
    }

    public class EmpresaDapperRepository : IEmpresaDapperRepository
    {
        public Empresa GetEmpresa(NpgsqlConnection db, long? id)
        {
            return db.QueryFirstOrDefault<Empresa>("select * from \"Empresa\" where \"id\" = " + id);
        }

        public Empresa GetEmpresaNum(NpgsqlConnection db, long? nuEmpresa)
        {
            return db.QueryFirstOrDefault<Empresa>("select * from \"Empresa\" where \"nuEmpresa\" = " + nuEmpresa);
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
