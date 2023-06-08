using CarRentalIdentityServer.Data;
using CarRentalIdentityServer.Options;
using CarRentalIdentityServer.Services.Emails;
using duende;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CarRentalIdentityServer
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {

            var apiUrls = builder.WebHost.GetSetting(WebHostDefaults.ServerUrlsKey).Split(";");
            var baseApiUrls = new BaseApiUrls
            {
                HttpsUrl = apiUrls.FirstOrDefault(x => x.StartsWith("https:")),
                HttpUrl = apiUrls.FirstOrDefault(x => x.StartsWith("http:"))
            };
            builder.Services.AddOptions<BaseApiUrls>()
                .Configure(x =>
                {
                    x.HttpsUrl = baseApiUrls.HttpsUrl;
                    x.HttpUrl = baseApiUrls.HttpUrl;
                });

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<IdentityUser>();

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = builder.Configuration["ExternalAuthenticationProviders:GoogleClientId"];
                    options.ClientSecret = builder.Configuration["ExternalAuthenticationProviders:GoogleClientSecret"];
                });

            builder.Services.AddTransient<IProfileService, ProfileService>();


            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.Configure<EmailsSettings>(builder.Configuration.GetSection("EmailsSettings"));
            return builder.Build();

        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}