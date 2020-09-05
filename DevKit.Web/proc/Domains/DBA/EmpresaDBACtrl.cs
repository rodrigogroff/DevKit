using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using DataModel;
using System;
using SyCrafEngine;
using App.Web;
using System.Net.Http;

namespace DevKit.Web.Controllers
{
    public class EmpresaDBAController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/EmpresaDBA/exportar", Name = "ExportarEmpresaDBA")]
        public HttpResponseMessage ExportarEmpresaDBA()
        {
            StartDatabase();

            var abaString = "Listagem";

            var x = new ExportMultiSheetWrapper("empresas.xlsx");

            x.NovaAba_Header(abaString, (new List<string> 
            { 
                "Código",
                "CNPJ", 
                "Fantasia",
                "Social", 
                "Endereço",
                "Telefone", 
                "Cartões",
                "Max. Parc.", 
                "Dia Fat.", 
                "Fechamento",
                "Email Plastico",
            }).
            ToArray());

            foreach (var mdl in (from e in db.T_Empresa select e).ToList())
            {
                x.AdicionarConteudo(abaString, (new List<string>
                {
                    mdl.st_empresa,
                    mdl.nu_CNPJ,
                    mdl.st_fantasia,
                    mdl.st_social,
                    mdl.st_endereco + " - " +mdl.st_estado + " " +mdl.st_cidade,
                    mdl.nu_telefone,
                    mdl.nu_cartoes.ToString(),
                    mdl.nu_parcelas.ToString(),
                    mdl.nu_periodoFat.ToString(),
                    mdl.nu_diaFech?.ToString() + " - " +mdl.st_horaFech?.Substring(0,2) + ":" + mdl.st_horaFech?.Substring(2,2),
                    mdl.st_emailPlastico,
                }).
                ToArray());
            }

            return x.GeraXLS();
        }

        public IHttpActionResult Get()
        {
            if (userLoggedParceiroId != "1")
                return BadRequest("Não autorizado!");

            var busca = Request.GetQueryStringValue("busca")?.ToUpper();
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var cnpj = Request.GetQueryStringValue("cnpj");
            var cidade = Request.GetQueryStringValue("cidade");
            var estado = Request.GetQueryStringValue("estado");
            var fkParceiro = Request.GetQueryStringValue<long?>("parceiro");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Empresa select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.st_fantasia.ToUpper().Contains(busca) || y.st_empresa.ToUpper().Contains(busca) || y.st_social.ToUpper().Contains(busca));

            if (!string.IsNullOrEmpty(cnpj))
                query = query.Where(y => y.nu_CNPJ.Contains(cnpj));

            if (!string.IsNullOrEmpty(cidade))
                query = query.Where(y => y.st_cidade.ToUpper().Contains(cidade));

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(y => y.st_estado.ToUpper().Contains(estado));

            if (fkParceiro > 0)
                query = query.Where(y => y.fkParceiro == fkParceiro);

            query = query.OrderBy(y => y.st_fantasia);

