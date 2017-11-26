using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class Expedicao
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class ExpedicaoReport
    {
        public int count;
        public List<Expedicao> results;
    }

    public class EnumExpedicao
    {
        public List<Expedicao> lst = new List<Expedicao>();

        public EnumExpedicao()
        {
            lst.Add(new Expedicao() { id = 0, stName = "Não expedido" });
            lst.Add(new Expedicao() { id = 1, stName = "Em expedição" });
            lst.Add(new Expedicao() { id = 2, stName = "Expedido" });
        }

        public Expedicao Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

