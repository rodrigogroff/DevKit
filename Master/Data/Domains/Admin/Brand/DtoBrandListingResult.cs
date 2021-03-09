using System.Collections.Generic;

namespace Entities.Api.Admin
{
    public class DtoBrandListingResult
    {
        public int totalRecords { get; set; }
        public List<DtoBrand> results { get; set; }
    }
}
