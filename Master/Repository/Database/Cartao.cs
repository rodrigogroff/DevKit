using System;

namespace Master.Infra.Entity.Database
{
    public class Cartao
    {
        #region - code - 

        public long id { get; set; }
        public long? fkEmpresa { get; set; }
        public long? nuMatricula { get; set; }
        public long? nuTitularidade { get; set; }
        public string stSenha { get; set; }
        public long? nuTipoCartao { get; set; }
        public string stVenctoCartao { get; set; }
        public long? nuStatus { get; set; }
        public long? nuSenhaErrada { get; set; }
        public DateTime? dtInclusao { get; set; }
        public DateTime? dtBloqueio { get; set; }
        public long? nuMotivoBloqueio { get; set; }
        public string stBanco { get; set; }
        public string stAgencia { get; set; }
        public string stConta { get; set; }
        public string stMatExtra { get; set; }
        public string stCelCartao { get; set; }
        public string stCpf { get; set; }
        public string stNome { get; set; }
        public string stEndereco { get; set; }
        public string stNumero { get; set; }
        public string stCompl { get; set; }
        public string stBairro { get; set; }
        public string stEstado { get; set; }
        public string stCidade { get; set; }
        public string stCEP { get; set; }
        public string stDDD { get; set; }
        public string stTelefone { get; set; }
        public DateTime? dtNasc { get; set; }
        public string stEmail { get; set; }
        public long? vrRenda { get; set; }
        public long? nuViaCartao { get; set; }
        public long? vrLimiteTotal { get; set; }
        public long? vrLimiteMensal { get; set; }
        public long? vrLimiteRotativo { get; set; }
        public long? vrCotaExtra { get; set; }
        public long? nuEmitido { get; set; }
        public bool? bConvenioComSaldo { get; set; }
        public long? vrSaldoConvenio { get; set; }
        public DateTime? dtPedidoCartao { get; set; }

        #endregion
    }
}
