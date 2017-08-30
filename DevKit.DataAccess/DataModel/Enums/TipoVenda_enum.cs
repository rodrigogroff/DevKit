using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class TipoVenda
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class TipoVendaReport
    {
        public int count;
        public List<TipoVenda> results;
    }

    public class EnumTipoVenda
    {
        public List<TipoVenda> lst = new List<TipoVenda>();

        public const long AoPortador = 1,
                            ComSenha = 2;

        public EnumTipoVenda()
        {
            lst.Add(new TipoVenda() { id = AoPortador, stName = "Ao portador (sem senha)" });
            lst.Add(new TipoVenda() { id = ComSenha, stName = "Autorização com senha" });
        }

        public TipoVenda Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

