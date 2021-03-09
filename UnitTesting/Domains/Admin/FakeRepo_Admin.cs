using Master.Infra.Entity.Database;
using Master.Repository;
using Npgsql;
using System.Collections.Generic;

namespace UnitTesting
{
    public class FakeRepo_Admin : IDapperAdminRepository
    {
        #region - code - 

        public Brand GetBrand(NpgsqlConnection db, long id)
        {
            return new Brand
            { 
                id = 1,
                stName = "Teste"
            };
        }

        public List<Brand> GetBrands(NpgsqlConnection db, string sTag, int page, int pageSize, int orderBy, ref int total )
        {
            return null;
        }

        public Category GetCategory(NpgsqlConnection db, long id)
        {
            return new Category
            {
                id = 1,
                stName = "Teste",
                fkCountry = 1,
                fkMainCategory = 0,
            };
        }

        public Product GetProduct(NpgsqlConnection db, long id)
        {
            return null;
        }

        public void InsertBrand(NpgsqlConnection db, Brand id)
        {
            
        }

        public void InsertCategory(NpgsqlConnection db, Category obj)
        {
            
        }

        #endregion
    }
}
