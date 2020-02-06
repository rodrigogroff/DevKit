using DataModel;
using System.Linq;
using LinqToDB;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class SaldoDisponivel
    {
        public long vrUtilizadoAtual = 0;

        public List<T_Parcela> lstParcelas;
        public List<LOG_Transaco> lstTrans;

        public void ObterEmLista(AutorizadorCNDB db, T_Cartao cart, ref long dispMensal, ref long dispTotal)
        {
            dispMensal = (long)cart.vr_limiteMensal;
            dispTotal = (long)cart.vr_limiteTotal;

            if (cart.vr_extraCota > 0)
            {
                dispMensal += (long)cart.vr_extraCota;
                dispTotal += (long)cart.vr_extraCota;
            }

            var lstCartoes = (from e in db.T_Cartao
                              where e.st_empresa == cart.st_empresa
                              where e.st_matricula == cart.st_matricula
                              select (int)e.i_unique).
                              ToList();

            var _lstParcelas = (from e in lstParcelas
                               where lstCartoes.Contains((int)e.fk_cartao)
                               where e.nu_parcela == 1
                               select e).
                               ToList();

            foreach (var parc in _lstParcelas)
            {
                var transacao = (from e in lstTrans
                                 where e.i_unique == parc.fk_log_transacoes
                                 select e).
                                 FirstOrDefault();

                if (transacao == null)
                    continue;

                if (transacao.tg_confirmada.ToString() == TipoConfirmacao.Confirmada ||
                    transacao.tg_confirmada.ToString() == TipoConfirmacao.Pendente)
                {
                    dispMensal -= (int)parc.vr_valor;
                    vrUtilizadoAtual += (long)parc.vr_valor;
                }
            }

            var lstParcelasTotais = (from e in lstParcelas
                                     where lstCartoes.Contains((int)e.fk_cartao)
                                     where e.nu_parcela >= 1
                                     select e).
                                     ToList();

            foreach (var parc in lstParcelasTotais)
            {
                var transacao = (from e in lstTrans
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

        public void Obter (AutorizadorCNDB db, T_Cartao cart, ref long dispMensal, ref long dispTotal)
        {
            dispMensal = (long)cart.vr_limiteMensal;
            dispTotal = (long)cart.vr_limiteTotal;

            if (cart.vr_extraCota > 0)
            {
                dispMensal += (long)cart.vr_extraCota;
                dispTotal += (long)cart.vr_extraCota;
            }
                
            var lstCartoes = (from e in db.T_Cartao
                              where e.st_empresa == cart.st_empresa
                              where e.st_matricula == cart.st_matricula
                              select (int)e.i_unique).
                              ToList();

            var lstParcelasTotais = (from e in db.T_Parcelas
                                     where lstCartoes.Contains((int)e.fk_cartao)
                                     where e.nu_parcela >= 1
                                     select e).
                                     ToList();

            var lst_log = lstParcelasTotais.
                            Where ( y=> y.fk_log_transacoes != null).
                            Select ( y=> (int)y.fk_log_transacoes).
                            Distinct().
                            ToList();

            var lstLogTrans = (from e in db.LOG_Transacoes
                               where lst_log.Contains((int)e.i_unique)
                               select e).
                               ToList();

            foreach (var parc in lstParcelasTotais.Where ( y=> y.nu_parcela == 1))
            {
                var transacao = (from e in lstLogTrans
                                 where e.i_unique == parc.fk_log_transacoes
                                 select e).
                                 FirstOrDefault();

                if (transacao == null)
                    continue;

                if (transacao.tg_confirmada.ToString() == TipoConfirmacao.Confirmada )
                {
                    dispMensal -= (int) parc.vr_valor;
                    vrUtilizadoAtual += (long) parc.vr_valor;
                }
            }                       

            foreach (var parc in lstParcelasTotais.Where(y => y.nu_parcela >= 1))
            {
                var transacao = (from e in lstLogTrans
                                 where e.i_unique == parc.fk_log_transacoes
                                 select e).
                                 FirstOrDefault();

                if (transacao == null)
                    continue;

                if (transacao.tg_confirmada.ToString() == TipoConfirmacao.Confirmada)
                {
                    dispTotal -= (int)parc.vr_valor;
                }
            }

            if (dispMensal < 0) dispMensal = 0;
            if (dispTotal < 0) dispTotal = 0;
        }
    }
}