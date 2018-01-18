using DataModel;
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
		public void Ativar(DevKitDB db, string lotes)
		{
            var lstLotes = lotes.TrimEnd(',').Split(',');

            foreach (var loteID in lstLotes)
            {
                var loteUpd = db.LoteGrafica.
                                Where(y => y.id.ToString() == loteID).
                                FirstOrDefault();

                loteUpd.tgAtivo = 1;

                foreach (var cart in (from e in db.LoteGraficaCartao
                                      where e.fkLoteGrafica.ToString() == loteID
                                      select e.fkAssociado).
                                      ToList())
                {
                    var personUpd = db.Person.
                                    Where(y => y.id == cart).
                                    FirstOrDefault();

                    personUpd.tgExpedicao = 2;

                    db.Update(personUpd);
                }

                db.Update(loteUpd);
            }
		}
	}
}
