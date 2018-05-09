using DataModel;
using SyCrafEngine;
using System;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class Cupom
    {
        public List<string> Cancelamento ( AutorizadorCNDB db, 
                                           T_Cartao cart, 
                                           string nsu_retorno, 
                                           string terminal, 
                                           string nsu, 
                                           LOG_Transaco lTr, 
                                           T_Proprietario prop )
        {
            var mon = new money();
            var cupom = new List<string>();

            cupom.Add("CONVEYNET - CONVÊNIOS");
            cupom.Add("COMPROVANTE CANCELAMENTO VENDA");
            cupom.Add(db.currentLojista.st_nome);
            cupom.Add("CNPJ: " + db.currentLojista.nu_CNPJ);
            cupom.Add(db.currentLojista.st_endereco.Replace("{SE$3}", "") + " " + db.currentLojista.st_cidade + " / " + db.currentLojista.st_estado);
            cupom.Add("Estabelecimento: " + db.currentLojista.st_loja);
            cupom.Add("Cartão: " + cart.st_empresa + cart.st_matricula);
            cupom.Add("Data transação: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            cupom.Add("NSU: " + nsu_retorno);
            cupom.Add("Terminal: " + terminal);
            cupom.Add("NSU trans. original: " + nsu.PadLeft(6, '0'));
            cupom.Add("Data da Trans. original: " + Convert.ToDateTime(lTr.dt_transacao).ToString("dd-MM-yyyy"));
            cupom.Add("Hora Trans. original: " + Convert.ToDateTime(lTr.dt_transacao).ToString("HH:mm:ss"));
            cupom.Add("Valor trans original: R$ " + mon.setMoneyFormat((long)lTr.vr_total));
            cupom.Add(prop.st_nome);
            cupom.Add("*Valor estornado do limite do associado");
            cupom.Add(" ");
            cupom.Add("   ");
            cupom.Add("-------------------------------------------");

            return cupom;
        }
        
        public List<string> Venda ( AutorizadorCNDB db, 
                                    T_Cartao associadoPrincipal, 
                                    T_Proprietario dadosProprietario,
                                    string dataVenda,
                                    string nsu_retorno, 
                                    string terminal, 
                                    int parcelas, 
                                    long valor,
                                    long p1,
                                    long p2,
                                    long p3,
                                    long p4,
                                    long p5,
                                    long p6,
                                    long p7,
                                    long p8,
                                    long p9,
                                    long p10,
                                    long p11,
                                    long p12  )
        {
            var mon = new money();

            var cupom = new List<string>();

            cupom.Add("CONVEYNET - CONVÊNIOS");
            cupom.Add(db.currentLojista.st_nome);
            cupom.Add("CNPJ: " + db.currentLojista.nu_CNPJ);
            cupom.Add(db.currentLojista.st_endereco.Replace("{SE$3}", "") + " " + db.currentLojista.st_cidade + " / " + db.currentLojista.st_estado);
            cupom.Add("Estabelecimento: " + db.currentLojista.st_loja);
            cupom.Add("Cartão: " + associadoPrincipal.st_empresa + associadoPrincipal.st_matricula);
            cupom.Add("Data transação: " + dataVenda);
            cupom.Add("NSU: " + nsu_retorno);
            cupom.Add("Terminal: " + terminal);
            cupom.Add("VALOR TOTAL: R$ " + mon.setMoneyFormat(valor));

            int iParc = 1;

            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p1)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p2)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p3)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p4)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p5)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p6)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p7)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p8)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p9)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p10)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p11)); iParc++;
            if (parcelas >= iParc) cupom.Add("Parcela " + iParc + " R$ " + mon.setMoneyFormat(p12)); iParc++;

            long dispM = 0, dispT = 0;

            new SaldoDisponivel().Obter(db, associadoPrincipal, ref dispM, ref dispT);

            cupom.Add("ASSINATURA/RG_________________________________");
            cupom.Add(dadosProprietario.st_nome);
            cupom.Add("Saldo disponivel no mes: R$" + mon.setMoneyFormat(dispM));
            cupom.Add("Saldo disponivel parcelado: R$" + mon.setMoneyFormat(dispT));
            cupom.Add("");

            return cupom;
        }
    }
}