using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class HomeDTO
	{
		public string name;

		public List<CompanyNews> news = new List<CompanyNews>();
		public List<Survey> surveys = new List<Survey>();
	}

	public class HomeView
	{
		public HomeDTO ComposedFilters(DevKitDB db)
		{
			var user = db.GetCurrentUser();
			var projects = db.GetCurrentUserProjects();

			var dto = new HomeDTO();

			dto.name = "Hi " + user.stLogin;

			// noticias não lidas

			dto.news = (from e in db.CompanyNews
						join eNR in db.UserNewsReads on e.id equals eNR.fkNews
						where e.id != eNR.fkNews
						where eNR.fkUser == user.id
						where e.fkProject == null || projects.Contains(e.fkProject)
						select e).
						ToList();

			foreach (var item in dto.news)
				item.LoadAssociations(db);
			
			return dto;
		}
	}
}
