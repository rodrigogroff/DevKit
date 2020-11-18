using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class OnboardingLojista
    {
        public string email { get; set; }

        public string senha { get; set; }

        public string cnpj { get; set; }

        public string razSoc { get; set; }

        public string fantasia { get; set; }

        public string cep { get; set; }

        public string cepStr { get; set; }

        public string numero { get; set; }

        public string cepInst { get; set; }

        public string cepInstStr { get; set; }

        public string numeroInst { get; set; }

        public string telCel { get; set; }

        public string telFixo { get; set; }

        public string resp { get; set; }

        public int fk_banco { get; set; }

        public string ag { get; set; }

        public string conta { get; set; }

        public string sitef { get; set; }

        public string assInst { get; set; }
    }

    public static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            return null;
        }
    }

    public class OnboardingController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/onboarding_lojista")]
        public IHttpActionResult onboarding_lojista(OnboardingLojista mdl)
        {
            using (var db = new AutorizadorCNDB())
            {
                var mon = new SyCrafEngine.money();

                var mdlNew = new T_Loja
                {
                    st_nome = mdl.fantasia,
                    st_social = mdl.razSoc,
                    st_enderecoInst = mdl.cepInstStr.Split(',')[0] + " " + mdl.numeroInst,
                    st_endereco = mdl.cepStr.Split(',')[0] + " " + mdl.numero,
                    st_contato = mdl.resp,
                    st_email = mdl.email,
                    st_senha = mdl.senha,
                    nu_telefone = mdl.telFixo,
                    st_telCelular = mdl.telCel,
                    st_cidade = mdl.cepStr.Split(',')[1],
                    st_estado = "RS",
                    nu_CEP = mdl.cep,
                    nu_inscEst = "",
                    nu_CNPJ = mdl.cnpj,
                    fk_banco = mdl.fk_banco,
                    st_agencia = mdl.ag,
                    st_conta = mdl.conta,
                };

                mdlNew.tg_isentoFat = 0;
                mdlNew.tg_blocked = '0';

                mdlNew.nu_periodoFat = 20;
                mdlNew.nu_diavenc = 3;
                mdlNew.nu_pctValor = 0;
                mdlNew.vr_mensalidade = 4590;
                mdlNew.vr_minimo = 0;
                mdlNew.vr_transacao = 90;
                mdlNew.nu_franquia = 0;

                mdlNew.st_obs = "";

                // ----------------
                // obter codigo
                // ----------------

                var cd = Convert.ToInt32(
                            db.T_Loja.
                                Where(y => Convert.ToInt64(y.st_loja) > 6300).
                                OrderByDescending(y => y.st_loja).
                                Select(y => y.st_loja).
                                FirstOrDefault()) + 1;

                mdlNew.st_loja = cd.ToString();

                mdlNew.i_unique = Convert.ToInt64(db.InsertWithIdentity(mdlNew));

                // -------------
                // terminal
                // -------------

                var ult = db.T_Terminal.
                                Where(y => Convert.ToInt32(y.nu_terminal) > 7700).
                                Where(y => Convert.ToInt32(y.nu_terminal) < 8000).
                                OrderByDescending(y => y.nu_terminal).FirstOrDefault();

                var novo = new T_Terminal
                {
                    nu_terminal = (Convert.ToInt32(ult.nu_terminal) + 1).ToString(),
                    fk_loja = (int)mdlNew.i_unique,
                    st_localizacao = "Terminal Automático"
                };

                novo.i_unique = Convert.ToInt64(db.InsertWithIdentity(novo));

                var textoEmail = "<br><h3>PREZADO LOJISTA:</h3><br>";

                textoEmail += "<p>Você está recebendo os anexos relativos ao contrato com a CONVEY BENEFÍCIOS,  que foi efetuado cadastro via adesão comercial eletrônica. <br>" +
                              "<br>Veja seus dados de acesso: </p><br><b>Dados de acesso</b><br>" +
                              "NÚMERO DE CONTRATO: Lojista: <b>" + mdlNew.st_loja + "</b><br>" +
                              "RAZÃO SOCIAL: <b>" + mdlNew.st_social + "</b><br>" +
                              "SITEF: <b>" + (mdl.sitef == "true" ? "SIM" : "NÃO") + "</b><br>" +
                              "ASS. INSTITUIÇÃO: <b>" + (mdl.assInst == "true" ? "SIM" : "NÃO") + "</b><br>" +
                              "DATA DE ADESÃO: <b>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</b><br>" +
                              "IP DO ENVIO: <b>" + HttpRequestMessageExtensions.GetClientIpAddress(this.Request) + "</b><br>" +
                              "Terminal de venda: <b>" + novo.nu_terminal + "</b><br>" +
                              "Senha: " + mdl.senha + "<br>" +
                              "<br><h3>ACESSO ÀS VENDAS:</h3>" +
                              "<a href='https://meuconvey.conveynet.com.br/login?tipo=1'>https://meuconvey.conveynet.com.br/login?tipo=1</a><br>" +
                              "<br><h3>ACESSO AS INFORMAÇÕES REPASSE FINANCEIRO:</h3>" +
                              "<a href='https://meuconvey.conveynet.com.br/login?tipo=3'>https://meuconvey.conveynet.com.br/login?tipo=3</a><br>" +
                              "<br><br>" +
                              "<h3>Baixe o app vendas nas lojas Android</h3>" +
                              "<br>" +
                              "Conveynet.com.br<br>" +
                              "<br><h3>DOCUMENTOS NECESSÁRIOS:</h3>" +
                              "<a target='_blank' href='http://meuconvey.conveynet.com.br/FORMULARIO_DE_CREDENCIAMENTO_COMERCIAL_CONVEY_BENEFICIOS.docx'>FORMULARIO_DE_CREDENCIAMENTO_COMERCIAL_CONVEY_BENEFICIOS.docx</a><br>" +
                              "<a target='_blank' href='http://meuconvey.conveynet.com.br/CONTRATO_ESTABELECIMENTO_COMERCIAL_CONVEY_BENEFICIOS.pdf'>CONTRATO_ESTABELECIMENTO_COMERCIAL_CONVEY_BENEFICIOS.pdf</a><br>" +
                              "<br>" +
                              "<br>" +
                              "Dúvidas: (51) 3500.8703 para capitais e regiões metropolitanas<br>" +
                              "Segunda a sexta, das 9h às 17hs<br>" +
                              "<br><i>Se você já contatou o canal de atendimento habitual, mas não ficou satisfeito,<br>" +
                              "entre em contato com a Ouvidoria.<br>" +
                              "<a mailto='e-mail:atendimento@conveynet.com.br'>atendimento@conveynet.com.br</a><br:" +
                              "";

                SendEmail("CONVEY - PROPOSTA COMERCIAL APROVADA – DADOS DE ACESSO", textoEmail, mdl.email );
                SendEmail("CONVEY - PROPOSTA COMERCIAL APROVADA – DADOS DE ACESSO", textoEmail, "atendimento@conveynet.com.br");

                return Ok();
            }
        }
    }
}
