using System.IO;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Master
{  
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<LocalNetwork>(Configuration.GetSection("localNetwork"));
            services.AddResponseCompression();
            services.AddMemoryCache();

            var key = Encoding.ASCII.GetBytes(LocalNetwork.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
  
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
#if DEBUG
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
#endif

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseResponseCompression();
            app.UseAuthentication();
            app.UseMvc();

            //sql migrations
            var connStr = Configuration["localNetwork:sqlServer"];
            var baseDb = File.ReadAllText(@"Repository\CreateDB_pg.sql");
            var db = new NpgsqlConnection(connStr);
            db.Open();
            db.Query(baseDb);
            db.Close();
        }
    }
}
