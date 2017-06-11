using DataModel;
using DevKit.Web.Controllers;
using System.Linq;
using System.Threading;

namespace DevKit.Web.Schedulers
{
    public class CacheControl
    {
        public CacheClean cleaner = new CacheClean();

        public long lastId = 0;

        public void Run()
        {
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