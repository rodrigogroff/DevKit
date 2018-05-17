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

    public class OperacaoCartao
    {
        public const string VENDA_EMPRESARIAL = "0";
        public const string CONF_VENDA_EMP = "1";
        public const string PAY_FONE_GRAVA_PEND = "2";
        public const string PAY_FONE_CANCELA_PEND = "3";
        public const string PAY_FONE_AUTORIZA_VENDA = "4";
        public const string PAY_FONE_CONFIRMA_VENDA = "5";
        public const string PAY_FONE_CANCELA_VENDA = "6";
        public const string PAY_FONE_CANCELA_PEND_LOJISTA = "7";
        public const string FALHA_VENDA_EMPRESARIAL = "8";
        public const string FALHA_PAY_FONE_GRAVA_PEND = "9";
        public const string FALHA_PAY_FONE_CANCELA_PEND = "10";
        public const string FALHA_PAY_FONE_AUTORIZA_VENDA = "11";
        public const string FALHA_PAY_FONE_CONFIRMA_VENDA = "12";
        public const string FALHA_PAY_FONE_CANCELA_VENDA = "13";
        public const string FALHA_PAY_FONE_CANCELA_PEND_LOJISTA = "14";
        public const string VENDA_EMPRESARIAL_CANCELA = "15";
        public const string FALHA_VENDA_EMPRESARIAL_CANCELA = "16";
        public const string FALHA_CONF_VENDA_EMP = "17";
        public const string EDU_DEP_IMEDIATO = "18";
        public const string EDU_DEP_FUNDO = "19";
        public const string EDU_DEP_DIARIO = "20";
        public const string VENDA_EMPRESARIAL_DESFAZ = "21";
    }

    public class Context
    {
        public const string TRUE = "1";
        public const string FALSE = "0";
        public const string NOT_SET = "0";
        public const string EMPTY = "";
        public const string OPEN = "0";
        public const string CLOSED = "1";
        public const string NONE = "0";
    }
}
