using Entities.Api.User;
using Master;
using Master.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class UTstCheckNewSocial_ID : UTstUserBase
    {
        [TestMethod]
        public void OK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();

            var srv = new SrvCheckNewSocial_IDV1(repo);

            if (!srv.Exec(network, new DtoCheckNewSocial_ID
            {
                sID = "9999",
                _language = 0
            }))
                Assert.Fail();
            
            #endregion
        }

        [TestMethod]
        public void UserCantExist()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repo = new FakeRepo_User();

            var srv = new SrvCheckNewSocial_IDV1(repo);

            if (srv.Exec(network, new DtoCheckNewSocial_ID
            {
                sID = "1",
                _language = 0
            }))
                Assert.Fail();

            #endregion
        }
    }
}
