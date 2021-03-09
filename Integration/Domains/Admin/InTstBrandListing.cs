using Api.Master.Controllers;
using Entities.Api.Admin;
using Master.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace Integration
{
    [TestClass]
    public class InTstBrandListing : TstBaseIntegration
    {
        CtrlBrand Setup()
        {
            #region - code - 

            var tst = new CtrlBrand(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

            SetupDatabase();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();

                var repo = new DapperAdminRepository();

                repo.InsertBrand(db, new Master.Infra.Entity.Database.Brand
                {
                    stName = "Samsung"
                });

                repo.InsertBrand(db, new Master.Infra.Entity.Database.Brand
                {
                    stName = "Toshiba"
                });

                db.Close();
            }

            return tst;

            #endregion
        }

        [TestMethod]
        public void OK()
        {
            var tst = Setup();

            var ret = tst.brandListing(new DtoBrandListing
            {                
                page = 0,
                pageSize = 99,
                orderBy = 1,
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();

            var OkResult = ret as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var dtoResp = OkResult.Value as DtoBrandListingResult;

            if (dtoResp.totalRecords != 2)
                Assert.Fail();

            if (dtoResp.results.Count != 2)
                Assert.Fail();
        }        
    }
}
