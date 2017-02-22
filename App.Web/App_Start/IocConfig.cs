using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using System.Security.Claims;
using System.Web;

namespace App.Web
{
	public class IocConfig
	{
		public static Container CreateContainer()
		{
			var container = new Container();

			container.Options.DefaultLifestyle = new WebRequestLifestyle();
			
			container.Register(() =>
			{
				if (AdvancedExtensions.IsVerifying(container) || HttpContext.Current == null)
					return new ClaimsPrincipal();

				return HttpContext.Current.User;
			});

			return container;
		}
	}
}