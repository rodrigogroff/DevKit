using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class POS_Entrada
    {
        public string st_empresa,
                        st_matricula,
                        st_titularidade,
                        st_senha,
                        st_terminal,
                        st_terminalSITEF,
                        vr_valor,
                        nu_parcelas,
                        st_valores,
                        st_nsuOrigemSITEF;
    }

    public class POS_Resposta
    {
        public string st_codResp,
                      st_nsuRcb,
                      st_nsuBanco,
                      st_nomeCliente,
                      st_loja,
                      st_PAN,
                      st_mesPri,
                      st_variavel;
    }
}
