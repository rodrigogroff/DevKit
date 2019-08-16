using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using DataModel;
using System;
using SyCrafEngine;
using System.Security.Cryptography;
using System.Text;

namespace DevKit.Web.Controllers
{
    public class DBAUsuarioDTO
    {
        public string id,nome, empresa, sit, ultLogin;
    }

    public class DBAUsuarioNovo
    {
        public string idEmpresa, nome;
    }

    public class DBAUsuarioController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca")?.ToUpper();
            var id_empresa = Request.GetQueryStringValue<int?>("idEmpresa");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Usuario where e.st_empresa != "000000" select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.st_nome.ToUpper().Contains(busca));

            if (id_empresa > 0)
            {
                var stEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == id_empresa).st_empresa;

                query = query.Where(y => y.st_empresa == stEmp);
            }                

            query = query.OrderBy(y => y.st_empresa);

            var lst = new List<DBAUsuarioDTO>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
                lst.Add(new DBAUsuarioDTO
                {
                    id = item.i_unique.ToString(),
                    empresa = item.st_empresa,
                    nome = item.st_nome,
                    sit = item.tg_bloqueio.ToString() == "0" ? "Ativo" : "Bloqueado",
                    ultLogin = db.LOG_Audit.FirstOrDefault ( y=> y.fk_usuario == item.i_unique && y.st_oper == "Login")?.dt_operacao?.ToString("dd/MM/yyyy HH:mm"),
                });

            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }


        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.T_Usuario where e.i_unique == id select e).FirstOrDefault();

            return Ok(mdl);
        }

        [NonAction]
        public string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        [HttpPost]
        public IHttpActionResult Post(DBAUsuarioNovo mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            db.Insert(new T_Usuario
            {
                st_nome = mdl.nome,
                tg_bloqueio = '0',
                st_empresa = db.T_Empresa.FirstOrDefault(y => y.i_unique.ToString() == mdl.idEmpresa).st_empresa,
                st_senha = getMd5Hash (mdl.nome),
            });

            return Ok(mdl);
        }

        /*
        [HttpPut]
        public IHttpActionResult Put(T_Empresa mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            if (!string.IsNullOrEmpty(mdl.svrMensalidade))
                mdl.vr_mensalidade = (int) mon.getNumericValue(mdl.svrMensalidade);

            if (!string.IsNullOrEmpty(mdl.svrCartaoAtivo))
                mdl.vr_cartaoAtivo = (int)mon.getNumericValue(mdl.svrCartaoAtivo);

            if (!string.IsNullOrEmpty(mdl.svrMinimo))
                mdl.vr_minimo = (int)mon.getNumericValue(mdl.svrMinimo);

            if (!string.IsNullOrEmpty(mdl.svrTransacao))
                mdl.vr_transacao = (int)mon.getNumericValue(mdl.svrTransacao);

            if (!string.IsNullOrEmpty(mdl.snuFranquia))
                mdl.nu_franquia = (int)mon.getNumericValue(mdl.snuFranquia);

            if (mdl.nu_diaFech > 28)
                return BadRequest("Dia de fechamento precisa estar entre 1 e 28");

            if (mdl.nu_diaFech == 0)
                return BadRequest("Dia de fechamento não pode estar zerado");

            db.Update(mdl);

            return Ok();
        }

        
        */
    }
}
