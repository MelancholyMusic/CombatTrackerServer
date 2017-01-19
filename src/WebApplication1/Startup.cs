using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CombatTrackerServer.Data;
using CombatTrackerServer.Models;
using CombatTrackerServer.Services;
using Microsoft.AspNetCore.Mvc;
using CombatTrackerServer.Models.MongoDB;
using CombatTrackerServer.Net;

namespace CombatTrackerServer
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if(env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();

				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.

			// https://blogs.msdn.microsoft.com/webdev/2016/10/27/bearer-token-authentication-in-asp-net-core/
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				// Configure the context to use Microsoft SQL Server.
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

				// Register the entity sets needed by OpenIddict.
				// Note: use the generic overload if you need
				// to replace the default OpenIddict entities.
				options.UseOpenIddict();
			});

			services.AddApplicationInsightsTelemetry(Configuration);

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddOpenIddict()
				.AddEntityFrameworkCoreStores<ApplicationDbContext>()
				.AddMvcBinders()
				.EnableTokenEndpoint("/connect/token")
				.AllowPasswordFlow()
				.AddEphemeralSigningKey(); // TODO: Create real signing key
										   //.AddSigningCertificate(jwtSigningCert);

			services.AddMvc();

			services.AddLogging();

			services.AddSwaggerGen();

			// Add application services.
			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddTransient<MongoDBDataAccess>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseApplicationInsightsRequestTelemetry();

			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseApplicationInsightsExceptionTelemetry();

			app.UseStaticFiles();

			app.UseIdentity();
			app.UseOAuthValidation();
			app.UseOpenIddict();

			// Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
			//app.UseFacebookAuthentication(new FacebookOptions()
			//{
			//	AppId = Configuration["Authentication:Facebook:AppId"],
			//	AppSecret = Configuration["Authentication:Facebook:AppSecret"]
			//});

			//app.UseGoogleAuthentication(new GoogleOptions()
			//{
			//	ClientId = Configuration["Authentication:Google:ClientId"],
			//	ClientSecret = Configuration["Authentication:Google:ClientSecret"]
			//});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseSwagger();
			app.UseSwaggerUi();

			app.Map("/echo", SocketHandler.Map);
		}
	}
}
