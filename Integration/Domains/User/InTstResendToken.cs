using Api.Master.Controllers;
using Entities.Api.User;
using Master.Infra.Entity.Database;
using Master.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace Integration
{
    [TestClass]
    public class InTstResendToken : TstBaseIntegration
    {
        CtrlResendToken Setup(User u)
        {
            #region - code - 

            var tst = new CtrlResendToken(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

            SetupDatabase();

            if (u != null)
                using (var db = new NpgsqlConnection(connStr))
                {
                    db.Open();
                    u.compile();
                    new DapperUserRepository().InsertUser(db, u);
                    db.Close();
                }

            return tst;

            #endregion
        }

        [TestMethod]
        public void OK()
        {
            var tst = Setup(new User
            {
                stSocialID = "90511603053",                
                stEmail = "a@a.com",
            });

            tst._sendEmail = false;

            var ret = tst.Post(new DtoResendToken
            {                
                sID = "90511603053",                
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void NotFound()
        {
            var tst = Setup(new User
            {
                stSocialID = "90511603053",
                stEmail = "a@a.com",
            });

            tst._sendEmail = false;

            var ret = tst.Post(new DtoResendToken
            {
                _language = 0,
                sID = "666",
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }
    }
}
