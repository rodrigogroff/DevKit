
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean(string target, long? id)
        {
            switch (target)
            {
                case CacheTags.T_Loja:
                    if (id != null)
                    {
                        StoreCache(CacheTags.T_Loja, id, null);

                    }
                    //CleanCacheReport(CacheTags.UserReport);
                    //CleanCacheReport(CacheTags.UserComboReport);
                    break;

                case CacheTags.associado:
                    if (id != null)
                        StoreCache(CacheTags.associado, id, null);
                    break;
            }
        }
    }
}
