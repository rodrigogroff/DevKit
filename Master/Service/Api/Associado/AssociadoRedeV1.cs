using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Master.Repository;

using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class AssociadoRedeV1 : BaseService
    {
        public AssociadoRedeV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoRede dto, string pesquisa)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var associadoPrincipal = repository.ObterCartao(db, au._id);
                    var empresa = repository.ObterEmpresa(db, associadoPrincipal.st_empresa);

                    if (!string.IsNullOrEmpty(pesquisa))
                        pesquisa = '%' + pesquisa + "%";

                    var lst = repository.ObterListaLojas(db, (long)empresa.i_unique, pesquisa);

                    foreach (var item in lst)
                    {
                        dto.resultados.Add(new LojaRede
                        {
                            nomeLoja = item.st_nome,
                            end = item.st_endereco + " " + item.st_cidade + " " + item.nu_telefone + " " + item.st_telCelular
                        });
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = new ServiceError
                {
                    message = _defaultError,
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }
    }
}
