using Master;
using Master.Service;
using Entities.Api.Product;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class UTstProductListing : UTstUserBase
    {
        [TestMethod]
        public void OK()
        {
            #region - code - 

            var network = new LocalNetwork() { sqlServer = connStr };
            var repoProduct = new FakeRepo_Product();
            var repoAdmin = new FakeRepo_Admin();

            var srv = new SrvProductListingV1(repoProduct, repoAdmin, null) {
                _sendEmail = false
            };

            DtoProductListingResult ret = new DtoProductListingResult();

            if (!srv.Exec(network, new DtoProductListing
            {
                tag = "Livro",
                category = 1,               
                page = 0,
                pageSize = 99,
                orderBy = 1,
                _language = 0
            }, ref ret))
                Assert.Fail();

            #endregion
        }
    }
}
