using TD.SLA.Web.Helpers.Recaptcha;
using Umbraco.Cms.Core.Notifications;
using UmbracoStarterProject.EventsHandler;
using UmbracoStarterProject.Models;
using UmbracoStarterProject.Services;

namespace UmbracoStarterProject
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337.
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddWebsite()
                .AddComposers()
                .AddNotificationHandler<ContentSavingNotification, GenericHandler>()

                .Build();
            services.AddScoped<IGenericService, GenericService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IGalleryService, GalleryService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            services.AddScoped<IHomeService, HomeService>();
            services.Configure<SearchOptions>(_config.GetSection("search"));

            services.Configure<MailSettings>(_config.GetSection("MailSettings"));
            services.AddTransient<IGoogleRecaptchaV3Service, GoogleRecaptchaV3Service>();


			services.AddScoped<IReportService, ReportService>();

		}

		/// <summary>
		/// Configures the application.
		/// </summary>
		/// <param name="app">The application builder.</param>
		/// <param name="env">The web hosting environment.</param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO , to test this one development server, we need to make it under the else
                app.UseExceptionHandler("/Error/Index");
            }

            app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();
                })
                .WithEndpoints(u =>
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });
        }
    }
}
