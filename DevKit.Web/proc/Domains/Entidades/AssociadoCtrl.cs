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

            // busca associado

            var associado = RestoreTimerCache("associadoEMV", empresa + matricula + vencimento, 1) as T_Cartao;

            if (associado == null)
            {
                associado = (from e in db.T_Cartao
                             where e.st_empresa == empresa
                             where e.st_matricula == matricula
                             where e.st_venctoCartao == vencimento
                             select e).
                             FirstOrDefault();

                if (associado == null)
                    return BadRequest();

                BackupCache(associado);
            }

            var tEmpresa = RestoreTimerCache("empresa", associado.st_empresa, 1) as T_Empresa;

            if (tEmpresa == null)
            {
                tEmpresa = (from e in db.T_Empresa
                           where e.st_empresa == associado.st_empresa
                           select e).
                           FirstOrDefault();

                if (tEmpresa == null)
                    return BadRequest();

                BackupCache(tEmpresa);
            }

            // busca dados proprietario

            var dadosProprietario = (from e in db.T_Proprietario
                                     where e.i_unique == associado.fk_dadosProprietario
                                     select e).
                                     FirstOrDefault();

            

            var codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                           matricula,
                                                           associado.st_titularidade,
                                                           associado.nu_viaCartao,
                                                           dadosProprietario.st_cpf);

            // verificação de código de acesso

            if (acesso != codAcessoCalc)
            {
                // pode ser dependente

                var lstCartoesDependentes = (from e in db.T_Cartao
                                             where e.st_empresa == empresa
                                             where e.st_matricula == matricula
                                             where e.i_unique != associado.i_unique
                                             select e).
                                             ToList();

                var found = false;

                foreach (var cartDep in lstCartoesDependentes)
                {
                    codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                           matricula,
                                                           cartDep.st_titularidade,
                                                           associado.nu_viaCartao,
                                                           dadosProprietario.st_cpf);

                    if (acesso == codAcessoCalc)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return BadRequest();
            }

            long dispMensal = 0, dispTotal = 0;

            new SaldoDisponivel().
                Obter(db, associado, ref dispMensal, ref dispTotal);

            var lstParcelas = new List<string>();

            var mon = new money();
            
            int it = 1, totParc = 0;

            foreach (var item in (from e in db.T_Parcelas
                                  where e.fk_cartao == associado.i_unique
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
                        id = associado.i_unique.ToString(),
                        nome = dadosProprietario.st_nome,
                        dispMensal = mon.setMoneyFormat (dispMensal),
                        dispTotal = mon.setMoneyFormat (dispTotal),
                        dispExtra = mon.setMoneyFormat ((long)associado.vr_extraCota),
                        maxParcelasEmpresa = tEmpresa.nu_parcelas.ToString(),
                        bloqueado = associado.tg_status == '1' ? true : false,
                        lstParcelas = lstParcelas,
                        email = dadosProprietario.st_email
                    }
                }
            });
        }
    }
}
