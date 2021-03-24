using Master.Data.Const;
using Master.Data.Domains;
using Master.Data.Domains.Associado;
using Master.Data.Domains.User;
using Master.Infra;
using Master.Repository;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Master.Service
{
    public class SrvML_AssociadoLimites : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
            },
        };

        public string getLanguage(int? indexLanguage, int messageIndex)
        {
            if (indexLanguage == null)
                indexLanguage = 0;

            return multi_language[(int)indexLanguage][messageIndex];
        }

        #endregion
    }

    public class SrvAssociadoLimitesV1 : SrvML_Authenticate
    {
        public ICartaoDapperRepository cartaoRepository;
        public IEmpresaDapperRepository empresaRepository;
        public IParcelaDapperRepository parcelaDapperRepository;
        public ILogTransacaoDapperRepository logTransacaoDapperRepository;

        public bool Exec ( LocalNetwork network, DtoAuthenticatedUser au, ref DtoAssociadoLimites dto ) 
        {
            try
            {
                using (var db = GetConnection(network))
                {
                    if (!_disableCache)
                    {
                        tagCache = "limites" + au.empresa + au.matricula;
                        var str = GetCachedData(tagCache, null, 1);
                        if (str != null)
                        {
                            dto = JsonSerializer.Deserialize<DtoAssociadoLimites>(str);
                            return true;
                        }
                    }
                    
                    var t_associado = cartaoRepository.GetCartao(db, Convert.ToInt64(au._id));
                    var t_empresa = empresaRepository.GetEmpresa(db, t_associado.fkEmpresa);

                    long    dispMensal = 0,
                            dispTotal = 0,
                            vrUtilizadoAtual = 0;

                    new SrvAssociadoSaldo ( db, 
                                            cartaoRepository, 
                                            parcelaDapperRepository,
                                            logTransacaoDapperRepository,
                                            t_associado, 
                                            ref dispMensal, 
                                            ref dispTotal, 
                                            ref vrUtilizadoAtual );


                    long varTotMensal = (long)t_associado.vrLimiteMensal + (long)t_associado.vrCotaExtra;

                    dto.limiteCartao = "R$ " + money(varTotMensal);
                    dto.parcelas = t_empresa.nuParcelas.ToString();
                    dto.limiteMensalDisp = "R$ " + money(dispMensal);
                    dto.cotaExtra = "R$ " + money ((long)t_associado.vrCotaExtra);
                    dto.melhorDia = t_empresa.nuDiaFech.ToString().PadLeft(2, '0');
                    dto.mensalUtilizado = "R$ " + money(vrUtilizadoAtual);

                    var dt = DateTime.Now;

                    if (DateTime.Now.Day >= t_empresa.nuDiaFech)
                        dt = dt.AddMonths(1);

                    dto.mesVigente = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year;

                    dto.pct = (dispMensal * 100 / varTotMensal).ToString();

                    if (!_disableCache)
                    {
                        UpdateCachedData(tagCache, JsonSerializer.Serialize(dto), null, 1);
                    }                    
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = new DtoServiceError
                {
                    message = getLanguage (null, 0),
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }        
    }
}
