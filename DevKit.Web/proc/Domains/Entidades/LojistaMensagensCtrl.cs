using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System;

namespace DevKit.Web.Controllers
{
    public class LojistaMensagensController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_LojaMensagem
                         where e.fk_loja == db.currentLojista.i_unique || e.fk_loja == 0
                         where DateTime.Now < e.dt_validade
                         where e.tg_ativa == true
                         select e);
            
            query = query.OrderByDescending(y => y.dt_criacao);

            var lst = new List<LojaMensagem>();

            foreach (var item in query.ToList())
            {
                lst.Add(new LojaMensagem
                {
                    mensagem = item.st_msg,
                    link = item.st_link,
                    dt_criacao = Convert.ToDateTime(item.dt_criacao).ToString("dd/MM/yyyy")
                });
            }

            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }
    }
}
