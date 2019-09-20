using Dapper;
using Entities.Database;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Repository
{
    public interface IDapperRepository
    {
        T_Cartao ObterCartao(SqlConnection db, string empresa, string matricula, string titularidade);
        T_Cartao ObterCartao(SqlConnection db, string id);
        T_Proprietario ObterProprietario(SqlConnection db, long id);
        List<T_Cartao> ObterListaCartao(SqlConnection db, string empresa, string matricula);
        T_Empresa ObterEmpresa(SqlConnection db, string empresa);
        List<T_Parcelas> ObterListaParcelaDeListaCartaoIgual(SqlConnection db, List<long> lstFkCartao, int nuParcela);
        List<T_Parcelas> ObterListaParcelaDeListaCartaoSuperior(SqlConnection db, List<long> lstFkCartao, int nuParcela);
        LOG_Transacoes ObterLogTransacao(SqlConnection db, long id);
    }

    public class DapperRepository : IDapperRepository
    {
        public T_Empresa ObterEmpresa(SqlConnection db, string empresa)
        {
            return db.Query<T_Empresa>(@"select * from [T_Empresa] (nolock) 
                                        where   st_empresa = @empresa ", new { empresa }).FirstOrDefault();
        }
        public T_Cartao ObterCartao(SqlConnection db, string id)
        {
            return db.Query<T_Cartao>(@"select * from [T_Cartao] (nolock) 
                                        where   i_unique = @id", new { id }).FirstOrDefault();
        }

        public T_Proprietario ObterProprietario(SqlConnection db, long id)
        {
            return db.Query<T_Proprietario>(@"select * from [T_Proprietario] (nolock) 
                                        where   i_unique = @id ", new { id }).FirstOrDefault();
        }

        public List<T_Cartao> ObterListaCartao(SqlConnection db, string empresa, string matricula)
        {
            return db.Query<T_Cartao>(@"select * from [T_Cartao] (nolock) 
                                        where   st_empresa = @empresa and
                                                st_matricula = @matricula ", new { empresa, matricula }).ToList();
        }

        public T_Cartao ObterCartao(SqlConnection db, string empresa, string matricula, string titularidade)
        {
            return db.Query<T_Cartao>(@"select * from [T_Cartao] (nolock) 
                                        where   st_empresa = @empresa and
                                                st_matricula = @matricula and 
                                                st_titularidade = @titularidade", new { empresa, matricula, titularidade }).FirstOrDefault();
        }
        
        public List<T_Parcelas> ObterListaParcelaDeListaCartaoIgual(SqlConnection db, List<long> lstFkCartao, int nuParcela)
        {
            return db.Query<T_Parcelas>(@"select * from [T_Parcelas] (nolock) 
                                        where   nu_parcela = @nuParcela and
                                                fk_cartao in @lstFkCartao", new { nuParcela, lstFkCartao }).ToList();
        }

        public List<T_Parcelas> ObterListaParcelaDeListaCartaoSuperior(SqlConnection db, List<long> lstFkCartao, int nuParcela)
        {
            return db.Query<T_Parcelas>(@"select * from [T_Parcelas] (nolock) 
                                        where   nu_parcela >= @nuParcela and
                                                fk_cartao in @lstFkCartao", new { nuParcela, lstFkCartao }).ToList();
        }

        public LOG_Transacoes ObterLogTransacao(SqlConnection db, long id)
        {
            return db.Query<LOG_Transacoes>(@"select * from [LOG_Transacoes] (nolock) 
                                        where   i_unique = @id ", new { id }).FirstOrDefault();
        }
    }
}
