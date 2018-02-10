using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public partial class Autorizacao
    {
        public string   sdtSolicitacao, 
                        stgSituacao,
                        sfkCredenciado,
                        snuCodigoCredenciado,
                        sfkEspecialidade,
                        sfkEmpresa,
                        sfkProcedimento,
                        snuMatriculaAssociado,
                        sfkAssociado;
    }
}
