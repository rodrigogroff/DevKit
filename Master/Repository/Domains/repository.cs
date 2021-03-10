using Dapper;
using Master.Infra.Entity.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface IDapperRepository
    {
        Cartao GetCartao(NpgsqlConnection db, long? id);

        Cartao GetCartao(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula, long? fkTitularidade);

        Empresa GetEmpresa(NpgsqlConnection db, long? id);

        Empresa GetEmpresaNum(NpgsqlConnection db, long? nuEmpresa);
    }

    public class DapperRepository : IDapperRepository
    {
        public Cartao GetCartao(NpgsqlConnection db, long? fkEmpresa, long? nuMatricula, long? nuTitularidade)
        {
            return db.QueryFirstOrDefault<Cartao>("select * from \"Cartao\" where \"fkEmpresa\"=" + fkEmpresa + " and \"nuMatricula\"=" + nuMatricula + " and \"nuTitularidade\"=" + nuTitularidade);
        }

        Cartao IDapperRepository.GetCartao(NpgsqlConnection db, long? id)
        {
            return db.QueryFirstOrDefault<Cartao>("select * from \"Cartao\" where \"id\" = " + id);
        }

        public Empresa GetEmpresa(NpgsqlConnection db, long? id)
        {
            return db.QueryFirstOrDefault<Empresa>("select * from \"Empresa\" where \"id\" = " + id);
        }

        public Empresa GetEmpresaNum(NpgsqlConnection db, long? nuEmpresa)
        {
            return db.QueryFirstOrDefault<Empresa>("select * from \"Empresa\" where \"nuEmpresa\" = " + nuEmpresa);
        }        
    }
}
