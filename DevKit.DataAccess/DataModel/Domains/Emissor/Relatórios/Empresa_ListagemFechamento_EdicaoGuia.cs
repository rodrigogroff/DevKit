using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public partial class Empresa
    {
		public bool EdicaoGuia ( DevKitDB db, long idGuia, long vlr, long vlrCoPart )
		{
            var guia = db.Autorizacao.FirstOrDefault(y => y.id == idGuia);

            if (guia == null)
                return false;

            if (guia.fkEmpresa != db.currentUser.fkEmpresa )
                return false;

            guia.vrParcela = vlr;
            guia.vrParcelaCoPart = vlrCoPart;

            db.Update(guia);

            return true;
		}
    }
}
