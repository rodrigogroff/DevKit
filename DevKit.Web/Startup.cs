// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="">
//   Copyright © 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: Microsoft.Owin.OwinStartup(typeof(DevKit.Web.Startup))]

namespace DevKit.Web
{
	using Microsoft.Owin;
	using Microsoft.Owin.Security.OAuth;
	using Owin;
	using SimpleInjector;
	using SimpleInjector.Integration.Web.Mvc;
	using SimpleInjector.Integration.WebApi;
	using System;
	using System.Web.Http;
	using System.Web.Mvc;

	public class Startup
	{
		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

		public void Configuration(IAppBuilder app)
		{
			var container = IocConfig.CreateContainer();

			container.RegisterMvcIntegratedFilterProvider();

			// Override default mvc and web api dependency resolvers
			DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
			GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

			ConfigureOAuth(app);

			container.Verify();
		}

		private void ConfigureOAuth(IAppBuilder app)
		{
			OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
				Provider = new SimpleAuthorizationServerProvider(),
			};

			// Geração do token
			app.UseOAuthAuthorizationServer(OAuthServerOptions);
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
		}
	}
}
