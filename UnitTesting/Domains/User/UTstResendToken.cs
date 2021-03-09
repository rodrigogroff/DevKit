using Entities.Api.User;
using Master;
using Master.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class UTstResendToken : UTstUserBase
    {
        [TestMethod]
        public void OK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();
            var srv = new SrvResendTokenV1(repo)
            {
                _sendEmail = false
            };

            if (!srv.Exec(network, new DtoResendToken
            {
                sID = "1",
                _language = 0
            }))
                Assert.Fail();
            
            #endregion
        }

        [TestMethod]
        public void SidNotExists()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();
            var srv = new SrvResendTokenV1(repo)
            {
                _sendEmail = true
            };

            if (srv.Exec(network, new DtoResendToken
            {
                sID = "2",
                _language = 0
            }))
                Assert.Fail();

            #endregion
        }

        [TestMethod]
        public void SidEmpty()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();
            var srv = new SrvResendTokenV1(repo)
            {
                _sendEmail = true
            };

            if (srv.Exec(network, new DtoResendToken
            {
                sID = null,
                _language = 0
            }))
                Assert.Fail();

            #endregion
        }
    }
}
