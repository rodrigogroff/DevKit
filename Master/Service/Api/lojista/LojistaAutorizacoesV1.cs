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
    public class LojistaAutorizacoesV1 : BaseService
    {
        public LojistaAutorizacoesV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref LojistaAutorizacoesDTO dto)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var lst = repository.ObterSolicitacoesLista(db, Convert.ToInt64(au._id));

                    dto.results = new System.Collections.Generic.List<LojistaAutorizacaoDTO>();

                    var mon = new money();

                    foreach (var item in lst)
                    {
                        var cart = repository.ObterCartao(db, item.fkCartao.ToString());

                        var stSit = "Pendente";

                        if (item.tgAberto == false && item.dtConf == null)
                            stSit = "Cancelada";

                        if (item.fkLogTrans != null)
                        {
                            var logTrans = repository.ObterLogTransacao(db, (long)item.fkLogTrans);

                            switch (logTrans.tg_confirmada.ToString())
                            {
                                case TipoConfirmacao.Confirmada: stSit = "Confirmada / Nsu " + logTrans.nu_nsu; break;
                                case TipoConfirmacao.Cancelada: stSit = "Cancelada "; break;
                                case TipoConfirmacao.Erro: stSit = "Erro " + logTrans.st_msg_transacao; break;
                            }
                        }                       
                        else
                        {
                            stSit = "Erro: " + item.stErro;
                        }                            

                        var x = new LojistaAutorizacaoDTO
                        {
                            valor = mon.setMoneyFormat((long)item.vrValor),
                            dt = Convert.ToDateTime(item.dtSolic).ToString("dd/MM/yyyy HH:mm"),
                            cartao = cart.st_empresa + "." + cart.st_matricula,
                            parcs = item.nuParcelas.ToString(),
                            id = item.id,
                            sit = stSit,
                        };

                        dto.results.Add(x);
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
