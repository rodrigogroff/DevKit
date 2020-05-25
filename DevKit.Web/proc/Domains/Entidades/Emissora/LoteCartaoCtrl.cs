using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

using SyCrafEngine;
using LinqToDB;
using DataModel;
using System.Net.Http;
using App.Web;

namespace DevKit.Web.Controllers
{
    public class LoteCartaoDTO
    {
        public string mat { get; set; }
        public string modo { get; set; }
    }

    public class LoteCartaoController : ApiControllerBase
    {
        [HttpPut]
        public IHttpActionResult Put(LoteCartaoDTO dto)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (string.IsNullOrEmpty(dto.modo) && string.IsNullOrEmpty(dto.mat))
                return BadRequest();

            bool changes = false;

            if (dto.modo == "Bloqueio")
            {
                var mats = dto.mat.TrimEnd(';').Split(';');

                foreach (var mat in mats)
                {
                    if (mat == "") continue;

                    foreach (var cart in db.T_Cartao.Where(y => y.st_empresa == userLoggedEmpresa && y.st_matricula == mat.PadLeft(6, '0')).ToList())
                    {
                        cart.tg_status = Convert.ToChar(CartaoStatus.Bloqueado);
                        cart.dt_bloqueio = DateTime.Now;

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Bloqueio (lote)",
                            st_log = "Mat: " + cart.st_matricula
                        });

                        changes = true;
                    }
                }
            }
            else if (dto.modo == "Desbloqueio")
            {
                var mats = dto.mat.TrimEnd(';').Split(';');

                foreach (var mat in mats)
                {
                    if (mat == "") continue;

                    foreach (var cart in db.T_Cartao.Where(y => y.st_empresa == userLoggedEmpresa && y.st_matricula == mat.PadLeft(6, '0')).ToList())
                    {
                        cart.tg_status = Convert.ToChar(CartaoStatus.Habilitado);

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Desbloqueio (lote)",
                            st_log = "Mat: " + cart.st_matricula
                        });

                        changes = true;
                    }
                }
            }

            if (changes)
                return Ok();
            else
                return BadRequest(dto.mat + " não foi encontrada!");
        }
    }
}
