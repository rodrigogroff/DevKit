using DataModel;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DtoEmissoraLancCC
    {
        public long id { get; set; }
        public string dtCriacao { get; set; }
        public string nuAno { get; set; }
        public string nuMes { get; set; }
        public string nuMatricula { get; set; }
        public string stFOPA { get; set; }
        public string stNome { get; set; }
        public string stTipo { get; set; }
        public string vrValor { get; set; }
        public string nuParcela { get; set; }
        public string _fkTipo { get; set; }
        public string _fkCartao { get; set; }
        public string _totParcelas { get; set; }
    }

    public class EmissoraLancCCController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            var del_item = Request.GetQueryStringValue<int?>("del_item");

            var nome = Request.GetQueryStringValue("nome");
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));
            var mat = Request.GetQueryStringValue("mat");

            var mon = new money();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            if (del_item != null)
            {
                var di = db.LancamentosCC.FirstOrDefault(y => y.id == del_item);

                if (di != null)
                {
                    if (di.nuParcela == 1)
                    {
                        db.Delete(di);

                        foreach (var item in db.LancamentosCC.Where(y => y.fkInicial == di.id).ToList())
                            db.Delete(item);

                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Somente lançamento inicial pode ser deletada");
                    }
                }
            }

            var query = (from e in db.LancamentosCC
                         where e.fkEmpresa == tEmp.i_unique
                         where e.nuAno == ano
                         where e.nuMes == mes
                         select e);

            if (!string.IsNullOrEmpty(nome))
            {
                query = from e in query
                        join cart in db.T_Cartao on (long)e.fkCartao equals cart.i_unique
                        join prop in db.T_Proprietario on (long)cart.fk_dadosProprietario equals prop.i_unique
                        where prop.st_nome.Contains(nome)
                        select e;
            }

            if (!string.IsNullOrEmpty(mat))
            {
                query = from e in query
                        join cart in db.T_Cartao on (long)e.fkCartao equals cart.i_unique
                        where cart.stCodigoFOPA.Contains(mat) || cart.st_matricula.Contains(mat)
                        select e;
            }

            query = query.OrderByDescending(y => y.id);

            var res = new List<DtoEmissoraLancCC>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var t_cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == item.fkCartao);

                res.Add(new DtoEmissoraLancCC
                {
                    id = item.id,
                    dtCriacao = item.dtLanc?.ToString("dd/MM/yyyy"),
                    nuAno = item.nuAno.ToString(),
                    nuMes = item.nuMes.ToString(),
                    nuMatricula = t_cart.st_matricula,
                    stFOPA = t_cart.stCodigoFOPA,
                    stNome = t_cart.st_titularidade == "01" ? db.T_Proprietario.FirstOrDefault(y => y.i_unique == t_cart.fk_dadosProprietario).st_nome :
                                                              db.T_Dependente.FirstOrDefault(y => y.i_unique == t_cart.fk_dadosProprietario).st_nome,
                    stTipo = item.bRecorrente != true ? db.EmpresaDespesa.FirstOrDefault(y => y.id == item.fkTipo).stDescricao :
                                                        db.EmpresaDespesaRecorrente.FirstOrDefault(y => y.id == item.fkTipo).stDescricao,
                    nuParcela = item.nuTotParcelas > 1 ? item.nuParcela + " / " + item.nuTotParcelas : "1",
                    vrValor = mon.setMoneyFormat((long)item.vrValor),
                });
            }

            return Ok(new { count = query.Count(), results = res });
        }

        [HttpPost]
        public IHttpActionResult Post(DtoEmissoraLancCC mdl)
        {
            var mon = new money();

            if (mdl._fkCartao == null)
                return BadRequest("Informe um cartão válido");

            if (mdl._fkTipo == null)
                return BadRequest("Informe um tipo de despesa válido");

            if (string.IsNullOrEmpty(mdl.stFOPA))
                return BadRequest("Informe um cartão válido");

            if (string.IsNullOrEmpty(mdl.nuAno))
                return BadRequest("Informe um ano válido");

            if (string.IsNullOrEmpty(mdl.nuMes))
                return BadRequest("Informe um mês válido");

            if (string.IsNullOrEmpty(mdl.vrValor))
                return BadRequest("Informe um valor válido");

            if (string.IsNullOrEmpty(mdl._totParcelas))
                return BadRequest("Informe um numero de parcelas válido");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var t_cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == Convert.ToInt32(mdl._fkCartao) && y.st_empresa == db.currentEmpresa.st_empresa);

            if (t_cart.stCodigoFOPA != mdl.stFOPA)
            {
                t_cart.stCodigoFOPA = mdl.stFOPA;

                db.Update(t_cart);
            }

            if (t_cart == null)
                return BadRequest("Cartão inválido");            

            var tot_parcelas = Convert.ToInt32(mdl._totParcelas);

            var totVal = mon.getNumericValue(mdl.vrValor);
            var vrDivValor = totVal / tot_parcelas;

            var primValor = vrDivValor;

            if (vrDivValor * tot_parcelas != totVal)
                primValor = totVal - vrDivValor * (tot_parcelas - 1);

            var fkInicial = Convert.ToInt64 (db.InsertWithIdentity(new LancamentosCC
            {
                fkCartao = (int) t_cart.i_unique,
                bRecorrente = false,
                dtLanc = DateTime.Now,
                fkEmpresa = (int)db.currentEmpresa.i_unique,
                fkInicial = null,
                fkTipo = Convert.ToInt32(mdl._fkTipo),
                nuAno = Convert.ToInt32(mdl.nuAno),
                nuMes = Convert.ToInt32(mdl.nuMes),
                nuParcela = 1,                
                nuTotParcelas = tot_parcelas,
                vrValor = primValor
            }));

            if (tot_parcelas > 1)
            {
                var dt = new DateTime(Convert.ToInt32(mdl.nuAno), Convert.ToInt32(mdl.nuMes), 1);

                for (int i = 2; i <= tot_parcelas; i++)
                {
                    dt = dt.AddMonths(1);

                    db.Insert(new LancamentosCC
                    {
                        fkCartao = (int)t_cart.i_unique,
                        bRecorrente = false,
                        dtLanc = DateTime.Now,
                        fkEmpresa = (int)db.currentEmpresa.i_unique,
                        fkInicial = fkInicial,
                        fkTipo = Convert.ToInt32(mdl._fkTipo),
                        nuAno = dt.Year,
                        nuMes = dt.Month,
                        nuParcela = i,
                        nuTotParcelas = tot_parcelas,
                        vrValor = vrDivValor
                    });
                }
            }
            
            return Ok();
        }

        [HttpPut]
        public IHttpActionResult Put(DtoEmissoraLancCC mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var lanc = db.LancamentosCC.FirstOrDefault(y => y.id == mdl.id);

            if (lanc != null)
            {
                var t_cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == lanc.fkCartao);

                if (t_cart != null)
                {
                    t_cart.stCodigoFOPA = mdl.stFOPA;
                    lanc.vrValor = new money().getNumericValue(mdl.vrValor);

                    db.Update(lanc);
                    db.Update(t_cart);
                }
            }

            return Ok();
        }
    }
}
