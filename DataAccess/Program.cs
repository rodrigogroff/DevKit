using System;
using System.Linq;
using LinqToDB;
using DataModel;

namespace GetStarted
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var db = new DataModel.SuporteCITDB())
			{
				{
					var x = new Perfil();

					x.StPermissoes = "|1011||1012||1013||1014||1015||1021||1022||1023||1024||1025|";

					db.Insert(x);
				}

				{
					var y = new Usuario();

					y.StLogin = "DBA";
					y.StPassword = "DBA";
					y.FkPerfil = 1;

					db.Insert(y);
				}
			}
		}
	}
}
