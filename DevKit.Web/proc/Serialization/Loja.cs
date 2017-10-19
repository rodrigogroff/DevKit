
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class LojaMensagem
    {
        public string   id,
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

    public class Loja
    {
        public string id,
                      terminal,
                      nome,
                      cidade,
                      estado,
                      situacao,
                      tipoVenda,
                      tg_blocked,
                      tg_portalComSenha;

        public List<LojaMensagem> lstMensagens;

        public LojaMensagem novaMensagem;
    }
}