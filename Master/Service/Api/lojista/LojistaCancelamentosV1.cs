using Entities.Api;
using Entities.Api.Configuration;
using Entities.Api.Lojista;
using Entities.Enums;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class LojistaCancelamentosV1 : BaseService
    {
        public LojistaCancelamentosV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref LojistaCancelamentosDTO dto)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var lst = repository.ObterSolicitacoesListaHoje(db, Convert.ToInt64(au._id));

                    dto.results = new System.Collections.Generic.List<LojistaCancelamentoDTO>();

                    var mon = new money();

                    foreach (var item in lst)
                    {
                        var cart = repository.ObterCartao(db, item.fkCartao.ToString());

                        if (item.tgAberto == false && item.dtConf == null)
                            continue;

                        if (item.fkLogTrans != null)
                        {
                            var logTrans = repository.ObterLogTransacao(db, (long)item.fkLogTrans);

                            switch (logTrans.tg_confirmada.ToString())
                            {
                                case TipoConfirmacao.Confirmada: break;
                                default: continue;                                
                            }
                        }                       
                        else
                            continue;

                        dto.results.Add(new LojistaCancelamentoDTO
                        {
                            valor = mon.setMoneyFormat((long)item.vrValor),
                            dt = Convert.ToDateTime(item.dtSolic).ToString("dd/MM/yyyy HH:mm"),
                            cartao = cart.st_empresa + "." + cart.st_matricula,
                            parcs = item.nuParcelas.ToString(),
                            id = item.id,
                        });
                    }
                    
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
