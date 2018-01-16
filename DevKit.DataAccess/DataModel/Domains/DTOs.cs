using System.Collections.Generic;

namespace DataModel
{
    public class BaseFilter
    {
        public long? fkEmpresa;
        public int skip, take;
        public string busca;
    }

    public class BaseComboResponse
    {
        public long id;
        public string stName;
    }

    public class ComboReport
    {
        public int count = 0;
        public List<BaseComboResponse> results = new List<BaseComboResponse>();
    }
}
