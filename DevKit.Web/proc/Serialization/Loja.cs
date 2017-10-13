
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class LojaMensagem
    {
        public string validade,
                        mensagem,
                        link,
                        tipo;
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
    }
}