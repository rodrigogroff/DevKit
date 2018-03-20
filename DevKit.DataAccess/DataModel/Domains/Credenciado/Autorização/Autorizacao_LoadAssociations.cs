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
            var assocPortador = db.Associado.Where(y => y.id == fkAssociadoPortador).FirstOrDefault();
            var cred = db.Credenciado.Where(y => y.id == fkCredenciado).FirstOrDefault();

            if (cred != null )
            {
                var espec = db.Especialidade.Where(y => y.id == cred.fkEspecialidade).FirstOrDefault();
                sfkEspecialidade = espec.stNome;

                snuCodigoCredenciado = cred.nuCodigo.ToString();
                sfkCredenciado = cred.stNome;
            }

            sdtSolicitacao = Convert.ToDateTime(dtSolicitacao).ToString("dd/MM/yyyy HH:mm");

            sfkEmpresa = db.EmpresaSecao.Where(y => y.id == assoc.fkSecao).Select (y=> y.nuEmpresa + " - " + y.stDesc).FirstOrDefault();

            if (proc != null)
                sfkProcedimento = proc.nuCodTUSS + " - " + proc.stProcedimento;

            sfkAssociado = assoc.stName;

            if (assocPortador != null)
            {
                sfkAssociadoPortador = assocPortador.stName;
                sfkAssociadoPortadorTit = assocPortador.nuTitularidade.ToString().PadLeft(2,'0');
            }
            else
                sfkAssociadoPortadorTit = assoc.nuTitularidade.ToString().PadLeft(2, '0');

            snuMatriculaAssociado = assoc.nuMatricula.ToString();

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
