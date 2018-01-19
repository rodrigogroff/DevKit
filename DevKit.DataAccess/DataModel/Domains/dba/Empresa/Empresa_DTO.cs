using System.Collections.Generic;

namespace DataModel
{
	public partial class Empresa
    {
        public object anexedEntity;

        public string   sqtdCartoes = "",
                        updateCommand = "";
	}

    public class EmpresaReport
    {
        public int count = 0;
        public List<Empresa> results = new List<Empresa>();
    }

}
