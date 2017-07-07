using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class ContactForm
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class ContactFormReport
    {
        public int count;
        public List<ContactForm> results;
    }

    public class EnumContactForm
    {
        public List<ContactForm> lst = new List<ContactForm>();

        public const long Phone = 1,
                            Email = 2,
                            Visit = 3;

        public EnumContactForm()
        {
            lst.Add(new ContactForm() { id = Phone, stName = "Phone" });
            lst.Add(new ContactForm() { id = Email, stName = "Email" });
            lst.Add(new ContactForm() { id = Visit, stName = "Visit" });
        }

        public ContactForm Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}