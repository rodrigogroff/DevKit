using Master.Data.Const;
using Master.Data.Database;
using Master.Repository;
using Npgsql;
using System.Linq;

namespace Master.Service
{
    public class SrvAssociadoSaldo
    {
        public SrvAssociadoSaldo ( NpgsqlConnection db, 
                                    ICartaoDapperRepository cartaoRepository,
                                    IParcelaDapperRepository parcelaRepository,
                                    ILogTransacaoDapperRepository logTransacaoRepository,
                                    Cartao cart,
                                    ref long dispMensal, 
                                    ref long dispTotal, 
                                    ref long vrUtilizadoAtual )
        {
            dispMensal = (long)cart.vrLimiteMensal;
            dispTotal = (long)cart.vrLimiteTotal;

            if (cart.vrCotaExtra > 0)
            {
                dispMensal += (long)cart.vrCotaExtra;
                dispTotal += (long)cart.vrCotaExtra;
            }

            var lstCartoes = cartaoRepository.GetListaCartoes(db, cart.fkEmpresa, cart.nuMatricula);                                

            var lstParcelasTotais = parcelaRepository.GetParcelasDeCartao(db, 1, lstCartoes);

            var lstTransacoesTotais = logTransacaoRepository.GetLogTransacaoLista(db, lstParcelasTotais.Select(y => (long)y.fkLogTransacao).ToList());

            foreach (var parc in lstParcelasTotais)
            {
                var transacao = lstTransacoesTotais.FirstOrDefault(y => (long)y.id == parc.fkLogTransacao);

                if (transacao == null)
                    continue;

                if (parc.nuParcela == 1 && transacao.nuConfirmada == TipoConfirmacao.Confirmada)
                {
                    if (parc.nuParcela == 1)
                        dispMensal -= (int)parc.vrValor;

                    vrUtilizadoAtual += (long)parc.vrValor;
                    dispTotal -= (int)parc.vrValor;
                }
            }

            if (dispMensal < 0) dispMensal = 0;
            if (dispTotal < 0) dispTotal = 0;
        }
    }
}
