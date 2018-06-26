using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class LancaDespesa_PARAMS
    {
        public long?    matricula,
                        credenciado,
                        vrValor,
                        nuTipo,
                        fkPrecificacao,
                        vrTotal,
                        nuParcelas;

        public string dataLanc = "";
    }
           
    public partial class Empresa
    {
		public string LancaDespesa( DevKitDB db, LancaDespesa_PARAMS _params )
		{
            #region - associado - 

            if (_params.matricula == 0)
                return "Matrícula inválida";

            var assoc = db.Associado.FirstOrDefault(y => y.fkEmpresa == db.currentUser.fkEmpresa &&
                                            y.nuMatricula == _params.matricula &&
                                            y.nuTitularidade == 1);

            if (assoc == null)
                return "Matrícula inválida";

            if (assoc.tgStatus == TipoSituacaoCartao.Bloqueado)
                return "Matrícula bloqueada";

            #endregion

            #region - credenciado -

            if (_params.credenciado == 0)
                return "Credenciado inválido";

            var cred = db.Credenciado.FirstOrDefault(y => y.nuCodigo == _params.credenciado);

            if (cred == null)
                return "Credenciado inválido";
            
            #endregion

            if ( _params.nuTipo == 0)
                return "Tipo de precificação inválido";

            

            return "";
		}
    }
}
