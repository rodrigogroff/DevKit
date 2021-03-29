
namespace Master.Data.Database
{
    public class Empresa
    {
        #region - code - 
        public long id { get; set; }
        public long? nuEmpresa { get; set; }
        public string stCNPJ { get; set; }
        public string stFantasia { get; set; }
        public string stSocial { get; set; }
        public string stEndereco { get; set; }
        public string stCidade { get; set; }
        public string stEstado { get; set; }
        public string stCEP { get; set; }
        public string stTelefone { get; set; }
        public long? nuParcelas { get; set; }
        public bool? bBlocked { get; set; }
        public long? fkAdmin { get; set; }
        public string stContaDeb { get; set; }
        public long? vrMensalidade { get; set; }
        public long? nuPctValor { get; set; }
        public long? vrTransacao { get; set; }
        public long? vrMinimo { get; set; }
        public long? nuFranquiaTrans { get; set; }
        public long? nuPeriodoFat { get; set; }
        public long? nuDiaVenc { get; set; }
        public string stBancoFat { get; set; }
        public long? vrCartaoAtivo { get; set; }
        public bool? bIsentoFat { get; set; }
        public string stObs { get; set; }
        public string stHomepage { get; set; }
        public long? nuDiaFech { get; set; }
        public string stHoraFech { get; set; }
        public bool? bConvenioSaldo { get; set; }
        public long? fkParceiro { get; set; }
        public string stEmailPlastico { get; set; }
        public bool? bContaCorrenteAssociado { get; set; }
        #endregion
    }
}
