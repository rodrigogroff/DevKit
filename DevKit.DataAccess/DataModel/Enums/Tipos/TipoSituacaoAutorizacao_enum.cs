using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
	public class EnumTipoSituacaoAutorizacao
	{
		public List<BaseComboResponse> itens = new List<BaseComboResponse>();

		public EnumTipoSituacaoAutorizacao()
		{
            itens.Add(new BaseComboResponse() { id = 1, stName = "Em Aberto" });
            itens.Add(new BaseComboResponse() { id = 2, stName = "Em Revisão" });
			itens.Add(new BaseComboResponse() { id = 3, stName = "Aprovado Emissor" });
            itens.Add(new BaseComboResponse() { id = 4, stName = "Cancelada Emissor" });
            itens.Add(new BaseComboResponse() { id = 5, stName = "Glosado Plano" });
            itens.Add(new BaseComboResponse() { id = 6, stName = "Aprovado Plano" });
            itens.Add(new BaseComboResponse() { id = 7, stName = "Rejeitado Plano" });
            itens.Add(new BaseComboResponse() { id = 8, stName = "Erro Autorizador" });
        }

		public BaseComboResponse Get(long _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
