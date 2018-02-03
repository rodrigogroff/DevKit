﻿using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
    public class AutorizaProcedimentoParams
    {
        public string ca, senha, titVia;
        public long emp, mat, tuss;
    }

    public partial class Medico
    {
		public string AutorizaProcedimento ( DevKitDB db, AutorizaProcedimentoParams _params )
		{
            var util = new Util();

            var nuTit = Convert.ToInt32(_params.titVia.Substring(0, 2));
            var nuVia = Convert.ToInt32(_params.titVia.Substring(2, 2));

            var empTb = (from e in db.Empresa
                         where e.nuEmpresa == _params.emp
                         select e).
                         FirstOrDefault();

            if (empTb == null)
                return "Empresa inválida";

            var associado = (from e in db.Associado
                             where e.fkEmpresa == empTb.id
                             where e.nuMatricula == _params.mat
                             where e.nuTitularidade == nuTit
                             where e.nuViaCartao == nuVia
                             select e).
                             FirstOrDefault();

            if (associado == null)
                return "Matrícula inválida";

            var caCalc = util.calculaCodigoAcesso(_params.emp.ToString().PadLeft(6, '0'),
                                                    _params.mat.ToString().PadLeft(6, '0'),
                                                    associado.nuTitularidade.ToString(),
                                                    associado.nuViaCartao.ToString(),
                                                    associado.stCPF);

            if (_params.ca != caCalc)
                return "Dados do cartão inválidos!";

            if (_params.senha != associado.stSenha)
                return "Senha inválida!";

            if (!db.MedicoEmpresa.Any(y => y.fkMedico == db.currentMedico.id &&
                                           y.fkEmpresa == empTb.id))
                return "Médico não conveniado à empresa " + _params.emp;

            var proc = db.TUSS.
                            Where(y => y.nuCodTUSS == _params.tuss).
                            FirstOrDefault();

            if (proc == null)
                return "Procedimento " + _params.tuss + " inválido!";

            //var dtHoje = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //var dtHojeFim = dtHoje.AddDays(1);

            //if (db.Autorizacao.Any (y=> y.fkMedico == db.currentMedico.id && 
            //                            y.fkAssociado == associado.id && 
            //                            y.dtSolicitacao > dtHoje && y.dtSolicitacao < dtHojeFim))
            //{
            //return BadRequest("Procedimento " + tuss + " em duplicidade!");
            //}
            //else

            DateTime dt = DateTime.Now;

            if (dt.Day < empTb.nuDiaFech)
                dt = dt.AddMonths(-1);

            db.Insert(new Autorizacao
            {
                dtSolicitacao = DateTime.Now,
                fkAssociado = associado.id,
                fkMedico = db.currentMedico.id,
                fkEmpresa = associado.fkEmpresa,
                fkProcedimento = proc.id,
                nuAno = dt.Year,
                nuMes = dt.Month,
                tgSituacao = TipoSitAutorizacao.Autorizado,
            });

            return "";
		}
	}
}
