using DataModel;
using DevKit.Web.Controllers;
using System.Linq;
using System.Threading;
using System.Web;

namespace DevKit.Web.Services
{
    public class CacheControlService
    {
        public void Run(HttpApplicationState _app)
        {
            var cleaner = new CacheClean()
            {
                myApplication = _app
            };

            long lastId = 0;

            while (true)
            {
                using (var db = new DevKitDB())
                {
                    if (lastId == 0)
                    {
                        var cc = (from e in db.CacheControl
                                  orderby e.id descending
                                  select e).
                                  FirstOrDefault();

                        if (cc != null)
                        {
                            lastId = cc.id;

                            cleaner.Clean(cc.stEntity, cc.fkTarget);
                        }                            
                    }
                    else
                    {
                        var lst = (from e in db.CacheControl
                                   where e.id > lastId
                                   orderby e.id
                                   select e).
                                   ToList();

                        foreach (var item in lst)
                        {
                            cleaner.Clean(item.stEntity, item.fkTarget);
                            lastId = item.id;
                        }
                    }                    
                }

                Thread.Sleep(1000);
            }
        }
    }
}
