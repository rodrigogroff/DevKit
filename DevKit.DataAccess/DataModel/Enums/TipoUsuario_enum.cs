using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class TipoUsuario
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class TipoUsuarioReport
    {
        public int count;
        public List<TipoUsuario> results;
    }

    public class EnumTipoUsuario
    {
        public List<TipoUsuario> lst = new List<TipoUsuario>();

        public EnumTipoUsuario()
        {
            lst.Add(new TipoUsuario() { id = 1, stName = "Admin" });
            lst.Add(new TipoUsuario() { id = 2, stName = "Operador" });
        }

        public TipoUsuario Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

