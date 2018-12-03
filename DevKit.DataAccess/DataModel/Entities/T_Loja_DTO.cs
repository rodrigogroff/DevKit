using System.Collections.Generic;

namespace DataModel
{
    public class LojaMensagem
    {
        public string id,
                        validade,
                        dia_final,
                        mes_final,
                        ano_final,
                        mensagem,
                        dt_criacao,
                        link,
                        tipo;

        public bool generica,
                    ativa;
    }

    public class LojaTerminal
    {
        public string id, codigo, texto;
    }

    public class LojaConvenio
    {
        public string id, empresa, idEmpresa, tx_admin;
    }

    public partial class T_Loja
    {
        public bool isentoFat = false;

        public string id,
                        terminal,
                        nome,
                        cidade,
                        estado,
                        situacao,
                        tipoVenda,

                        snuPctValor,
                        svrMensalidade,
                        svrMinimo,
                        svrTransacao,
                        snuFranquia;                        

        public List<LojaMensagem> lstMensagens;
        public List<LojaTerminal> lstTerminais;
        public List<LojaConvenio> lstConvenios;

        public LojaMensagem novaMensagem;

        public LojaTerminal novoTerminal;
        public LojaTerminal editTerminal;

        public LojaConvenio editConvenio;
        public LojaConvenio novoConvenio;
    }
}
