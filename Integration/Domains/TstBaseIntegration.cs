using Npgsql;
using SqlLocalDb;
using System.IO;

namespace Integration
{
    public class TstBaseIntegration
    {
        public string connStr = "User ID=postgres;Password=Gustavo123;Host=localhost;Port=5432;Database=NanoServerIntegration;";
        public string baseDb = File.ReadAllText(@"Repository\CreateDB_pg.sql");
        public string setupDb = File.ReadAllText(@"Repository\StartupDB_pg.sql");

        public string[] truncateTables =
        {
            "User",
            "Brand",
            "Product",
            "ProductCatalog",
            "ProductCatalogLink"
        };

        public void SetupDatabase()
        {
            using var db = new NpgsqlConnection(connStr);
            db.Open();
            db.ExecuteScript(baseDb);
            db.ExecuteScript(setupDb);

            foreach (var item in truncateTables)
                db.ExecuteScript("truncate table \"" + item + "\" RESTART IDENTITY");

            db.Close();
        }
    }
}
