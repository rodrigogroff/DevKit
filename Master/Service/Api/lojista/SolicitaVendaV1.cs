using Entities.Api;
using Entities.Api.Login;
using Entities.Api.Lojista;
using Entities.Database;
using Master.Repository;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class SolicitaVendaV1 : BaseService
    {
        public SolicitaVendaV1(IDapperRepository repository) : base (repository) { }

        bool ValidadeRequest(ReqSolicitacaoVenda req)
        {
            #region - code - 

            if (string.IsNullOrEmpty(req.empresa))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.matricula))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.codAcesso))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.venc))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.valor))
            {
                Error = new ServiceError { message = "Valor inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.parcelas))
            {
                Error = new ServiceError { message = "Num. Parcelas inválido" };
                return false;
            }

            return true;

            #endregion
        }

        public bool Exec ( LocalNetwork network, long fkTerminal, ReqSolicitacaoVenda req )
        {
            try
            {
                if (!ValidadeRequest(req))
                    return false;

                req.empresa = req.empresa.PadLeft(6, '0');
                req.matricula = req.matricula.PadLeft(6, '0');
                
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var terminal = repository.ObterTerminal(db, fkTerminal);

                    var associadoPrincipal = repository.ObterCartao(db, req.empresa, req.matricula, "01");

                    if (associadoPrincipal == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "associadoPrincipal == null"
                        };

                        return false;
                    }

                    if (associadoPrincipal.st_venctoCartao != req.venc)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "associadoPrincipal.st_venctoCartao != login.venc"
                        };

                        return false;
                    }

                    var tEmp = repository.ObterEmpresa(db, req.empresa);

                    if (tEmp == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "tEmp == null"
                        };

                        return false;
                    }

                    var dadosProprietario = repository.ObterProprietario(db, (int) associadoPrincipal.fk_dadosProprietario);

                    if (dadosProprietario == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "dadosProprietario == null"
                        };

                        return false;
                    }

                    var codAcessoCalc = new CodigoAcesso().Obter ( req.empresa,
                                                                   req.matricula,
                                                                   associadoPrincipal.st_titularidade,
                                                                   associadoPrincipal.nu_viaCartao,
                                                                   dadosProprietario.st_cpf );

                    if (codAcessoCalc != req.codAcesso)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "dadosProprietario == null"
                        };

                        return false;
                    }

                    // -----------------------
                    // salvar no banco!!
                    // -----------------------

                    var solic = new SolicitacaoVenda
                    { 
                        dtSolic = DateTime.Now,
                        dtConf = null,
                        tgAberto = true,
                        fkCartao = (long) associadoPrincipal.i_unique,
                        fkLoja = terminal.fk_loja,                        
                        fkTerminal = fkTerminal,
                        vrValor = Convert.ToInt32(req.valor.Replace(".","").Replace(",","")),
                        nuParcelas = Convert.ToInt32(req.parcelas),
                        stParcelas = req.parcelas_str
                    };

                    repository.InserirSolicitacaoVenda(db, solic);
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
