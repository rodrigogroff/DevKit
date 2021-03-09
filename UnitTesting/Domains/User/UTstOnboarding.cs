using Entities.Api.User;
using Master;
using Master.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class UTstOnboarding : UTstUserBase
    {
        [TestMethod]
        public void OK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();

            var srv = new SrvOnboardingV1(repo) {
                _sendEmail = false
            };

            if (!srv.Exec(network, new DtoOnboarding
            {
                sEmail = "1234@123.com",
                sID = "102030405060",
                sName = "One",
                sPass = "Pass",
                _language = 0
            }))
                Assert.Fail();

            #endregion
        }
    }
}
