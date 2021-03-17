using Dapper;
using Npgsql;
using System.IO;

namespace IntegrationTest
{
    public class TstBaseIntegration
    {
        public string connStr = "User ID=postgres;Password=Gustavo123;Host=localhost;Port=5432;Database=ConveynetIntegration;";
        public string baseDb = File.ReadAllText(@"Repository\CreateDB_pg.sql");
        
        public string[] truncateTables =
        {
            "Cartao",
            "ConfigPlasticoEnvio",
            "DashboardGrafico",
            "Empresa",
            "Faturamento",
            "FaturamentoDetalhe",
            "JobFechamento",
            "LogAudit",
            "LogFechamento",
            "LogNsu",
            "LogTransacao",
            "Loja",
            "LojaEmpresa",
            "LojaMsg",
            "LoteCartao",
            "LoteCartaoDetalhe",
            "Parceiro",
            "Parcela",
            "SaldoConvenio",
            "SolicitacaoVenda",
            "Terminal",
            "UsuarioEmissor",
            "UsuarioParceiro",
        };

        public void SetupDatabase()
        {
            using var db = new NpgsqlConnection(connStr);
            db.Open();

            new NpgsqlCommand(baseDb, db).ExecuteNonQuery();

            foreach (var item in truncateTables)
                db.Query("truncate table \"" + item + "\" RESTART IDENTITY");

            db.Close();
        }
    }
}
