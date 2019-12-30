using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class LojistaSolicitacaoV1 : BaseService
    {
        public LojistaSolicitacaoV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoSolicitacao dto)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var solic = repository.ObterSolicLojistaEmAberto(db, Convert.ToInt64(au._id));

                    if (solic != null)
                    {
                        var cart = repository.ObterCartao(db, solic.fkCartao.ToString());
                        var prop = repository.ObterProprietario(db, (long)cart.fk_dadosProprietario);

                        dto.id = solic.id;
                        dto.nuParcelas = solic.nuParcelas.ToString();
                        dto.stValor = new money().setMoneyFormat((long)solic.vrValor);
                        dto.stCartao = cart.st_empresa + "." + cart.st_matricula + " / " + prop.st_nome;
                        dto.dtSolic = Convert.ToDateTime(solic.dtSolic).ToString("dd/MM/yyyy HH:mm");
                    }
                    else
                        dto.id = 0;

                    return true;
                }
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
