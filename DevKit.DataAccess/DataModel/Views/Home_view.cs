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

			#region - news - 
			{
				var news = from e in db.CompanyNews
						   where e.fkProject == null || projects.Contains(e.fkProject)
						   select e;

				var newsRead = (from e in db.UserNewsReads where e.fkUser == user.id select e.fkNews).ToList();

				if (newsRead.Count() > 0)
					news = from e in news where !newsRead.Contains(e.id) select e;

				dto.news = news.OrderByDescending(y => y.id).ToList();

				foreach (var item in dto.news)
					item.LoadAssociations(db);
			}
			#endregion

			#region - survey - 
			{
				var surveys = from e in db.Surveys
							  where e.fkProject == null || projects.Contains(e.fkProject)
							  select e;

				var surveysMark = (from e in db.SurveyUserOptions where e.fkUser == user.id select e.fkSurvey).ToList();

				if (surveysMark.Count() > 0)
					surveys = from e in surveys where !surveysMark.Contains(e.id) select e;

				dto.surveys = surveys.OrderByDescending(y => y.id).ToList();

				foreach (var item in dto.surveys)
					item.LoadAssociations(db);
			}
			#endregion

			return dto;
		}
	}
}
