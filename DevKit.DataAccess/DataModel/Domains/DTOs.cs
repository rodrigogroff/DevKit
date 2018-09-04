using System.Collections.Generic;

namespace DataModel
{
    public class BaseFilter
    {
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

    public class LancDespesa
    {
        public string fkCartao, associado, matricula, valor, saldo;
        public bool falta = false;
    }
}
