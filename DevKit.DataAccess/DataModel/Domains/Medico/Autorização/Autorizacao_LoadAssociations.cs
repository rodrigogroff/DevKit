using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;
using DevKit.DataAccess;

namespace DataModel
{
	public partial class Autorizacao
    {
		public Autorizacao LoadAssociations(DevKitDB db)
		{
            var proc = db.TUSS.Where(y => y.id == fkProcedimento).FirstOrDefault();
            var assoc = db.Associado.Where(y => y.id == fkAssociado).FirstOrDefault();

            sdtSolicitacao = Convert.ToDateTime(dtSolicitacao).ToString("dd/MM/yyyy HH:mm");

            sfkMedico = db.Medico.Where (y=>y.id == fkMedico).FirstOrDefault().stNome;
            sfkEmpresa = db.Empresa.Where(y => y.id == fkEmpresa).FirstOrDefault().stSigla;
            sfkProcedimento = proc.nuCodTUSS + " - " + proc.stProcedimento;
            sfkAssociado = assoc.nuMatricula + " - " + assoc.stName;

            switch (tgSituacao)
            {
                case TipoSitAutorizacao.Autorizado: stgSituacao = "Autorizado"; break;
                case TipoSitAutorizacao.Confirmado: stgSituacao = "Confirmado"; break;
                case TipoSitAutorizacao.Glosado:    stgSituacao = "Glosado"; break;
                case TipoSitAutorizacao.Rejeitado:  stgSituacao = "Rejeitado"; break;
            }

            return this;
		}
    }
}
