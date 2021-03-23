
using System;

namespace Master.Data.Database
{
    public class LogTransacao
    {
        #region - code - 
        public long id { get; set; }
        public long? fkTerminal { get; set; }
        public DateTime? dtTransacao { get; set; }
        public long? nuNsu { get; set; }
        public long? fkEmpresa { get; set; }
        public long? fkCartao { get; set; }
        public long? vrTotal { get; set; }
        public long? nuParcelas { get; set; }
        public long? nuCodErro { get; set; }
        public long? nuConfirmada { get; set; }
        public long? nuNsuOrig { get; set; }
        public long? nuOperacao { get; set; }
        public string stMsg { get; set; }
        public bool? bContabil { get; set; }
        public long? fkLoja { get; set; }
        public string stDoc { get; set; }

        #endregion
    }
}
