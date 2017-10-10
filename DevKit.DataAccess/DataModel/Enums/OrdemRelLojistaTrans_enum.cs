using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class OrdemRelLojistaTransItem
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class OrdemRelLojistaTransReport
    {
        public int count;
        public List<OrdemRelLojistaTransItem> results;
    }

    public class EnumOrdemRelLojistaTrans
    {
        public List<OrdemRelLojistaTransItem> lst = new List<OrdemRelLojistaTransItem>();

        public const long data = 1, valor = 2, associado = 3;

        public EnumOrdemRelLojistaTrans()
        {
            lst.Add(new OrdemRelLojistaTransItem() { id = data, stName = "Data" });
            lst.Add(new OrdemRelLojistaTransItem() { id = valor, stName = "Valor" });
            lst.Add(new OrdemRelLojistaTransItem() { id = associado, stName = "Associado" });
        }

        public OrdemRelLojistaTransItem Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

