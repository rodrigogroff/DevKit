using LinqToDB;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel
{
    public partial class LOG_Fechamento
    {
        [NotMapped]
        public long? fkTerminal { get; set; }
    }
}
