using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
    public class ContactFormReport
    {
        public int count;
        public List<ComboItem> results;
    }

    public class EnumContactForm
    {
        public List<ComboItem> itens = new List<ComboItem>();

        public const long Phone = 1,
                            Email = 2,
                            Visit = 3;

        public EnumContactForm()
        {
            itens.Add(new ComboItem() { id = 1, stName = "Telefone" });
            itens.Add(new ComboItem() { id = 2, stName = "Email" });
            itens.Add(new ComboItem() { id = 3, stName = "Visita" });
        }

        public ComboItem Get(long _id)
        {
            return itens.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}