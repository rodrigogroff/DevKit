
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean(string target, long? id)
        {
            switch (target)
            {
                case CacheTags.T_Terminal:
                    if (id != null)
                    {
                        StoreCache(CacheTags.T_Terminal, id, null);

                    }
                    //CleanCacheReport(CacheTags.UserReport);
                    //CleanCacheReport(CacheTags.UserComboReport);
                    break;

                case CacheTags.T_Cartao:
                    if (id != null)
                        StoreCache(CacheTags.T_Cartao, id, null);
                    break;

                case CacheTags.associado:
                    if (id != null)
                        StoreCache(CacheTags.associado, id, null);
                    break;
            }
        }
    }
}
