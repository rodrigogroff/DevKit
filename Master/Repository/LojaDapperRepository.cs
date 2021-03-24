using Master.Data.Database;
using Npgsql;
using System;

namespace Master.Repository
{
    public interface ILojaDapperRepository
    {
        void InsertLoja(NpgsqlConnection db, Loja novo);
        void InsertTerminal(NpgsqlConnection db, Terminal novo);
    }

    public class LojaDapperRepository : ILojaDapperRepository
    {
        public void InsertLoja(NpgsqlConnection db, Loja novo)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Loja\" (\"bBlocked\",\"bCancel\",\"bIsentoFat\",\"bPortalSenha\",\"fkBanco\",\"nuBancoFat\"," +
                                                                    "\"nuDiaVenc\",\"nuFranquia\",\"nuPctValor\",\"nuPeriodoFat\",\"nuTipoCob\",\"stAgencia\",\"stCelular\"," +
                                                                    "\"stCEP\",\"stCidade\",\"stCNPJ\",\"stConta\",\"stContaDeb\",\"stContato\",\"stCPFResp\"," +
                                                                    "\"stDataResp\",\"stEmail\",\"stEndereco\",\"stEnderecoInst\",\"stEstado\",\"stFax\",\"stInscEst\",\"stLoja\",\"stNome\"," +
                                                                    "\"stObs\",\"stSenha\",\"stSocial\",\"stTelefone\",\"vrMensalidade\",\"vrMinimo\",\"vrTransacao\" ) " +
                                                                    "VALUES ( @bBlocked,@bCancel,@bIsentoFat,@bPortalSenha,@fkBanco,@nuBancoFat," +
                                                                    "@nuDiaVenc,@nuFranquia,@nuPctValor,@nuPeriodoFat,@nuTipoCob,@stAgencia,@stCelular," +
                                                                    "@stCEP,@stCidade,@stCNPJ,@stConta,@stContaDeb,@stContato,@stCPFResp," +
                                                                    "@stDataResp,@stEmail,@stEndereco,@stEnderecoInst,@stEstado,@stFax,@stInscEst,@stLoja,@stNome," +
                                                                    "@stObs,@stSenha,@stSocial,@stTelefone,@vrMensalidade,@vrMinimo,@vrTransacao );", db))
            {
                cmd.Parameters.AddWithValue("bBlocked", ((object)novo.bBlocked) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bCancel", ((object)novo.bCancel) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bIsentoFat", ((object)novo.bIsentoFat) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bPortalSenha", ((object)novo.bPortalSenha) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkBanco", ((object)novo.fkBanco) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuBancoFat", ((object)novo.nuBancoFat) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuDiaVenc", ((object)novo.nuDiaVenc) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuFranquia", ((object)novo.nuFranquia) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuPctValor", ((object)novo.nuPctValor) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuPeriodoFat", ((object)novo.nuPeriodoFat) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTipoCob", ((object)novo.nuTipoCob) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stAgencia", ((object)novo.stAgencia) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCelular", ((object)novo.stCelular) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCEP", ((object)novo.stCEP) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCidade", ((object)novo.stCidade) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCNPJ", ((object)novo.stCNPJ) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stConta", ((object)novo.stConta) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stContaDeb", ((object)novo.stContaDeb) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stContato", ((object)novo.stContato) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stCPFResp", ((object)novo.stCPFResp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stDataResp", ((object)novo.stDataResp) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEmail", ((object)novo.stEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEndereco", ((object)novo.stEndereco) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEnderecoInst", ((object)novo.stEnderecoInst) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stEstado", ((object)novo.stEstado) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stFax", ((object)novo.stFax) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stInscEst", ((object)novo.stInscEst) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stLoja", ((object)novo.stLoja) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stNome", ((object)novo.stNome) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stObs", ((object)novo.stObs) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stSenha", ((object)novo.stSenha) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stSocial", ((object)novo.stSocial) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stTelefone", ((object)novo.stTelefone) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrMensalidade", ((object)novo.vrMensalidade) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrMinimo", ((object)novo.vrMinimo) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("vrTransacao", ((object)novo.vrTransacao) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void InsertTerminal(NpgsqlConnection db, Terminal novo)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Terminal\" (\"fkLoja\",\"stLocal\",\"stTerminal\" ) " +
                                                                "VALUES ( @fkLoja,@stLocal,@stTerminal );", db))
            {
                cmd.Parameters.AddWithValue("fkLoja", ((object)novo.fkLoja) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stLocal", ((object)novo.stLocal) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stTerminal", ((object)novo.stTerminal) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
