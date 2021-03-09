using Dapper;
using Master.Infra.Entity.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface IDapperProductRepository
    {
        List<Product> GetProducts(NpgsqlConnection db, string sTag, long? nuCategory, int page, int pageSize, int orderBy, ref int total);
        void InsertProduct(NpgsqlConnection db, Product obj);
        void InsertProductCatalog(NpgsqlConnection db, ProductCatalog obj);
        void InsertProductCatalogLink(NpgsqlConnection db, ProductCatalogLink obj);
    }

    public class DapperProductRepository : IDapperProductRepository
    {
        public void InsertProduct(NpgsqlConnection db, Product obj)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"Product\"(\"stName\",\"fkCountry\",\"fkBrand\",\"fkCategory\",\"nuPrice\",\"stDesc\") " +
                "VALUES(@stName,@fkCountry,@fkBrand,@fkCategory,@nuPrice,@stDesc);", db))
            {
                cmd.Parameters.AddWithValue("stName", ((object)obj.stName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkCountry", ((object)obj.fkCountry) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkBrand", ((object)obj.fkBrand) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkCategory", ((object)obj.fkCategory) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuPrice", ((object)obj.nuPrice) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stDesc", ((object)obj.stDesc) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void InsertProductCatalog(NpgsqlConnection db, ProductCatalog obj)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"ProductCatalog\"(\"stTag\") " +
                "VALUES(@stTag);", db))
            {
                cmd.Parameters.AddWithValue("stTag", ((object)obj.stTag) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void InsertProductCatalogLink(NpgsqlConnection db, ProductCatalogLink obj)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"ProductCatalogLink\"(\"fkProduct\",\"fkProductCatalog\") " +
               "VALUES(@fkProduct,@fkProductCatalog);", db))
            {
                cmd.Parameters.AddWithValue("fkProduct", ((object)obj.fkProduct) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("fkProductCatalog", ((object)obj.fkProductCatalog) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        List<Product> IDapperProductRepository.GetProducts(NpgsqlConnection db, string sTag, long? nuCategory, int page, int pageSize, int orderBy, ref int total)
        {
            var query = "with cat as ( select * from \"ProductCatalog\" where \"stTag\" = '" + sTag + "' ) " +
                "select p.* from \"Product\" p, cat, \"ProductCatalogLink\" pcl " +
                "where cat.\"id\" = pcl.\"fkProductCatalog\" and pcl.\"fkProductCatalog\" = p.id ";

            if (nuCategory != null)
                query += " and p.\"fkCategory\" = " + nuCategory;

            switch (orderBy)
            {
                default: case 1: query += " order by p.id asc"; break;
                case 2: query += " order by p.id desc"; break;                
            }

            total = db.Query(query).Count();

            query += " offset " + (page * pageSize) + " limit " + pageSize;

            return db.Query<Product>(query).ToList();
        }
    }
}
