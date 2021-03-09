
namespace Entities.Api.Product
{
    public class DtoProductListing : DtoBase
    {
        public string tag { get; set; }
        public long? category { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public int? orderBy { get; set; }
    }
}
