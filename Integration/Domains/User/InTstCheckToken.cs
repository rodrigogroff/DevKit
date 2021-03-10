using Api.Master.Controllers;
using Entities.Api.User;
using Master.Infra.Entity.Database;
using Master.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System;

namespace Integration
{
    [TestClass]
    public class InTstCheckToken : TstBaseIntegration
    {
        /*
        CtrlCheckToken Setup(User u)
        {
            
            #region - code - 

            var tst = new CtrlCheckToken(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

            SetupDatabase();

            if (u != null)
                using (var db = new NpgsqlConnection(connStr))
                {
                    db.Open();
                    u.compile();
                    new DapperRepository().InsertUser(db, u);
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
                stEmail = "123456@123456.com",
                dtTokenExpires = DateTime.Now.AddSeconds (60),
                stToken = "102030",
            });

            var ret = tst.Post(new DtoCheckToken
            {                
                sToken = "102030",
                sID = "90511603053",
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void Fail_invalid_token()
        {
            var tst = Setup(new User
            {
                stSocialID = "90511603053",
                stEmail = "123456@123456.com",
                dtTokenExpires = DateTime.Now.AddSeconds(60),
                stToken = "102030",
            });

            var ret = tst.Post(new DtoCheckToken
            {
                sToken = "999999",
                sID = "90511603053",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void Fail_token_expired()
        {
            var tst = Setup(new User
            {
                stSocialID = "90511603053",
                stEmail = "123456@123456.com",
                dtTokenExpires = DateTime.Now.AddSeconds(-60),
                stToken = "102030",
            });

            var ret = tst.Post(new DtoCheckToken
            {
                _language = 0,
                sToken = "999999",
                sID = "90511603053",
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }
        */
    }
}
