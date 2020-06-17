using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class AssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var empresa = Request.GetQueryStringValue("empresa");
            var matricula = Request.GetQueryStringValue("matricula");
            var vencimento = Request.GetQueryStringValue("vencimento");

            if (empresa.Length < 6) empresa = empresa.PadLeft(6, '0');
            if (matricula.Length < 6) matricula = matricula.PadLeft(6, '0');

            var acesso = Request.GetQueryStringValue("acesso");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            T_Cartao cartDigitado = null;

            // busca associado

            var associados = (from e in db.T_Cartao
                              where e.st_empresa == empresa
                              where e.st_matricula == matricula
                              where e.st_venctoCartao == vencimento
                              select e).
                              ToList();
                             
            if (associados == null || associados.Count() == 0) 
                return BadRequest();

            var tEmpresa = (from e in db.T_Empresa
                            where e.st_empresa == empresa
                            select e).
                            FirstOrDefault();

            if (tEmpresa == null)
                return BadRequest();

            // busca dados proprietario

            var titular = associados.FirstOrDefault(y => y.st_titularidade == "01");

            cartDigitado = titular;

            var fkdp = titular.fk_dadosProprietario;

            var dadosProprietario = (from e in db.T_Proprietario where e.i_unique == fkdp select e).FirstOrDefault();

            var nome = dadosProprietario.st_nome;

            var codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                           matricula,
                                                           titular.st_titularidade,
                                                           titular.nu_viaCartao,
                                                           dadosProprietario.st_cpf);

            // verificação de código de acesso

            if (acesso != codAcessoCalc)
            {
                // pode ser dependente

                var lstCartoesDependentes = (from e in db.T_Cartao
                                             where e.st_empresa == empresa
                                             where e.st_matricula == matricula
                                             where e.i_unique != titular.i_unique
                                             select e).
                                             ToList();

                var found = false;

                foreach (var cartDep in lstCartoesDependentes)
                {
                    codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                           matricula,
                                                           cartDep.st_titularidade,
                                                           cartDep.nu_viaCartao,
                                                           dadosProprietario.st_cpf);

                    if (acesso == codAcessoCalc)
                    {
                        found = true;
                        nome = db.T_Dependente.Where(y => y.fk_proprietario == fkdp).FirstOrDefault()?.st_nome;
                        cartDigitado = cartDep;
                        break;
                    }
                }

                if (!found)
                    return BadRequest();
            }

            long dispMensal = 0, dispTotal = 0;

            new SaldoDisponivel().
                Obter(db, cartDigitado, ref dispMensal, ref dispTotal);

            var lstParcelas = new List<string>();

            var mon = new money();
            
            int it = 1, totParc = 0;

            foreach (var item in (from e in db.T_Parcelas
                                  where e.fk_cartao == cartDigitado.i_unique
                                  where e.nu_parcela >= 1
                                  orderby e.nu_parcela, e.dt_inclusao
                                  select e).
                                  ToList())
            {

                var ltr = (from e in db.LOG_Transacoes
                           where e.i_unique == item.fk_log_transacoes
                           select e).
                           FirstOrDefault();

                if (ltr != null)
                if (ltr.tg_confirmada != null)
                {
                    if (ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada)
                    {
                        totParc += (int)item.vr_valor;
                        //  lstParcelas.Add("Parcela " + item.nu_parcela + " -> R$ " + mon.setMoneyFormat((long)item.vr_valor));
                    }

                    if (it != item.nu_parcela)
                    {
                        lstParcelas.Add("Total " + it + " : R$ " + mon.setMoneyFormat((long)totParc));
                        it = (int)item.nu_parcela;
                    }
                }
            }

            if (dadosProprietario.st_email != null)
                if (!dadosProprietario.st_email.Contains("@"))
                {
                    dadosProprietario.st_email = "";

                    db.Update(dadosProprietario);
                }                    

            return Ok(new
            {
                count = 0,
                results = new List<Associado>
                {
                    new Associado
                    {
                        id = cartDigitado.i_unique.ToString(),
                        nome = nome,
                        dispMensal = mon.setMoneyFormat (dispMensal),
                        dispTotal = mon.setMoneyFormat (dispTotal),
                        dispExtra = mon.setMoneyFormat ((long)cartDigitado.vr_extraCota),
                        maxParcelasEmpresa = tEmpresa.nu_parcelas.ToString(),
                        bloqueado = titular.tg_status.ToString() == CartaoStatus.Bloqueado ? true : false,
                        lstParcelas = lstParcelas,
                        email = dadosProprietario.st_email
                    }
                }
            });
        }
    }
}
