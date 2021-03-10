using Entities.Api.User;
using Master;
using Master.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class UTstAuthenticate : UTstUserBase
    {
        /*
        [TestMethod]
        public void OK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };         
            var auth = new DtoAuthenticatedUser();
            var repo = new FakeRepo_User();

            var tu_obj = new DtoLoginInformation
            {
                email = Base_Ut_user.stEmail,
                password = Base_Ut_user.stPassword
            };

            var srv = new SrvAuthenticateV1(repo);

            if (!srv.Exec(network, tu_obj, ref auth))
                Assert.Fail();

            if (auth.login != Base_Ut_user.stName) Assert.Fail();
            if (auth.email != Base_Ut_user.stEmail) Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void PassNOK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var auth = new DtoAuthenticatedUser();
            var repo = new FakeRepo_User();

            var tu_obj = new DtoLoginInformation
            {
                email = Base_Ut_user.stEmail,
                password = Base_Ut_user.stPassword + "...."
            };

            var srv = new SrvAuthenticateV1(repo);

            if (srv.Exec(network, tu_obj, ref auth))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void EmailNOK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var auth = new DtoAuthenticatedUser();
            var repo = new FakeRepo_User();

            var tu_obj = new DtoLoginInformation
            {
                email = Base_Ut_user.stEmail + "....",
                password = Base_Ut_user.stPassword
            };

            var srv = new SrvAuthenticateV1(repo);

            if (srv.Exec(network, tu_obj, ref auth))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void NotActive()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var auth = new DtoAuthenticatedUser();
            var repo = new FakeRepo_User();

            var tu_obj = new DtoLoginInformation
            {
                email = Base_Ut_user_not_active.stEmail,
                password = Base_Ut_user_not_active.stPassword
            };

            var srv = new SrvAuthenticateV1(repo);

            if (srv.Exec(network, tu_obj, ref auth))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void NotTokenized()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var auth = new DtoAuthenticatedUser();
            var repo = new FakeRepo_User();

            var tu_obj = new DtoLoginInformation
            {
                email = Base_Ut_user_not_tokenized.stEmail,
                password = Base_Ut_user_not_tokenized.stPassword
            };

            var srv = new SrvAuthenticateV1(repo);

            if (srv.Exec(network, tu_obj, ref auth))
                Assert.Fail();

            #endregion
        }

        */
    }
}
