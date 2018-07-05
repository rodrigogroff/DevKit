using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public partial class Autorizacao
    {
        public string sdtSolicitacao,
                        stgSituacao,
                        sfkCredenciado,
                        snuCodigoCredenciado,
                        sfkEspecialidade,
                        sfkEmpresa,
                        sfkSecao,
                        sfkProcedimento,
                        snuMatriculaAssociado,
                        sfkAssociado,
                        sfkAssociadoPortador,
                        sfkAssociadoPortadorTit;

        public CupomAutorizacao cupom = new CupomAutorizacao();
    }
}
