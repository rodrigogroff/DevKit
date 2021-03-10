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
    public class InTstAuthenticate : TstBaseIntegration
    {
        /*
        CtrlAuthenticate Setup(User u)
        {
            #region - code - 

            u.compile();

            var tst = new CtrlAuthenticate(null,null) { network = new Master.LocalNetwork { sqlServer = connStr } };

            SetupDatabase();

            using (var db = new NpgsqlConnection(connStr))
            {
                db.Open();
                //new DapperRepository().InsertUser(db, u);
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
                stPassword = "rodrigo",
            });

            var ret = tst.Post(new DtoLoginInformation
            {
                email = "rodrigo.groff@gmail.com",
                password = "rodrigo"
            });

            if (ret.ToString().Contains("BadRequest"))
                Assert.Fail();
        }

        [TestMethod]
        public void NotExist()
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

            var ret = tst.Post(new DtoLoginInformation
            {
                email = "teste",
                password = "senha"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
            
        }

        [TestMethod]
        public void NonActive()
        {
            
            var tst = Setup(new User
            {
                id = 1,
                bActive = false,
                bAdmin = false,
                bTokenized = true,
                dtCreation = DateTime.Now,
                stSocialID = "90511603053",
                stToken = "",
                dtLastLogin = DateTime.Now,
                stEmail = "marco.ferreira@vortigo.com.br",
                stName = "marco",
                stPassword = "102030"
            });

            var ret = tst.Post(new DtoLoginInformation
            {
                email = "marco.ferreira@vortigo.com.br",
                password = "102030"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
            
        }

        [TestMethod]
        public void NonTokenized()
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
                stEmail = "marco.ferreira@vortigo.com.br",
                stName = "marco",
                stPassword = "102030"
            });

            var ret = tst.Post(new DtoLoginInformation
            {
                email = "marco.ferreira@vortigo.com.br",
                password = "102030"
            });

            if (!ret.ToString().Contains("BadRequest"))
                Assert.Fail();
            
        }
        */
    }
}
