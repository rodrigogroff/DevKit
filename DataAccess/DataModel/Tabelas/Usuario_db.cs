using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public class UsuarioLoadParams
	{
		public bool bTudo = false,
					bPerfil = false,
					bTelefones = false,
					bQtdeTelefones = false,
					bEmails = false,
					bQtdeEmails = false;
	}

	public class UsuarioFilter
	{
		public int skip, take;
		public long? fkPerfil;
		public bool? ativo;
		public string busca;
	}

	public partial class Usuario
	{
		UsuarioLoadParams load = new UsuarioLoadParams { bTudo = true };

		public int	QtdeTelefones = 0,
					QtdeEmails = 0;

		public Perfil Perfil;
		public Perfil LoadPerfil(SuporteCITDB db) { return (from e in db.Perfils where e.Id == FkPerfil select e).FirstOrDefault(); }

		public List<UsuarioTelefone> Telefones = new List<UsuarioTelefone>();
		public List<UsuarioTelefone> LoadTelefones(SuporteCITDB db) { return (from e in db.UsuarioTelefones where e.FkUsuario == Id select e).OrderBy ( t=> t.StTelefone).ToList(); }
		public int CountTelefones(SuporteCITDB db) { return (from e in db.UsuarioTelefones where e.FkUsuario == Id select e).Count(); }

		public List<UsuarioEmail> Emails = new List<UsuarioEmail>();
		public List<UsuarioEmail> LoadEmails(SuporteCITDB db) { return (from e in db.UsuarioEmails where e.FkUsuario == Id select e).OrderByDescending(t=>t.DtCriacao).ToList(); }
		public int CountEmails(SuporteCITDB db) { return (from e in db.UsuarioEmails where e.FkUsuario == Id select e).Count(); }

		public IQueryable<Usuario> ComposedFilters(SuporteCITDB db, UsuarioFilter filter)
		{
			var query = from e in db.Usuarios select e;

			if (filter.ativo != null)
				query = from e in query where e.bAtivo == filter.ativo select e;

			if (filter.busca != null)
				query = from e in query where e.StLogin.ToUpper().Contains(filter.busca) select e;

			if (filter.fkPerfil != null)
				query = from e in query where e.FkPerfil == filter.fkPerfil select e;

			return query;
		}

		public Usuario Load(SuporteCITDB db, UsuarioLoadParams _load = null)
		{
			if (_load != null)
				load = _load;

			if (load.bTudo || load.bPerfil)
				Perfil = LoadPerfil(db);

			// telefones
			if (load.bTudo || load.bTelefones) Telefones = LoadTelefones(db);
			if (load.bTudo) QtdeTelefones = Telefones.Count(); else if (load.bQtdeTelefones) QtdeTelefones = CountTelefones(db);

			// emails
			if (load.bTudo || load.bEmails) Emails = LoadEmails(db);
			if (load.bTudo) QtdeEmails = Emails.Count(); else if (load.bQtdeEmails) QtdeEmails = CountEmails(db);

			return this;
		}
		
		public bool Update(SuporteCITDB db, ref string resp)
		{
			if (!UpdateTelefones(db))
			{
				resp = "Telefone duplicado!";
				return false;
			}

			if (!UpdateEmails(db))
			{
				resp = "Email duplicado!";
				return false;
			}

			db.Update(this);

			return true;
		}
		
		public bool UpdateTelefones(SuporteCITDB db)
		{
			// busca maskara global
			var mask = "(99) 9999999";

			for (int x = 0; x < Telefones.Count; ++x)
			{
				var item = Telefones.ElementAt(x);

				bool foundMask = false;

				foreach (var i in item.StTelefone)
					if (!Char.IsLetterOrDigit(i))
					{
						foundMask = true;
						break;
					}
					
				if (!foundMask)
				{
					var result = ""; var index = 0; var maxlen = item.StTelefone.Length;

					foreach (var i in mask)
					{
						if (Char.IsLetterOrDigit(i))
						{
							if (index < maxlen)
								result += item.StTelefone[index++];
						}
						else
							result += i;
					}

					item.StTelefone = result;
				}
			}

			// atualiza toda a arvore de corelacionais
			var originais = LoadTelefones(db);
			var ids_orig = (from e in originais select e.Id).ToList();
			var ids_new = (from e in Telefones select e.Id).ToList();

			foreach (var itemOldId in ids_orig)
				if (!ids_new.Contains(itemOldId))
					db.Delete((from e in originais where e.Id == itemOldId select e).FirstOrDefault());

			foreach (var itemNewId in ids_new)
				if (!ids_orig.Contains(itemNewId))
					db.Insert((from e in Telefones where e.Id == itemNewId select e).FirstOrDefault());

			return true;
		}

		public bool UpdateEmails(SuporteCITDB db)
		{
			var originais = LoadEmails(db);
			var ids_orig = (from e in originais select e.Id).ToList();
			var ids_new = (from e in Emails select e.Id).ToList();

			foreach (var itemOldId in ids_orig)
				if (!ids_new.Contains(itemOldId))
					db.Delete((from e in originais where e.Id == itemOldId select e).FirstOrDefault());

			foreach (var itemNewId in ids_new)
				if (!ids_orig.Contains(itemNewId))
					db.Insert((from e in Emails where e.Id == itemNewId select e).FirstOrDefault());

			return true;
		}
	}
}
