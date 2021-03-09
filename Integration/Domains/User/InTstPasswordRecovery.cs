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
    public class InTstPasswordRecovery : TstBaseIntegration
    {
        CtrlPasswordRecovery Setup(User u)
        {
            #region - code - 

            var tst = new CtrlPasswordRecovery(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

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
                id = 1,
                bActive = true,
                bAdmin = false,
                bTokenized = true,
                dtCreation = DateTime.Now,
                stSocialID = "90511603053",
                stToken = "",
                dtLastLogin = DateTime.Now,
                stEmail = "rodrigo.groff@gmail.com",
                stName = "rodrigo",
                stPassword = "rodrigo"
            });

            // do NOT send email on testing....
            tst._sendEmail = false;

            var ret = tst.Post(new DtoPasswordRecovery
            {
                email = "rodrigo.groff@gmail.com",
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void UserNotExists()
        {
            var tst = Setup(null);

            // do NOT send email on testing....
            tst._sendEmail = false;

            var ret = tst.Post(new DtoPasswordRecovery
            {
                _language = 0,
                email = "rodrigo.groff@gmail.com.brrrrrrrrrr",                
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }
    }
}
