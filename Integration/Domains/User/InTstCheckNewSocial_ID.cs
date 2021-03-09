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
    public class InTstCheckNewSocial_ID : TstBaseIntegration
    {
        CtrlCheckNewSocial_ID Setup(User u)
        {
            #region - code - 

            var tst = new CtrlCheckNewSocial_ID(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

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
            var tst = Setup(null);

            var ret = tst.Post(new DtoCheckNewSocial_ID
            {
                sID = "6666666666",
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void UserExists()
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

            var ret = tst.Post(new DtoCheckNewSocial_ID
            {
                sID = "90511603053",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void UserExists_not_tokenized()
        {
            var tst = Setup(new User
            {
                id = 1,
                bActive = true,
                bAdmin = false,
                bTokenized = false,
                dtCreation = DateTime.Now,
                stSocialID = "90511603053",
                stToken = "",
                dtLastLogin = DateTime.Now,
                stEmail = "rodrigo.groff@gmail.com",
                stName = "rodrigo",
                stPassword = "rodrigo"
            });

            var ret = tst.Post(new DtoCheckNewSocial_ID
            {
                sID = "90511603053",
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }
    }
}
