using Dapper;
using Entities.Database;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Repository
{
    public interface IDapperRepository
    {
        T_Cartao ObterCartao(SqlConnection db, string empresa, string matricula, string titularidade);

        T_Empresa ObterEmpresa(SqlConnection db, string empresa);

        T_Proprietario ObterProprietario(SqlConnection db, long id);
    }

    public class DapperRepository : IDapperRepository
    {
        public T_Cartao ObterCartao(SqlConnection db, string empresa, string matricula, string titularidade)
        {
            return db.Query<T_Cartao>(@"select * from [T_Cartao] (nolock) 
                                        where   st_empresa = @empresa and
                                                st_matricula = @matricula and 
                                                st_titularidade = @titularidade", new { empresa, matricula, titularidade }).FirstOrDefault();
        }

        public T_Empresa ObterEmpresa(SqlConnection db, string empresa)
        {
            return db.Query<T_Empresa>(@"select * from [T_Empresa] (nolock) 
                                        where   st_empresa = @empresa ", new { empresa }).FirstOrDefault();
        }

        public T_Proprietario ObterProprietario(SqlConnection db, long id)
        {
            return db.Query<T_Proprietario>(@"select * from [T_Proprietario] (nolock) 
                                        where   i_unique = @id ", new { id }).FirstOrDefault();
        }
    }
}