            var lst = new List<T_Empresa>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
                lst.Add(item);

            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.T_Empresa where e.i_unique == id select e).FirstOrDefault();

            if (mdl.vr_mensalidade != null)
                mdl.svrMensalidade = new money().setMoneyFormat((long)mdl.vr_mensalidade);

            if (mdl.vr_cartaoAtivo != null)
                mdl.svrCartaoAtivo = new money().setMoneyFormat((long)mdl.vr_cartaoAtivo);

            if (mdl.vr_minimo != null)
                mdl.svrMinimo = new money().setMoneyFormat((long)mdl.vr_minimo);

            if (mdl.vr_transacao != null)
                mdl.svrTransacao = new money().setMoneyFormat((long)mdl.vr_transacao);

            mdl.snuFranquia = mdl.nu_franquia?.ToString();

            mdl.stotLojistas = db.LINK_LojaEmpresa.Where(y => y.fk_empresa == mdl.i_unique).ToList().Count().ToString();

            mdl.lstFechamento = db.T_JobFechamento.Where(y => y.fk_empresa == mdl.i_unique).OrderByDescending(y => y.i_unique).Take(5).ToList();

            var mon = new money();

            foreach (var itemFech in mdl.lstFechamento)
            {
                itemFech.sdt_inicio = itemFech.dt_inicio?.ToString("dd/MM/yyyy HH:mm");
                itemFech.sdt_fim = itemFech.dt_fim?.ToString("dd/MM/yyyy HH:mm");

                itemFech.sfechCartoes = db.LOG_Fechamento.Where(y => y.fk_empresa == mdl.i_unique &&
                                                                                     y.st_ano == itemFech.st_ano &&
                                                                                     y.st_mes == itemFech.st_mes ).
                                                                                     Select(y => y.fk_cartao).
                                                                                     Distinct().
                                                                                     Count().
                                                                                     ToString();

                itemFech.sfechValorTotal = "R$ " + mon.formatToMoney(db.LOG_Fechamento.Where(y => y.fk_empresa == mdl.i_unique &&
                                                                y.st_ano == itemFech.st_ano &&
                                                                y.st_mes == itemFech.st_mes).
                                                                 Sum(y => y.vr_valor).ToString());
            }

            mdl._tg_bloq = mdl.tg_bloq == 1;
            mdl._tg_isentoFat = mdl.tg_isentoFat == 1;

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(mdl);
        }

        [HttpPut]
        public IHttpActionResult Put(T_Empresa mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            if (!string.IsNullOrEmpty(mdl.svrMensalidade))
                mdl.vr_mensalidade = (int) mon.getNumericValue(mdl.svrMensalidade);

            if (!string.IsNullOrEmpty(mdl.svrCartaoAtivo))
                mdl.vr_cartaoAtivo = (int)mon.getNumericValue(mdl.svrCartaoAtivo);

            if (!string.IsNullOrEmpty(mdl.svrMinimo))
                mdl.vr_minimo = (int)mon.getNumericValue(mdl.svrMinimo);

            if (!string.IsNullOrEmpty(mdl.svrTransacao))
                mdl.vr_transacao = (int)mon.getNumericValue(mdl.svrTransacao);

            if (!string.IsNullOrEmpty(mdl.snuFranquia))
                mdl.nu_franquia = (int)mon.getNumericValue(mdl.snuFranquia);

            if (mdl.nu_diaFech > 28)
                return BadRequest("Dia de fechamento precisa estar entre 1 e 28");

            if (mdl.nu_diaFech == 0)
                return BadRequest("Dia de fechamento não pode estar zerado");

            if (mdl._tg_bloq == true) mdl.tg_bloq = 1; else mdl.tg_bloq = 0;
            if (mdl._tg_isentoFat == true) mdl.tg_isentoFat = 1; else mdl.tg_isentoFat = 0;

            db.Update(mdl);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post(T_Empresa mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            mdl.st_empresa = mdl.st_empresa.PadLeft(6, '0');

            if (!string.IsNullOrEmpty(mdl.svrMensalidade))
                mdl.vr_mensalidade = (int)mon.getNumericValue(mdl.svrMensalidade);

            if (!string.IsNullOrEmpty(mdl.svrCartaoAtivo))
                mdl.vr_cartaoAtivo = (int)mon.getNumericValue(mdl.svrCartaoAtivo);

            if (!string.IsNullOrEmpty(mdl.svrMinimo))
                mdl.vr_minimo = (int)mon.getNumericValue(mdl.svrMinimo);

            if (!string.IsNullOrEmpty(mdl.svrTransacao))
                mdl.vr_transacao = (int)mon.getNumericValue(mdl.svrTransacao);

            if (!string.IsNullOrEmpty(mdl.snuFranquia))
                mdl.nu_franquia = (int)mon.getNumericValue(mdl.snuFranquia);

            mdl.i_unique = Convert.ToInt64(db.InsertWithIdentity(mdl));

            return Ok(mdl);
        }
    }
}
