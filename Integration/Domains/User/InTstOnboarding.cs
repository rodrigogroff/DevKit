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
    public class InTstOnboarding : TstBaseIntegration
    {
        CtrlOnboarding Setup(User u)
        {
            #region - code - 

            var tst = new CtrlOnboarding(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

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

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {                
                sEmail = "rodrigo.groff@gmail.com",
                sID = "90511603053",
                sName = "rodrigo",
                sPass = "rodrigo",
                _language = 0,
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void NOK_Email()
        {
            var tst = Setup(null);

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {
                //sEmail = "rodrigo.groff@gmail.com",
                sID = "90511603053",
                sName = "rodrigo",
                sPass = "rodrigo",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void NOK_SID()
        {
            var tst = Setup(null);

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {
                sEmail = "rodrigo.groff@gmail.com",
                //sID = "90511603053",
                sName = "rodrigo",
                sPass = "rodrigo",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void NOK_Name()
        {
            var tst = Setup(null);

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {
                sEmail = "rodrigo.groff@gmail.com",
                sID = "90511603053",
                //sName = "rodrigo",
                sPass = "rodrigo",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void NOK_Pass()
        {
            var tst = Setup(null);

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {
                sEmail = "rodrigo.groff@gmail.com",
                sID = "90511603053",
                sName = "rodrigo",
                //sPass = "rodrigo",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
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

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {                
                sEmail = "rodrigo.groff@gmail.com",
                sID = "90511603053",
                sName = "rodrigo",
                sPass = "rodrigo",
                _language = 0,
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void UserExists_but_not_tokenized()
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

            tst._sendEmail = false;

            var ret = tst.Post(new DtoOnboarding
            {
                _language = 0,
                sEmail = "rodrigo.groff@gmail.com",
                sID = "90511603053",
                sName = "rodrigo",
                sPass = "rodrigo"
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }
    }
}
