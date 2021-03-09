using Entities.Api;
using Entities.Api.Admin;
using Master.Repository;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Master.Service
{
    public class SrvML_BrandListing : SrvBaseService
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

    public class SrvBrandListingV1 : SrvML_ProductListing
    {
        IDapperAdminRepository adminRepository;

        public SrvBrandListingV1(IDapperAdminRepository _adminRepository, IMemoryCache cache) 
        {
            adminRepository = _adminRepository;
            _cache = cache;
        }

        bool ValidadeRequest(DtoBrandListing dto)
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

            if (dto.orderBy == null || dto.orderBy > 2 || dto.orderBy < 1 )
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            return true;

            #endregion
        }
        
        public string GetCacheTag(DtoBrandListing request)
        {
            return "BrandListing_" + (request.tag ?? "") + "_" + "_" + request.page + "_" + request.pageSize + "_" + request.orderBy;
        }

        public bool Exec ( LocalNetwork network, DtoBrandListing request, ref DtoBrandListingResult ret )
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
                            ret = JsonConvert.DeserializeObject<DtoBrandListingResult>(result);
                            return true;
                        }
                    }

                    int total = 0;
                    var lst = adminRepository.GetBrands ( db, 
                                                          request.tag,                                                         
                                                          (int)request.page,
                                                          (int)request.pageSize,
                                                          (int)request.orderBy, 
                                                          ref total);

                    ret.totalRecords = total;
                    ret.results = new List<DtoBrand>();

                    for (int i = 0; i < lst.Count; i++)
                    {
                        var item = lst[i];
                        ret.results.Add(new DtoBrand
                        {
                            id = item.id,
                            name = item.stName,
                        });
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
