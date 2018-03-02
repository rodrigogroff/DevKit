using LinqToDB;
using System;
using System.Linq;
using System.Threading;
using SyCrafEngine;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class Empresa
    {
		public bool Create(DevKitDB db, ref string apiError)
		{
            var mon = new money();
            
            if (!string.IsNullOrEmpty(this.svrMaxProcSemAut)) this.vrMaxProcSemAut = Convert.ToInt64(mon.prepareNumber(this.svrMaxProcSemAut)); else this.vrMaxProcSemAut = 0;
            if (!string.IsNullOrEmpty(this.svrCobMensalidade)) this.vrCobMensalidade = Convert.ToInt64(mon.prepareNumber(this.svrCobMensalidade)); else this.vrCobMensalidade = 0;
            if (!string.IsNullOrEmpty(this.svrCobCartaoBase)) this.vrCobCartaoBase = Convert.ToInt64(mon.prepareNumber(this.svrCobCartaoBase)); else this.vrCobCartaoBase = 0;
            if (!string.IsNullOrEmpty(this.svrCobCartaoAtivo)) this.vrCobCartaoAtivo = Convert.ToInt64(mon.prepareNumber(this.svrCobCartaoAtivo)); else this.vrCobCartaoAtivo = 0;
            if (!string.IsNullOrEmpty(this.svrCobAutorizacao)) this.vrCobAutorizacao = Convert.ToInt64(mon.prepareNumber(this.svrCobAutorizacao)); else this.vrCobAutorizacao = 0;
            if (!string.IsNullOrEmpty(this.svrCobNovoCartao)) this.vrCobNovoCartao = Convert.ToInt64(mon.prepareNumber(this.svrCobNovoCartao)); else this.vrCobNovoCartao = 0;
            if (!string.IsNullOrEmpty(this.svrCobServBloq)) this.vrCobServBloq = Convert.ToInt64(mon.prepareNumber(this.svrCobServBloq)); else this.vrCobServBloq = 0;
            if (!string.IsNullOrEmpty(this.svrCobServDesbloq)) this.vrCobServDesbloq = Convert.ToInt64(mon.prepareNumber(this.svrCobServDesbloq)); else this.vrCobServDesbloq = 0;

            this.id = Convert.ToInt64(db.InsertWithIdentity(this));

            return true;
		}
	}
}
