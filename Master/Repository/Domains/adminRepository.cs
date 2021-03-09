using Dapper;
using Master.Infra.Entity.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface IDapperAdminRepository
    {
        List<Brand> GetBrands(NpgsqlConnection db, string sTag, int page, int pageSize, int orderBy, ref int total );
        Brand GetBrand(NpgsqlConnection db, long id);
        Category GetCategory(NpgsqlConnection db, long id);
        Product GetProduct(NpgsqlConnection db, long id);

        void InsertBrand(NpgsqlConnection db, Brand obj);
        void InsertCategory(NpgsqlConnection db, Category obj);
    }

    public class DapperAdminRepository : IDapperAdminRepository
    {
        public List<Brand> GetBrands(NpgsqlConnection db, string sTag, int page, int pageSize, int orderBy, ref int total)
        {
            var query = "select * from \"Brand\" " +
                            (!string.IsNullOrEmpty(sTag) ? " where \"stName\" like '%" + sTag + "%' " : "");

            switch (orderBy)
            {
                default: case 1: query += " order by \"stName\" asc"; break;
                case 2: query += " order by \"stName\" desc"; break;
            }

            total = db.Query(query).Count();

            query += " offset " + (page * pageSize) + " limit " + pageSize;

            return db.Query<Brand>(query).ToList();
        }

        public void InsertBrand(NpgsqlConnection db, Brand obj)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Brand\"(\"stName\") " +
                "VALUES(@stName);", db))
            {
                cmd.Parameters.AddWithValue("stName", ((object)obj.stName) ?? DBNull.Value);                
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertCategory(NpgsqlConnection db, Category obj)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Category\"(\"stName\",\"fkMainCategory\",\"fkCountry\") " +
                "VALUES(@stName,@fkMainCategory,@fkCountry);", db))
            {
                cmd.Parameters.AddWithValue("stName", ((object)obj.stName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkMainCategory", ((object)obj.fkMainCategory) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkCountry", ((object)obj.fkCountry) ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        Brand IDapperAdminRepository.GetBrand(NpgsqlConnection db, long id)
        {
            return db.QueryFirstOrDefault<Brand>("select * from \"Brand\" where id = " + id);            
        }

        Category IDapperAdminRepository.GetCategory(NpgsqlConnection db, long id)
        {
            return db.QueryFirstOrDefault<Category>("select * from \"Category\" where id = " + id);
        }

        Product IDapperAdminRepository.GetProduct(NpgsqlConnection db, long id)
        {
            return db.QueryFirstOrDefault<Product>("select * from \"Product\" where id = " + id);
        }
    }
}
