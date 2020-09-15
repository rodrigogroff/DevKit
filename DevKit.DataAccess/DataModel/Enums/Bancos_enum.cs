using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class Banco
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class BancoReport
    {
        public int count;
        public List<Banco> results;
    }

    public class EnumBancos
    {
        public List<Banco> lst = new List<Banco>();

        public EnumBancos()
        {
            int t = 1;

            lst.Add(new Banco() { id = t++, stName = "001 – Banco do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "033 – Banco Santander (Brasil) S.A." });
            lst.Add(new Banco() { id = t++, stName = "104 – Caixa Econômica Federal" });
            lst.Add(new Banco() { id = t++, stName = "237 – Banco Bradesco S.A." });
            lst.Add(new Banco() { id = t++, stName = "341 – Banco Itaú S.A." });
            lst.Add(new Banco() { id = t++, stName = "356 – Banco Real S.A. (antigo)" });
            lst.Add(new Banco() { id = t++, stName = "389 – Banco Mercantil do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "399 – HSBC Bank Brasil S.A. – Banco Múltiplo" });
            lst.Add(new Banco() { id = t++, stName = "422 – Banco Safra S.A." });
            lst.Add(new Banco() { id = t++, stName = "453 – Banco Rural S.A." });
            lst.Add(new Banco() { id = t++, stName = "633 – Banco Rendimento S.A." });
            lst.Add(new Banco() { id = t++, stName = "652 – Itaú Unibanco Holding S.A." });
            lst.Add(new Banco() { id = t++, stName = "745 – Banco Citibank S.A." });
            lst.Add(new Banco() { id = t++, stName = "246 – Banco ABC Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "025 – Banco Alfa S.A." });
            lst.Add(new Banco() { id = t++, stName = "641 – Banco Alvorada S.A." });
            lst.Add(new Banco() { id = t++, stName = "029 – Banco Banerj S.A." });
            lst.Add(new Banco() { id = t++, stName = "038 – Banco Banestado S.A." });
            lst.Add(new Banco() { id = t++, stName = "000 – Banco Bankpar S.A." });
            lst.Add(new Banco() { id = t++, stName = "740 – Banco Barclays S.A." });
            lst.Add(new Banco() { id = t++, stName = "107 – Banco BBM S.A." });
            lst.Add(new Banco() { id = t++, stName = "031 – Banco Beg S.A." });
            lst.Add(new Banco() { id = t++, stName = "096 – Banco BM&F de Serviços de Liquidação e Custódia S.A" });
            lst.Add(new Banco() { id = t++, stName = "318 – Banco BMG S.A." });
            lst.Add(new Banco() { id = t++, stName = "752 – Banco BNP Paribas Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "248 – Banco Boavista Interatlântico S.A." });
            lst.Add(new Banco() { id = t++, stName = "036 – Banco Bradesco BBI S.A." });
            lst.Add(new Banco() { id = t++, stName = "204 – Banco Bradesco Cartões S.A." });
            lst.Add(new Banco() { id = t++, stName = "225 – Banco Brascan S.A." });
            lst.Add(new Banco() { id = t++, stName = "044 – Banco BVA S.A." });
            lst.Add(new Banco() { id = t++, stName = "263 – Banco Cacique S.A." });
            lst.Add(new Banco() { id = t++, stName = "473 – Banco Caixa Geral – Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "222 – Banco Calyon Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "040 – Banco Cargill S.A." });
            lst.Add(new Banco() { id = t++, stName = "M08 – Banco Citicard S.A." });
            lst.Add(new Banco() { id = t++, stName = "M19 – Banco CNH Capital S.A." });
            lst.Add(new Banco() { id = t++, stName = "215 – Banco Comercial e de Investimento Sudameris S.A." });
            lst.Add(new Banco() { id = t++, stName = "756 – Banco Cooperativo do Brasil S.A. – BANCOOB" });
            lst.Add(new Banco() { id = t++, stName = "748 – Banco Cooperativo Sicredi S.A." });
            lst.Add(new Banco() { id = t++, stName = "505 – Banco Credit Suisse (Brasil) S.A." });
            lst.Add(new Banco() { id = t++, stName = "229 – Banco Cruzeiro do Sul S.A." });
            lst.Add(new Banco() { id = t++, stName = "003 – Banco da Amazônia S.A." });
            lst.Add(new Banco() { id = t++, stName = "083-3 – Banco da China Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "707 – Banco Daycoval S.A." });
            lst.Add(new Banco() { id = t++, stName = "M06 – Banco de Lage Landen Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "024 – Banco de Pernambuco S.A. – BANDEPE" });
            lst.Add(new Banco() { id = t++, stName = "456 – Banco de Tokyo-Mitsubishi UFJ Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "214 – Banco Dibens S.A." });
            lst.Add(new Banco() { id = t++, stName = "047 – Banco do Estado de Sergipe S.A." });
            lst.Add(new Banco() { id = t++, stName = "037 – Banco do Estado do Pará S.A." });
            lst.Add(new Banco() { id = t++, stName = "041 – Banco do Estado do Rio Grande do Sul S.A." });
            lst.Add(new Banco() { id = t++, stName = "004 – Banco do Nordeste do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "265 – Banco Fator S.A." });
            lst.Add(new Banco() { id = t++, stName = "M03 – Banco Fiat S.A." });
            lst.Add(new Banco() { id = t++, stName = "224 – Banco Fibra S.A." });
            lst.Add(new Banco() { id = t++, stName = "626 – Banco Ficsa S.A." });
            lst.Add(new Banco() { id = t++, stName = "394 – Banco Finasa BMC S.A." });
            lst.Add(new Banco() { id = t++, stName = "M18 – Banco Ford S.A." });
            lst.Add(new Banco() { id = t++, stName = "233 – Banco GE Capital S.A." });
            lst.Add(new Banco() { id = t++, stName = "734 – Banco Gerdau S.A." });
            lst.Add(new Banco() { id = t++, stName = "M07 – Banco GMAC S.A." });
            lst.Add(new Banco() { id = t++, stName = "612 – Banco Guanabara S.A." });
            lst.Add(new Banco() { id = t++, stName = "M22 – Banco Honda S.A." });
            lst.Add(new Banco() { id = t++, stName = "063 – Banco Ibi S.A. Banco Múltiplo" });
            lst.Add(new Banco() { id = t++, stName = "M11 – Banco IBM S.A." });
            lst.Add(new Banco() { id = t++, stName = "604 – Banco Industrial do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "320 – Banco Industrial e Comercial S.A." });
            lst.Add(new Banco() { id = t++, stName = "653 – Banco Indusval S.A." });
            lst.Add(new Banco() { id = t++, stName = "630 – Banco Intercap S.A." });
            lst.Add(new Banco() { id = t++, stName = "249 – Banco Investcred Unibanco S.A." });
            lst.Add(new Banco() { id = t++, stName = "184 – Banco Itaú BBA S.A." });
            lst.Add(new Banco() { id = t++, stName = "479 – Banco ItaúBank S.A" });
            lst.Add(new Banco() { id = t++, stName = "M09 – Banco Itaucred Financiamentos S.A." });
            lst.Add(new Banco() { id = t++, stName = "376 – Banco J. P. Morgan S.A." });
            lst.Add(new Banco() { id = t++, stName = "074 – Banco J. Safra S.A." });
            lst.Add(new Banco() { id = t++, stName = "217 – Banco John Deere S.A." });
            lst.Add(new Banco() { id = t++, stName = "065 – Banco Lemon S.A." });
            lst.Add(new Banco() { id = t++, stName = "600 – Banco Luso Brasileiro S.A." });
            lst.Add(new Banco() { id = t++, stName = "755 – Banco Merrill Lynch de Investimentos S.A." });
            lst.Add(new Banco() { id = t++, stName = "746 – Banco Modal S.A." });
            lst.Add(new Banco() { id = t++, stName = "151 – Banco Nossa Caixa S.A." });
            lst.Add(new Banco() { id = t++, stName = "045 – Banco Opportunity S.A." });
            lst.Add(new Banco() { id = t++, stName = "623 – Banco Panamericano S.A." });
            lst.Add(new Banco() { id = t++, stName = "611 – Banco Paulista S.A." });
            lst.Add(new Banco() { id = t++, stName = "643 – Banco Pine S.A." });
            lst.Add(new Banco() { id = t++, stName = "638 – Banco Prosper S.A." });
            lst.Add(new Banco() { id = t++, stName = "747 – Banco Rabobank International Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "M16 – Banco Rodobens S.A." });
            lst.Add(new Banco() { id = t++, stName = "072 – Banco Rural Mais S.A." });
            lst.Add(new Banco() { id = t++, stName = "250 – Banco Schahin S.A." });
            lst.Add(new Banco() { id = t++, stName = "749 – Banco Simples S.A." });
            lst.Add(new Banco() { id = t++, stName = "366 – Banco Société Générale Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "637 – Banco Sofisa S.A." });
            lst.Add(new Banco() { id = t++, stName = "464 – Banco Sumitomo Mitsui Brasileiro S.A." });
            lst.Add(new Banco() { id = t++, stName = "082-5 – Banco Topázio S.A." });
            lst.Add(new Banco() { id = t++, stName = "M20 – Banco Toyota do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "634 – Banco Triângulo S.A." });
            lst.Add(new Banco() { id = t++, stName = "208 – Banco UBS Pactual S.A." });
            lst.Add(new Banco() { id = t++, stName = "M14 – Banco Volkswagen S.A." });
            lst.Add(new Banco() { id = t++, stName = "655 – Banco Votorantim S.A." });
            lst.Add(new Banco() { id = t++, stName = "610 – Banco VR S.A." });
            lst.Add(new Banco() { id = t++, stName = "370 – Banco WestLB do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "021 – BANESTES S.A. Banco do Estado do Espírito Santo" });
            lst.Add(new Banco() { id = t++, stName = "719 – Banif-Banco Internacional do Funchal (Brasil)S.A." });
            lst.Add(new Banco() { id = t++, stName = "073 – BB Banco Popular do Brasil S.A." });
            lst.Add(new Banco() { id = t++, stName = "078 – BES Investimento do Brasil S.A.-Banco de Investimento" });
            lst.Add(new Banco() { id = t++, stName = "069 – BPN Brasil Banco Múltiplo S.A." });
            lst.Add(new Banco() { id = t++, stName = "070 – BRB – Banco de Brasília S.A." });
            lst.Add(new Banco() { id = t++, stName = "477 – Citibank N.A." });
            lst.Add(new Banco() { id = t++, stName = "081-7 – Concórdia Banco S.A." });
            lst.Add(new Banco() { id = t++, stName = "487 – Deutsche Bank S.A. – Banco Alemão" });
            lst.Add(new Banco() { id = t++, stName = "751 – Dresdner Bank Brasil S.A. – Banco Múltiplo" });
            lst.Add(new Banco() { id = t++, stName = "062 – Hipercard Banco Múltiplo S.A." });
            lst.Add(new Banco() { id = t++, stName = "492 – ING Bank N.V." });
            lst.Add(new Banco() { id = t++, stName = "488 – JPMorgan Chase Bank" });
            lst.Add(new Banco() { id = t++, stName = "409 – UNIBANCO – União de Bancos Brasileiros S.A." });
            lst.Add(new Banco() { id = t++, stName = "230 – Unicard Banco Múltiplo S.A." });
        }

        public Banco Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

