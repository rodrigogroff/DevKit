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
            var medico = db.Medico.Where(y => y.id == fkMedico).FirstOrDefault();
            var espec = db.Especialidade.Where(y => y.id == medico.fkEspecialidade).FirstOrDefault();

            sdtSolicitacao = Convert.ToDateTime(dtSolicitacao).ToString("dd/MM/yyyy HH:mm");

            sfkEmpresa = db.Empresa.Where(y => y.id == fkEmpresa).FirstOrDefault().stSigla;
            sfkProcedimento = proc.nuCodTUSS + " - " + proc.stProcedimento;

            sfkAssociado = assoc.stName;
            snuMattriculaAssociado = assoc.nuMatricula.ToString();

            snuCodigoMedico = medico.nuCodigo.ToString();
            sfkMedico = medico.stNome;

            sfkEspecialidade = espec.stNome;

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
