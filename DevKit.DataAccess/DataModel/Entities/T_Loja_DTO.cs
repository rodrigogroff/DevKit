using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        public bool isentoFat = false;

        [NotMapped]
        public string   id,
                        terminal,
                        nome,
                        cidade,
                        estado,
                        situacao,
                        strTerminais,
                        strEmpresas,
                        tipoVenda,
                        snuPctValor,
                        svrMensalidade,
                        svrMinimo,
                        svrTransacao,
                        snuFranquia,
                        sdtCadastro;

        [NotMapped]
        public List<LojaMensagem> lstMensagens;

        [NotMapped]
        public List<LojaTerminal> lstTerminais;

        [NotMapped]
        public List<LojaConvenio> lstConvenios;

        [NotMapped]
        public LojaMensagem novaMensagem;

        [NotMapped]
        public LojaTerminal novoTerminal;

        [NotMapped]
        public LojaTerminal editTerminal;

        [NotMapped]
        public LojaConvenio editConvenio;

        [NotMapped]
        public LojaConvenio novoConvenio;
    }
}
