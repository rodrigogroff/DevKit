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

			var news = from e in db.CompanyNews
						where e.fkProject == null || projects.Contains(e.fkProject)
						select e;

			var newsRead = from e in db.UserNewsReads where e.fkUser == user.id select e;

			if (newsRead.Count() > 0)
			{
				news = from e in news
					   join eRead in newsRead on e.id equals eRead.fkNews
					   where e.id != eRead.fkNews
					   select e;
			}

			dto.news = news.ToList();

			foreach (var item in dto.news)
				item.LoadAssociations(db);
			
			return dto;
		}
	}
}
