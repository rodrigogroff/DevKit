using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
	public class EnumTipoSituacao
	{
		public List<BaseComboResponse> itens = new List<BaseComboResponse>();

		public EnumTipoSituacao()
		{
			itens.Add(new BaseComboResponse() { id = 1, stName = "Habilitado" });
			itens.Add(new BaseComboResponse() { id = 2, stName = "Bloqueado" });
			
            itens.OrderBy(y => y.stName);
		}

		public BaseComboResponse Get(long _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
