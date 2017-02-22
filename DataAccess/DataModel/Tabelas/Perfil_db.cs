using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class PerfilLoad_Params
	{
		public bool bTudo = false,
					bUsuarios = false,
					bQtdUsuarios = false;
	}

	public partial class Perfil
	{
		PerfilLoad_Params load = new PerfilLoad_Params { bTudo = true };
				
		public int	QtdeUsuarios = 0,
					QtdePermissoes = 0;

		public List<Usuario> Usuarios { get; set; }
		public List<Usuario> LoadUsuarios(SuporteCITDB db) { return (from e in db.Usuarios where e.FkPerfil == Id select e).ToList(); }
		public int CountUsuarios(SuporteCITDB db) { return (from e in db.Usuarios where e.FkPerfil == Id select e).Count(); }

		public Perfil Load(SuporteCITDB db, PerfilLoad_Params _load = null)
		{
			if (_load != null)
				load = _load;

			if (StPermissoes != null && StPermissoes.Length > 0)
				QtdePermissoes = StPermissoes.Split('|').Length / 2;

			// usuários
			if (load.bTudo || load.bUsuarios) Usuarios = LoadUsuarios(db);
			if (load.bTudo) QtdeUsuarios = Usuarios.Count(); else if (load.bQtdUsuarios) QtdeUsuarios = CountUsuarios(db);
			
			return this;
		}
	}
}
