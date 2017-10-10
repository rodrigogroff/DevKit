using DataModel;
using System.Linq;
using LinqToDB;

namespace DevKit.Web.Controllers
{
    public class SaldoDisponivel
    {
        public void Obter (AutorizadorCNDB db, T_Cartao cart, ref long dispMensal, ref long dispTotal)
        {
            var lstCartoes = (from e in db.T_Cartao
                              where e.st_empresa == cart.st_empresa
                              where e.st_matricula == cart.st_matricula
                              select (int)e.i_unique).
                              ToList();

            var lstParcelas = (from e in db.T_Parcelas
                               where lstCartoes.Contains((int)e.fk_cartao)
                               where e.nu_parcela == 1
                               select e).
                               ToList();

            foreach (var parc in lstParcelas)
            {
                var transacao = (from e in db.LOG_Transacoes
                                 where e.i_unique == parc.fk_log_transacoes
                                 select e).
                                 FirstOrDefault();

                if (transacao == null)
                    continue;

                if (transacao.tg_confirmada.ToString() == TipoConfirmacao.Confirmada ||
                    transacao.tg_confirmada.ToString() == TipoConfirmacao.Pendente )
                {
                    dispMensal -= (int) parc.vr_valor;
                }
            }

            var lstParcelasTotais = (from e in db.T_Parcelas
                                     where lstCartoes.Contains((int)e.fk_cartao)
                                     where e.nu_parcela >= 1
                                     select e).
                                     ToList();

            foreach (var parc in lstParcelasTotais)
            {
                var transacao = (from e in db.LOG_Transacoes
                                 where e.i_unique == parc.fk_log_transacoes
                                 select e).
                                 FirstOrDefault();

                if (transacao == null)
                    continue;

                var sit = transacao.tg_confirmada.ToString();

                if (sit == TipoConfirmacao.Confirmada ||
                    sit == TipoConfirmacao.Pendente)
                {
                    dispTotal -= (int)parc.vr_valor;
                }
            }

            if (dispMensal < 0) dispMensal = 0;
            if (dispTotal < 0) dispTotal = 0;
        }
    }
}