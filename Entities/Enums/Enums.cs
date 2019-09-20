
using System.Collections.Generic;
using System.Linq;

namespace Entities.Enums
{
    public class TipoConfirmacao
    {
        public const string Pendente = "0",
                            Confirmada = "1",
                            Negada = "2",
                            Erro = "3",
                            Registro = "4",
                            Cancelada = "5",
                            Desfeita = "6";
    }

    public class TipoCaptura
    {
        public const string SITEF = "1",
                            PORTAL = "2";
    }

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
                        st_codLoja,
                        st_nsuOrigemSITEF,
                        tipoWeb = "";
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
                      st_variavel,
                      st_via;
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

    //public class Context
    //{
    //    public const string TRUE = "1";
    //    public const string FALSE = "0";
    //    public const string NOT_SET = "0";
    //    public const string EMPTY = "";
    //    public const string OPEN = "0";
    //    public const string CLOSED = "1";
    //    public const string NONE = "0";
    //}

    public class MotivoBloqueio
    {
        public const string PERDA = "0";
        public const string ROUBO = "1";
        public const string SENHA_ERRADA = "2";
        public const string CANCELAMENTO = "3";
        public const string ADMINISTRATIVO = "4";
    }

    public class Month
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class EnumMonth
    {
        public List<Month> lst = new List<Month>();

        public const long January = 1,
                            February = 2,
                            March = 3,
                            April = 4,
                            May = 5,
                            June = 6,
                            July = 7,
                            August = 8,
                            September = 9,
                            October = 10,
                            November = 11,
                            December = 12;

        public EnumMonth()
        {
            lst.Add(new Month() { id = January, stName = "Janeiro" });
            lst.Add(new Month() { id = February, stName = "Fevereiro" });
            lst.Add(new Month() { id = March, stName = "Março" });
            lst.Add(new Month() { id = April, stName = "Abril" });
            lst.Add(new Month() { id = May, stName = "Maio" });
            lst.Add(new Month() { id = June, stName = "Junho" });
            lst.Add(new Month() { id = July, stName = "Julho" });
            lst.Add(new Month() { id = August, stName = "Agosto" });
            lst.Add(new Month() { id = September, stName = "Setembro" });
            lst.Add(new Month() { id = October, stName = "Outubro" });
            lst.Add(new Month() { id = November, stName = "Novembro" });
            lst.Add(new Month() { id = December, stName = "Dezembro" });
        }

        public Month Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}
