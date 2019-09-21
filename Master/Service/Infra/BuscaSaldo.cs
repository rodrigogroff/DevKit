using Entities.Database;
using Entities.Enums;
using Master.Repository;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Service
{
    public class BuscaSaldo : BaseService
    {
        public BuscaSaldo() { }

        public void SaldoCartao (   IDapperRepository repository,
                                    SqlConnection db, 
                                    T_Cartao cart, 
                                    ref long dispMensal, 
                                    ref long dispTotal, 
                                    ref long vrUtilizadoAtual )
        {
            dispMensal = (long)cart.vr_limiteMensal;
            dispTotal = (long)cart.vr_limiteTotal;

            if (cart.vr_extraCota > 0)
            {
                dispMensal += (long)cart.vr_extraCota;
                dispTotal += (long)cart.vr_extraCota;
            }

            var lstCartoes = repository.ObterListaCartao (db, cart.st_empresa, cart.st_matricula).
                                Select ( y=> (long) y.i_unique).
                                ToList();

            var lstParcelasTotais = repository.ObterListaParcelaDeListaCartaoSuperior(db, lstCartoes, 1);
            var lstTransacoesTotais = repository.ObterListaLogTransacao(db, lstParcelasTotais.Select(y => (long)y.fk_log_transacoes).ToList());

            foreach (var parc in lstParcelasTotais)
            {
                var transacao = lstTransacoesTotais.FirstOrDefault(y => (long)y.i_unique == parc.fk_log_transacoes);

                if (transacao == null)
                    continue;

                if (transacao.tg_confirmada.ToString() == TipoConfirmacao.Confirmada)
                {
                    if (parc.nu_parcela == 1)
                        dispMensal -= (int)parc.vr_valor;

                    vrUtilizadoAtual += (long)parc.vr_valor;
                    dispTotal -= (int)parc.vr_valor;
                }
            }            

            if (dispMensal < 0) dispMensal = 0;
            if (dispTotal < 0) dispTotal = 0;
        }
    }
}
