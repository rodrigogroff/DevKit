using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
	public class EnumTipoExpedicao
	{
		public List<BaseComboResponse> itens = new List<BaseComboResponse>();

		public EnumTipoExpedicao()
		{
			itens.Add(new BaseComboResponse() { id = 1, stName = "Requerido" });
			itens.Add(new BaseComboResponse() { id = 2, stName = "Em produção" });
            itens.Add(new BaseComboResponse() { id = 3, stName = "Entregue" });

            itens.OrderBy(y => y.stName);
		}

		public BaseComboResponse Get(long _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
