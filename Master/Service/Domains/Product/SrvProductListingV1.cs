using Entities.Api;
using Entities.Api.Product;
using Master.Infra.Entity.Database;
using Master.Repository;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Master.Service
{
    public class SrvML_ProductListing : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Invalid parameters", /* 1 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "Autenticación de usuario no válida", /* 1 */                
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "Parâmetros inválidos", /* 1 */                
            },
        };

        public string getLanguage(int? indexLanguage, int messageIndex)
        {
            if (indexLanguage == null)
                indexLanguage = 0;

            return multi_language[(int)indexLanguage][messageIndex];
        }

        #endregion
    }

    public class SrvProductListingV1 : SrvML_ProductListing
    {
        IDapperProductRepository repository;
        IDapperAdminRepository adminRepository;

        public SrvProductListingV1( IDapperProductRepository _repository,
                                    IDapperAdminRepository _adminRepository, 
                                    IMemoryCache cache) 
        {
            repository = _repository;
            adminRepository = _adminRepository;
            _cache = cache;
        }

        bool ValidadeRequest(DtoProductListing dto)
        {
            #region - code - 
            
            if (dto.page == null)
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            if (dto.page < 0 )
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            if (dto.pageSize == null)
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            if (dto.pageSize <= 20)
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            if (dto.orderBy == null || dto.orderBy > 2 || dto.orderBy < 1)
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            return true;

            #endregion
        }
        
        public string GetCacheTag(DtoProductListing request)
        {
            return "ProductListing_" + (request.tag ?? "") + "_" + (request.category?.ToString() ?? "") + "_" + request.page + "_" + request.pageSize + "_" + request.orderBy;
        }

        public bool Exec ( LocalNetwork network, DtoProductListing request, ref DtoProductListingResult ret )
        {
            try
            {
                if (!ValidadeRequest(request))
                    return false;

                using (var db = GetConnection(network))
                {
                    var tagCache = GetCacheTag(request);
                    var cacheOn = !string.IsNullOrEmpty(network.cacheServer);

                    if (cacheOn) {
                        var result = GetCachedData(tagCache, network.cacheServer, 1);
                        if (result != null) {
                            ret = JsonConvert.DeserializeObject<DtoProductListingResult>(result);
                            return true;
                        }
                    }

                    int total = 0;
                    var lst = repository.GetProducts(db, 
                                                         request.tag,
                                                         request.category, 
                                                         (int)request.page,
                                                         (int)request.pageSize,
                                                         (int)request.orderBy,
                                                         ref total );

                    ret.totalRecords = total;
                    ret.results = new List<DtoProduct>();

                    var threads = new List<int> { 1, 2 };
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var item = lst[i];

                        var finalProd = new DtoProduct
                        {
                            name = item.stName,
                            serial = "45445454564456546546"
                        };

                        Parallel.ForEach(threads, th =>
                        {
                            switch (th)
                            {
                                case 1: 
                                    {
                                        #region - brand -

                                        if (cacheOn)
                                        {
                                            var tagCache_brand = "Brand_" + item.fkBrand;
                                            var result = GetCachedData(tagCache_brand, network.cacheServer, 10);

                                            if (result != null)
                                            {
                                                var brandCache = JsonConvert.DeserializeObject<Brand>(result);
                                                finalProd.brand = brandCache.stName;
                                            }
                                            else
                                            {
                                                using (var db_brand = GetConnection(network))
                                                {
                                                    var brandCache = adminRepository.GetBrand(db_brand, item.fkBrand);
                                                    finalProd.brand = brandCache.stName;

                                                    var retStr = System.Text.Json.JsonSerializer.Serialize(brandCache);
                                                    UpdateCachedData(tagCache_brand, retStr, network.cacheServer, 10);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            using (var db_brand = GetConnection(network))
                                            {
                                                var brandDB = adminRepository.GetBrand(db_brand, item.fkBrand);
                                                finalProd.brand = brandDB.stName;
                                            }
                                        }

                                        #endregion

                                        break;
                                    }

                                case 2: 
                                    {
                                        #region - category -

                                        if (cacheOn)
                                        {
                                            var tagCache_Categ = "Category_" + item.fkCategory;
                                            var result = GetCachedData(tagCache_Categ, network.cacheServer, 10);

                                            if (result != null)
                                            {
                                                var brandCache = JsonConvert.DeserializeObject<Category>(result);
                                                finalProd.brand = brandCache.stName;
                                            }
                                            else
                                            {
                                                using (var db_categ = GetConnection(network))
                                                {
                                                    var categoryCache = adminRepository.GetCategory(db_categ, item.fkCategory);
                                                    finalProd.category = categoryCache.stName;

                                                    var retStr = System.Text.Json.JsonSerializer.Serialize(categoryCache);
                                                    UpdateCachedData(tagCache_Categ, retStr, network.cacheServer, 10);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            using (var db_categ = GetConnection(network))
                                            {
                                                var categoryCache = adminRepository.GetCategory(db_categ, item.fkCategory);
                                                finalProd.category = categoryCache.stName;
                                            }
                                        }

                                        #endregion

                                        break;
                                    }
                            }
                        });                        

                        ret.results.Add(finalProd);
                    }

                    if (cacheOn) {
                        var retStr = System.Text.Json.JsonSerializer.Serialize(ret);
                        UpdateCachedData ( tagCache, retStr, network.cacheServer, 1);
                    }
                }
    
                return true;
            }
            catch (Exception ex)
            {
                Error = new DtoServiceError
                {
                    message = getLanguage(0, 0),
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }        
    }
}
