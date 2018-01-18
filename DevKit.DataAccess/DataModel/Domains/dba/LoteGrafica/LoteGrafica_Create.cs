using LinqToDB;
using System;
using System.Linq;
using System.Threading;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class LoteGrafica
    {
		public string Create(DevKitDB db, string emps)
		{
            var setup = new Setup();
            var lstIds = emps.TrimEnd(',').Split(',');

            var lg = new LoteGrafica
            {
                dtLog = DateTime.Now,
                nuCodigo = Convert.ToInt32(setup.RandomString(6)),
                tgAtivo = 0
            };

            while (db.LoteGrafica.Any(y => y.nuCodigo == lg.nuCodigo))
            {
                Thread.Sleep(1);
                lg.nuCodigo = Convert.ToInt32(setup.RandomString(6));
            }

            lg.id = Convert.ToInt64(db.InsertWithIdentity(lg));

            foreach (var emp in lstIds)
            {
                foreach (var cart in (from e in db.Person
                                      where e.tgExpedicao == 0
                                      where e.tgStatus == 0
                                      where e.fkEmpresa.ToString() == emp
                                      select e).
                                      ToList())
                {
                    db.Insert(new LoteGraficaCartao
                    {
                        fkLoteGrafica = lg.id,
                        fkAssociado = cart.id,
                        fkEmpresa = cart.fkEmpresa,
                        nuTit = 1,
                        nuVia = cart.nuViaCartao
                    });

                    var cartUpd = db.Person.Where(y => y.id == cart.id).FirstOrDefault();

                    cartUpd.tgExpedicao = 1;

                    db.Update(cartUpd);
                }
            }

            return lg.nuCodigo.ToString();
		}
	}
}
