using System.Collections.Generic;

namespace Entities.Api.Product
{
    public class DtoProductListingResult
    {
        public int totalRecords { get; set; }
        public List<DtoProduct> results { get; set; }
    }
}
