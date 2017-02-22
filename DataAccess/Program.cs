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
				var x = db.Perfils.Find(1);

				x.StPermissoes = "|1012|";

				db.Update(x);
			}
		}
	}
}
