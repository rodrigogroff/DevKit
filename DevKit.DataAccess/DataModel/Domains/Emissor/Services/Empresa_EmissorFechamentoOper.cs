using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
    public class EmissorFechamentoOper_PARAMS
    {
        public string  oper,
                        nsu,                        
                        tgSituacaoLote;
    }

    public partial class Empresa
    {
        public string EmissorFechamentoOper(DevKitDB db, EmissorFechamentoOper_PARAMS _params)
        {
            switch (_params.oper)
            {
                case "mudaSituacao":

                    var lst = _params.nsu.Trim().TrimEnd(',').Split(',');

                    foreach (var item in lst)
                    {
                        var aut = db.Autorizacao.FirstOrDefault(y => y.nuNSU.ToString() == item);

                        aut.tgSituacao = Convert.ToInt32(_params.tgSituacaoLote);

                        db.Update(aut);
                    }

                    break;
            }

            return "";
        }
    }
}