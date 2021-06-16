using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmissoraBaixaCCController : ApiControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("baixacc")]
        public HttpResponseMessage Xdad()
        {
            var httpRequest = HttpContext.Current.Request;

            if (HttpContext.Current.Request.Files.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var binReader = new BinaryReader(postedFile.InputStream);
                    byte[] byteArray = binReader.ReadBytes(postedFile.ContentLength);

                    string result = System.Text.Encoding.UTF8.GetString(byteArray);

                    foreach (var line in result.Split ('\n'))
                    {

                    }
                }

                return Request.CreateResponse(HttpStatusCode.Created);
            }
        }
    }
}
