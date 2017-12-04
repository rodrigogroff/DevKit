using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class OrdemEmissorManutCartoesItem
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class OrdemEmissorManutCartoesReport
    {
        public int count;
        public List<OrdemEmissorManutCartoesItem> results;
    }

    public class EnumOrdemEmissorManutCartoes
    {
        public List<OrdemEmissorManutCartoesItem> lst = new List<OrdemEmissorManutCartoesItem>();

        public const long nomeAssociado = 1, matricula = 2;

        public EnumOrdemEmissorManutCartoes()
        {
            lst.Add(new OrdemEmissorManutCartoesItem() { id = nomeAssociado, stName = "Nome associado" });
            lst.Add(new OrdemEmissorManutCartoesItem() { id = matricula, stName = "Matricula" });            
        }

        public OrdemEmissorManutCartoesItem Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

