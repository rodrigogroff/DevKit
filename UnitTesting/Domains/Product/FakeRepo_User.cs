using Master.Infra.Entity.Database;
using Master.Repository;
using Npgsql;
using System.Collections.Generic;

namespace UnitTesting
{
    public class FakeRepo_Product : IDapperProductRepository
    {
        public List<Product> GetProducts(NpgsqlConnection db, string sTag, long? nuCategory, int page, int pageSize, int orderBy, ref int total )
        {
            if (sTag == "Livro")
            {
                return new List<Product>()
                {
                    new Product
                    {
                        id = 1,
                        fkCategory = 1,
                        fkBrand = 1,
                        fkCountry = 1,
                        stName = "Livro",
                        nuPrice = 123456,
                        stDesc = "Livro Desc"
                    },
                };
            }

            return null;
        }

        public void InsertProduct(NpgsqlConnection db, Product obj)
        {
            
        }

        public void InsertProductCatalog(NpgsqlConnection db, ProductCatalog obj)
        {

        }

        public void InsertProductCatalogLink(NpgsqlConnection db, ProductCatalogLink obj)
        {
        }
    }
}
