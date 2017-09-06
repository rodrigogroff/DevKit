using System.Collections.Generic;
using System.Web.Http;
using System;

namespace DevKit.Web.Controllers
{
    public class EfetuaVendaController : ApiControllerBase
    {
        [NonAction]
        public long ObtemValor(string valor)
        {
            if (valor == null)
                return 0;

            var iValor = 0;

            if (!valor.Contains(","))
                valor += ",00";

            valor = valor.Replace(",", "").Replace(".", "");
            iValor = Convert.ToInt32(valor);

            return iValor;
        }

        public string empresa, matricula, codAcesso, stVencimento, retorno;
        public long valor, parcelas, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12;

        public IHttpActionResult Get()
        {
            empresa = Request.GetQueryStringValue("empresa");
            matricula = Request.GetQueryStringValue("matricula");
            codAcesso = Request.GetQueryStringValue("codAcesso");
            stVencimento = Request.GetQueryStringValue("stVencimento");
            valor = ObtemValor(Request.GetQueryStringValue("valor"));
            parcelas = Request.GetQueryStringValue<int>("parcelas");
            
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
            
            var sc = new SocketConvey();

            var sck = sc.connectSocket("localhost", 2000);

            if (sck == null)
                return BadRequest("Falha de comunicação com servidor (0x1)");
            
            if (!sc.socketEnvia(sck, MontaVendaDigitada()))
                return BadRequest("Falha de comunicação com servidor (0x2)");

            retorno = sc.socketRecebe(sck);

            if (retorno == "")
                return BadRequest("Falha de comunicação com servidor (0x3)");

            if (!sc.socketEnvia(sck, MontaConfirmacao()))
                return BadRequest("Falha de comunicação com servidor (0x4)");

            retorno = sc.socketRecebe(sck);

            if (retorno == "")
                return BadRequest("Falha de comunicação com servidor (0x5)");

            return Ok(new
            {
                count = 0,
                results = new List<string> { "OK" }
            });
        }

        public string MontaVendaDigitada()
        {
            // ------------------------------------
            // monta venda digitada
            // ------------------------------------

            var reg = "05DICE";

            reg += db.currentUser.st_loja.PadRight(8, ' ');

            reg += "000000";

            reg += empresa.PadLeft(6, '0');
            reg += matricula.PadLeft(6, '0');
            reg += "01";
            reg += codAcesso.PadRight(16, ' ');

            reg += valor.ToString().PadLeft(12, '0');
            reg += parcelas.ToString().PadLeft(2, '0');

            string valores = "";

            if (parcelas >= 1) valores += p1.ToString().PadLeft(12, '0');
            if (parcelas >= 2) valores += p2.ToString().PadLeft(12, '0');
            if (parcelas >= 3) valores += p3.ToString().PadLeft(12, '0');
            if (parcelas >= 4) valores += p4.ToString().PadLeft(12, '0');
            if (parcelas >= 5) valores += p5.ToString().PadLeft(12, '0');
            if (parcelas >= 6) valores += p6.ToString().PadLeft(12, '0');
            if (parcelas >= 7) valores += p7.ToString().PadLeft(12, '0');
            if (parcelas >= 8) valores += p8.ToString().PadLeft(12, '0');
            if (parcelas >= 9) valores += p9.ToString().PadLeft(12, '0');
            if (parcelas >= 10) valores += p10.ToString().PadLeft(12, '0');
            if (parcelas >= 11) valores += p11.ToString().PadLeft(12, '0');
            if (parcelas >= 12) valores += p12.ToString().PadLeft(12, '0');

            reg += valores;

            return reg;
        }

        public string MontaConfirmacao()
        {
            // ------------------------------------
            // monta venda digitada
            // ------------------------------------

            var reg = "05CECC";
            
            /*
            
            string trilha = client_msg.Substring ( 14, 27 );
			
			POS_Entrada  pe = new POS_Entrada();
			
			pe.set_st_terminal 		( client_msg.Substring 	(  6,  8 ) );
			
            pe.set_st_empresa   	( trilha.Substring 		(  6,  6 ) );
            pe.set_st_matricula 	( trilha.Substring	 	( 12,  6 ) );
            pe.set_st_titularidade 	( trilha.Substring 		( 18,  2 ) );

            exec_pos_confirmaVendaEmpresarial tr = new exec_pos_confirmaVendaEmpresarial ( trans );
            
            tr.input_cont_pe  = pe;
            tr.input_st_nsu   = client_msg.Substring ( 41, 6 );
            
            */

            return reg;
        }
    }
}
