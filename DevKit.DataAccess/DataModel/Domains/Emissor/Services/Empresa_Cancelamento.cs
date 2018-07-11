using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
    public class Cancelamento_PARAMS
    {
        public string codCredenciado,
                        matricula,
                        fkSecao,                        
                        nsu;

        public DateTime? dt;

        public bool? lote;
    }

    public partial class Empresa
    {
        public string CancelamentoAutorizacao(DevKitDB db, Cancelamento_PARAMS _params)
        {
            var emp = db.EmpresaSecao.FirstOrDefault(y => y.id.ToString() == _params.fkSecao);

            var associadoLst = db.Associado.Where ( y =>  y.fkSecao.ToString() == _params.fkSecao &&
                                                          y.nuMatricula.ToString() == _params.matricula).
                                                          Select ( y=> y.id).
                                                          ToList();

            var cred = db.Credenciado.FirstOrDefault(y => y.nuCodigo.ToString() == _params.codCredenciado);

            var aut = db.Autorizacao.Where(y => associadoLst.Contains((long)y.fkAssociadoPortador) &&
                                                y.fkCredenciado == cred.id &&
                                                y.nuNSU.ToString() == _params.nsu  || y.nuNSURef.ToString() == _params.nsu).
                                                ToList();
            
            if (aut.Count() > 0)
            {
                foreach (var item in aut)
                {
                    var autUpd = db.Autorizacao.FirstOrDefault(y => y.id == item.id);

                    if (autUpd != null)
                    {
                        autUpd.tgSituacao = TipoSitAutorizacao.CanceladaEmissor;

                        db.Update(autUpd);
                    }                    
                }

                return "";
            }

            return "Nenhuma autorização encontrada!";
        }
    }
}