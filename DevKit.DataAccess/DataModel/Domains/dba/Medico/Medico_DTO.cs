using System.Collections.Generic;

namespace DataModel
{
	public partial class Medico
    {
        public object anexedEntity;

        public string   sfkEspecialidade,
                        updateCommand = "";
	}

    public class MedicoReport
    {
        public int count = 0;
        public List<Medico> results = new List<Medico>();
    }

}
