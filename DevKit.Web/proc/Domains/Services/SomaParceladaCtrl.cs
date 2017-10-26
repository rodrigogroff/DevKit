using System.Collections.Generic;
using System.Web.Http;
using System;

namespace DevKit.Web.Controllers
{
    public class SomaParceladaView
    {
        public string requerSenha = "";
    }
    
    public class SomaParceladaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var valor = ObtemValor(Request.GetQueryStringValue("valor"));
            var parcelas = Request.GetQueryStringValue<int>("parcelas");

            long  p1 = ObtemValor(Request.GetQueryStringValue("p1")),
                    p2 = ObtemValor(Request.GetQueryStringValue("p2")),
                    p3 = ObtemValor(Request.GetQueryStringValue("p3")),
                    p4 = ObtemValor(Request.GetQueryStringValue("p4")),
                    p5 = ObtemValor(Request.GetQueryStringValue("p5")),
                    p6 = ObtemValor(Request.GetQueryStringValue("p6")),
                    p7 = ObtemValor(Request.GetQueryStringValue("p7")),
                    p8 = ObtemValor(Request.GetQueryStringValue("p8")),
                    p9 = ObtemValor(Request.GetQueryStringValue("p9")),
                    p10 = ObtemValor(Request.GetQueryStringValue("p10")),
                    p11 = ObtemValor(Request.GetQueryStringValue("p11")),
                    p12 = ObtemValor(Request.GetQueryStringValue("p12"));

            long vrTotal = 0;

            if (parcelas >= 1) vrTotal += p1;
            if (parcelas >= 2) vrTotal += p2;
            if (parcelas >= 3) vrTotal += p3;
            if (parcelas >= 4) vrTotal += p4;
            if (parcelas >= 5) vrTotal += p5;
            if (parcelas >= 6) vrTotal += p6;
            if (parcelas >= 7) vrTotal += p7;
            if (parcelas >= 8) vrTotal += p8;
            if (parcelas >= 9) vrTotal += p9;
            if (parcelas >= 10) vrTotal += p10;
            if (parcelas >= 11) vrTotal += p11;
            if (parcelas >= 12) vrTotal += p12;

            if (vrTotal != valor)
                return BadRequest("SOMA DAS PARCELAS DIVERGE DO VALOR TOTAL");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var senha = "1";

            if (db.currentLojista.tg_portalComSenha == 0 )
                senha = "0";

            var lst = new List<SomaParceladaView>();

            lst.Add(new SomaParceladaView
            {
                requerSenha = senha
            });

            return Ok(new
            {
                count = 0,
                results = lst
            });
        }
    }
}
