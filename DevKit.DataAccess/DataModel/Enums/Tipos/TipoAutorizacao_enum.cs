using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
	public class EnumTipoAutorizacao
	{
		public List<BaseComboResponse> itens = new List<BaseComboResponse>();

		public EnumTipoAutorizacao()
		{
			itens.Add(new BaseComboResponse() { id = 1, stName = "Diária" });
			itens.Add(new BaseComboResponse() { id = 2, stName = "Materiais" });
            itens.Add(new BaseComboResponse() { id = 3, stName = "Medicamentos" });
            itens.Add(new BaseComboResponse() { id = 4, stName = "Não médicos" });
            itens.Add(new BaseComboResponse() { id = 5, stName = "OPME" });
            itens.Add(new BaseComboResponse() { id = 6, stName = "Pacote Serviços" });
		}

		public BaseComboResponse Get(long _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
