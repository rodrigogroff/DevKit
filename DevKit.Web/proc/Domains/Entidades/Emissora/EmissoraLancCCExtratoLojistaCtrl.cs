using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DtoExtratoLojista_LancFech
    {
        public string ano { get; set; }
        public string mes { get; set; }
        public string valor { get; set; }
        public string situacao { get; set; }
    }

    public class EmissoraLancCCExtratoLojistaController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));
            var codigo = Request.GetQueryStringValue("codigo");

            if (string.IsNullOrEmpty(codigo))
                return BadRequest("Informe um código");
            
            var mon = new money();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            var t_loja = db.T_Loja.FirstOrDefault(y => y.st_loja == codigo);

            if (t_loja == null)
                return BadRequest("Informe um codigo válido");

            var lst = new List<DtoExtratoLojista_LancFech>();
            var dt = new DateTime(ano, mes, 1);
            long tot = 0;

            while (true)
            {
                var queryLancCC = (from e in db.LOG_Fechamento
                                   where e.fk_empresa == tEmp.i_unique
                                   where e.fk_loja == t_loja.i_unique
                                   where e.st_ano == dt.Year.ToString()
                                   where e.st_mes == dt.Month.ToString().PadLeft(2, '0')
                                   select e).
                                    ToList();

                if (queryLancCC.Count == 0)
                    break;

                var _t = queryLancCC.Sum(y => (long)y.vr_valor);

                lst.Add(new DtoExtratoLojista_LancFech
                {
                    ano = dt.Year.ToString(),
                    mes = dt.Month.ToString().PadLeft(2, '0'),
                    valor = mon.setMoneyFormat(_t),
                    situacao = "Aberto",
                });

                tot += _t;

                dt = dt.AddMonths(1);
            }

            return Ok(new 
            {
                cnpj = t_loja.nu_CNPJ,
                social = t_loja.st_social,
                fantasia = t_loja.st_nome,
                valor = "R$ " + mon.setMoneyFormat(tot),
                situacao = "",
                list = lst,
            }); 
        }        
    }
}
