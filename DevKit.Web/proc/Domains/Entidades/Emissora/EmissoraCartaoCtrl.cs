using LinqToDB;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class CartaoListagemDTO
    {
        public string   id,
                        associado, 
                        situacao,
                        matricula,
                        cartao, 
                        cpf, 
                        tit, 
                        dispM, 
                        limM, 
                        dispT, 
                        limT;             
    }

    public class CartaoDTO
    {
        public string id,
            matricula,
            nome, cpf, dtNasc, limMes, limTot, banco, bancoAg, bancoCta, tel, email;

        /*
         * $scope.mat_fail = invalidCheck($scope.viewModel.matricula);
                $scope.nome_fail = invalidCheck($scope.viewModel.nome);
                $scope.cpf_fail = invalidCheck($scope.viewModel.cpf);
                $scope.dtNasc_fail = invalidCheck($scope.viewModel.dtNasc);
                $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
                $scope.limTot_fail = invalidCheck($scope.viewModel.limTot);
                $scope.banco_fail = invalidCheck($scope.viewModel.banco);
                $scope.bancoAg_fail = invalidCheck($scope.viewModel.bancoAg);
                $scope.bancoCta_fail = invalidCheck($scope.viewModel.bancoCta);
                $scope.tel_fail = invalidCheck($scope.viewModel.tel);
                $scope.email_fail = invalidCheck($scope.viewModel.email);
                */
                        
    }

    public class EmissoraCartaoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var matricula = Request.GetQueryStringValue("matricula");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Cartao
                         where e.st_empresa == userLoggedEmpresa
                         select e);

            if (matricula != null && matricula != "")
            {
                query = (from e in query
                         where e.st_matricula == matricula.PadLeft(6, '0')
                         select e);
            }

            var res = new List<CartaoListagemDTO>();

            query = (from e in query
                     join associado in db.T_Proprietario on e.fk_dadosProprietario equals (int)associado.i_unique
                     orderby associado.st_nome
                     select e);

            var calcAcesso = new CodigoAcesso();

            var sd = new SaldoDisponivel();
            var mon = new money();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var assoc = (from e in db.T_Proprietario
                             where e.i_unique == item.fk_dadosProprietario
                             select e).
                             FirstOrDefault();

                long dispM = 0, dispT = 0;

                sd.Obter(db, item, ref dispM, ref dispT);

                if (assoc != null)
                {
                    var codAcessoCalc = calcAcesso.Obter(item.st_empresa,
                                                           item.st_matricula,
                                                           item.st_titularidade,
                                                           item.nu_viaCartao,
                                                           assoc.st_cpf);

                    res.Add(new CartaoListagemDTO
                    {
                        id = item.i_unique.ToString(),
                        associado = assoc.st_nome,
                        cartao = item.st_empresa + "." +
                                 item.st_matricula + "." +
                                 codAcessoCalc + "." +
                                 item.st_venctoCartao,
                        cpf = assoc.st_cpf,
                        situacao = "",
                        matricula = item.st_matricula,
                        tit = item.st_titularidade,
                        dispM = mon.setMoneyFormat(dispM),
                        dispT = mon.setMoneyFormat(dispT),
                        limM = mon.setMoneyFormat((long)item.vr_limiteMensal),
                        limT = mon.setMoneyFormat((long)item.vr_limiteTotal),
                    });
                }
            }

            return Ok(new { count = query.Count(), results = res });
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var cart = (from e in db.T_Cartao
                       where e.i_unique == id
                       select e).
                       FirstOrDefault();

            if (cart == null)
                return StatusCode(HttpStatusCode.NotFound);

            var assoc = (from e in db.T_Proprietario
                         where e.i_unique == cart.fk_dadosProprietario
                         select e).
                         FirstOrDefault();

            return Ok(new CartaoDTO
            {
                id = id.ToString(),
                nome = assoc.st_nome,
            });
        }

        [HttpPost]
        public IHttpActionResult Post(CartaoDTO mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var cart = new T_Cartao();

            cart.st_empresa = userLoggedEmpresa;
            cart.st_matricula = mdl.matricula;

            //if (mdl.limMes.Contains )

            /*
             * 
             * $scope.mat_fail = invalidCheck($scope.viewModel.matricula);
                $scope.nome_fail = invalidCheck($scope.viewModel.nome);
                $scope.cpf_fail = invalidCheck($scope.viewModel.cpf);
                $scope.dtNasc_fail = invalidCheck($scope.viewModel.dtNasc);
                $scope.limMes_fail = invalidCheck($scope.viewModel.limMes);
                $scope.limTot_fail = invalidCheck($scope.viewModel.limTot);
                $scope.banco_fail = invalidCheck($scope.viewModel.banco);
                $scope.bancoAg_fail = invalidCheck($scope.viewModel.bancoAg);
                $scope.bancoCta_fail = invalidCheck($scope.viewModel.bancoCta);
                $scope.tel_fail = invalidCheck($scope.viewModel.tel);
                $scope.email_fail = invalidCheck($scope.viewModel.email);
                
                */

        db.Insert(cart);

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult Put(CartaoDTO mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var cart = (from e in db.T_Cartao
                             where e.i_unique == Convert.ToInt32(mdl.id)
                             select e).
                             FirstOrDefault();

            db.Update(cart);

            return Ok();
        }
    }
}
