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
                        sfkMedico,
                        snuCodigoMedico,
                        sfkEmpresa,
                        sfkProcedimento,
                        snuMattriculaAssociado,
                        sfkAssociado;
    }
}
