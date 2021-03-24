
namespace Master.Data.Database
{
    public class Loja
    {
        #region - code - 
        public long id { get; set; }
        public string stCNPJ { get; set; }
        public string stNome { get; set; }
        public string stSocial { get; set; }
        public string stEndereco { get; set; }
        public string stEnderecoInst { get; set; }
        public string stInscEst { get; set; }
        public string stCidade { get; set; }
        public string stEstado { get; set; }
        public string stCEP { get; set; }
        public string stTelefone { get; set; }
        public string stFax { get; set; }
        public string stContato { get; set; }
        public long? vrMensalidade { get; set; }
        public string stContaDeb { get; set; }
        public string stObs { get; set; }
        public string stLoja { get; set; }
        public bool? bBlocked { get; set; }
        public long? nuPctValor { get; set; }
        public long? vrTransacao { get; set; }
        public long? vrMinimo { get; set; }
        public long? nuFranquia { get; set; }
        public long? nuPeriodoFat { get; set; }
        public long? nuDiaVenc { get; set; }
        public long? nuTipoCob { get; set; }
        public long? nuBancoFat { get; set; }
        public bool? bIsentoFat { get; set; }
        public string stSenha { get; set; }
        public bool? bCancel { get; set; }
        public bool? bPortalSenha { get; set; }
        public string stEmail { get; set; }
        public string stCelular { get; set; }
        public string stBanco { get; set; }
        public string stAgencia { get; set; }
        public string stConta { get; set; }
        public long? fkBanco { get; set; }
        public string stCPFResp { get; set; }
        public string stDataResp { get; set; }
        #endregion
    }
}
