
using System;

namespace Master.Data.Database
{
    public class Parcela
    {
        #region - code - 
        public long id { get; set; }
        public long? nuNsu { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkCartao { get; set; }
        public DateTime? dtInclusao { get; set; }
        public long? nuParcela { get; set; }
        public long? vrValor { get; set; }
        public long? nuIndice { get; set; }
        public long? fkLoja { get; set; }
        public long? nuTotParcelas { get; set; }
        public long? fkTerminal { get; set; }
        public long? fkLogTransacao { get; set; }

        #endregion
    }
}
