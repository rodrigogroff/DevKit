
namespace Entities.Api.Admin
{
    public class DtoBrandListing : DtoBase
    {
        public string tag { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public int? orderBy { get; set; }
    }
}
