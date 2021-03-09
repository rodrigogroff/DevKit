using Entities.Api.User;
using Master;
using Master.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class UTstPasswordRecovery : UTstUserBase
    {
        [TestMethod]
        public void OK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();
            var srv = new SrvPasswordRecoveryV1(repo)
            {
                _sendEmail = false
            };

            if (!srv.Exec(network, new DtoPasswordRecovery
            {
                email = new UTstUserBase().Base_Ut_user.stEmail,
                _language = 0
            }))
                Assert.Fail();
            
            #endregion
        }

        [TestMethod]
        public void EmailNotExists()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();
            var srv = new SrvPasswordRecoveryV1(repo)
            {
                _sendEmail = false
            };

            if (srv.Exec(network, new DtoPasswordRecovery
            {
                email = "test1@gmail.com.brrrrrrrrrrrrrrrrrrr",
                _language = 0
            }))
                Assert.Fail();

            #endregion
        }


    }
}
