using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using Microsoft.Ajax.Utilities;
using System.Web;

namespace DevKit.Web.Controllers
{
    public class EfetuaVendaController : ApiControllerBase
    {
        public string terminal,
                        empresa,
                        matricula,
                        codAcesso,
                        stVencimento,
                        strMessage,
                        retorno,
                        nsu_retorno,
                        ultima_linha,
                        senha,
                        tipoWeb;

        public long idCartao, 
                    valor, 
                    parcelas, 
                    p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12;

        public IHttpActionResult Get()
        {
            empresa = Request.GetQueryStringValue("empresa").PadLeft(6,'0');
            senha = Request.GetQueryStringValue("senha");
            matricula = Request.GetQueryStringValue("matricula").PadLeft(6, '0');
            codAcesso = Request.GetQueryStringValue("codAcesso");
            stVencimento = Request.GetQueryStringValue("stVencimento");
            idCartao = Request.GetQueryStringValue<int>("cartao");
            valor = ObtemValor(Request.GetQueryStringValue("valor"));            
            parcelas = Request.GetQueryStringValue<int>("parcelas");
            tipoWeb = Request.GetQueryStringValue("tipoWeb");

            p1 = ObtemValor(Request.GetQueryStringValue("p1"));
            p2 = ObtemValor(Request.GetQueryStringValue("p2"));
            p3 = ObtemValor(Request.GetQueryStringValue("p3"));
            p4 = ObtemValor(Request.GetQueryStringValue("p4"));
            p5 = ObtemValor(Request.GetQueryStringValue("p5"));
            p6 = ObtemValor(Request.GetQueryStringValue("p6"));
            p7 = ObtemValor(Request.GetQueryStringValue("p7"));
            p8 = ObtemValor(Request.GetQueryStringValue("p8"));
            p9 = ObtemValor(Request.GetQueryStringValue("p9"));
            p10 = ObtemValor(Request.GetQueryStringValue("p10"));
            p11 = ObtemValor(Request.GetQueryStringValue("p11"));
            p12 = ObtemValor(Request.GetQueryStringValue("p12"));

            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var tag = "venda" + empresa + matricula;

            try
            {                
                myApplication = HttpContext.Current.Application;

                if (myApplication [tag] == null)
                    myApplication [tag] = true;
                else
                    return BadRequest("Autorização em andamento. Aguarde o processamento para enviar uma nova venda!");
                
                // =============================
                // obtem terminal
                // =============================

                terminal = userLoggedName.PadLeft(8, '0');

                // =============================
                // verifica cartão
                // =============================

                #region - code - 

                var associadoPrincipal = (from e in db.T_Cartao
                                          where e.st_empresa == empresa
                                          where e.st_matricula == matricula
                                          where e.st_titularidade == "01"
                                          select e).
                                 FirstOrDefault();

                var dadosProprietario = (from e in db.T_Proprietario
                                         where e.i_unique == associadoPrincipal.fk_dadosProprietario
                                         select e).
                                          FirstOrDefault();

                int titularidadeFinal = 1, via = 1;

                var calcAcesso = new CodigoAcesso();

                var codAcessoCalc = calcAcesso.Obter(empresa,
                                                       matricula,
                                                       titularidadeFinal,
                                                       via,
                                                       dadosProprietario.st_cpf);

                while (codAcessoCalc != codAcesso)
                {
                    via = 0;

                    for (int t = 0; t < 9; ++t)
                    {
                        codAcessoCalc = calcAcesso.Obter(empresa,
                                                           matricula,
                                                           titularidadeFinal,
                                                           ++via,
                                                           dadosProprietario.st_cpf);

                        if (codAcessoCalc == codAcesso)
                            break;
                    }

                    if (codAcessoCalc == codAcesso)
                        break;

                    titularidadeFinal++;

                    if (titularidadeFinal > 9)
                        break;
                }

                if (titularidadeFinal != 1 && titularidadeFinal < 10)
                {
                    var associado = (from e in db.T_Cartao
                                     where e.st_empresa == empresa
                                     where e.st_matricula == matricula
                                     where e.st_titularidade == titularidadeFinal.ToString().PadLeft(2, '0')
                                     select e).
                                     FirstOrDefault();

                    if (associado.st_venctoCartao != stVencimento)
                        return BadRequest("Cartão inválido (0xA1)");
                }
                else
                {
                    if (associadoPrincipal.st_venctoCartao != stVencimento)
                        return BadRequest("Cartão inválido (0xA)");
                }

                #endregion
                
                #region - autorizador interno -

                var cartPortador = db.T_Cartao.FirstOrDefault(y => y.i_unique == idCartao);

                var v = new DataModel.VendaEmpresarial();

                v.input_cont_pe.st_terminal = terminal;
                v.input_cont_pe.st_empresa = empresa;
                v.input_cont_pe.st_matricula = matricula;
                v.input_cont_pe.st_titularidade = cartPortador.st_titularidade;
                v.input_cont_pe.nu_parcelas = parcelas.ToString().PadLeft(2,'0');
                v.input_cont_pe.vr_valor = valor.ToString();
                v.input_cont_pe.st_valores = "";
                v.input_cont_pe.tipoWeb = tipoWeb;

                if (parcelas >= 1) v.input_cont_pe.st_valores += p1.ToString().PadLeft(12, '0');
                if (parcelas >= 2) v.input_cont_pe.st_valores += p2.ToString().PadLeft(12, '0');
                if (parcelas >= 3) v.input_cont_pe.st_valores += p3.ToString().PadLeft(12, '0');
                if (parcelas >= 4) v.input_cont_pe.st_valores += p4.ToString().PadLeft(12, '0');
                if (parcelas >= 5) v.input_cont_pe.st_valores += p5.ToString().PadLeft(12, '0');
                if (parcelas >= 6) v.input_cont_pe.st_valores += p6.ToString().PadLeft(12, '0');
                if (parcelas >= 7) v.input_cont_pe.st_valores += p7.ToString().PadLeft(12, '0');
                if (parcelas >= 8) v.input_cont_pe.st_valores += p8.ToString().PadLeft(12, '0');
                if (parcelas >= 9) v.input_cont_pe.st_valores += p9.ToString().PadLeft(12, '0');
                if (parcelas >= 10) v.input_cont_pe.st_valores += p10.ToString().PadLeft(12, '0');
                if (parcelas >= 11) v.input_cont_pe.st_valores += p11.ToString().PadLeft(12, '0');
                if (parcelas >= 12) v.input_cont_pe.st_valores += p12.ToString().PadLeft(12, '0');

                if (senha != null)
                    v.input_cont_pe.st_senha = new DataModel.BaseVenda().DESCript(senha.PadLeft(8, '*'), "12345678");

                v.Run(db);

                var cdResp = v.output_cont_pr.st_codResp;

                if (cdResp != "0000")
                {
                    myApplication[tag] = null;

                    if (cdResp == "0505")
                    {
                        return BadRequest("(05) Cartão bloqueado, procure a instituição emissora do cartão");
                    }
                    else if (cdResp == "4343")
                    {
                        var numErros = (from e in db.T_Cartao
                                        where e.i_unique == associadoPrincipal.i_unique
                                        select e.nu_senhaErrada).
                                        FirstOrDefault();

                        if (numErros == 3)
                        {
                            // ultima!
                            return BadRequest("(44) Senha inválida. A próxima senha inválida irá bloquear o cartão!");
                        }
                        else if (numErros >= 4)
                        {
                            return BadRequest("(05) Cartão bloqueado, procure a instituição emissora do cartão");
                        }
                        else
                        {
                            var tentativas = "Você ainda tem (" + (4 - numErros) + ") tentativas";

                            return BadRequest("(43) Senha inválida! " + tentativas);
                        }
                    }
                    else
                        return BadRequest("Falha VC (0xE" + cdResp + " - " + v.output_st_msg + " )");
                }

                nsu_retorno = v.output_cont_pr.st_nsuRcb;

                var vc = new DataModel.VendaEmpresarialConfirmacao();

                vc.Run(db, v.output_cont_pr.st_nsuRcb);

                #endregion
                
                var cupom = new Cupom().
                       Venda(db,
                               associadoPrincipal,
                               dadosProprietario,
                               DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                               nsu_retorno,
                               terminal,
                               (int)parcelas,
                               valor,
                               p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);

                myApplication[tag] = null;

                return Ok(new
                {
                    count = 1,
                    results = cupom
                });
            }
            catch (SystemException ex)
            {
                myApplication[tag] = null;

                return BadRequest(ex.ToString());
            }
        }
    }
}
