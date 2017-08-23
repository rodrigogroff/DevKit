
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean(string target, long? id)
        {
            switch (target)
            {
                case CacheTags.Lojista:
                    if (id != null)
                    {
                        StoreCache(CacheTags.Lojista, id, null);
                        
                    }
                    //CleanCacheReport(CacheTags.UserReport);
                    //CleanCacheReport(CacheTags.UserComboReport);
                    break;
            }
        }
    }
}
