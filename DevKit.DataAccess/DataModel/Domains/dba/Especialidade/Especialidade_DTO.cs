using System.Collections.Generic;

namespace DataModel
{
	public partial class Especialidade
    {
        public object anexedEntity;

        public string   sqtdCredenciados = "",
                        updateCommand = "";
	}

    public class EspecialidadeReport
    {
        public int count = 0;
        public List<Especialidade> results = new List<Especialidade>();
    }

}
