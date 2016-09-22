using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServerWithAspNetIdentity.Data;
using IdentityServerWithAspNetIdentity.Models;
using IdentityServerWithAspNetIdentity.Services;
using QuickstartIdentityServer;
using IdentityServer4.Services;
using IdentityModel;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Swashbuckle.Swagger.Model;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServerWithAspNetIdentity
{
    public class Startup
    {
        string PublicHostUri { get { return "http://localhost:5000"; } }

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddCors();

            services.AddMvc();

            services.AddIdentityServer(x =>
            {
                x.IssuerUri = PublicHostUri;
                x.SiteName = HostingEnvironment.ApplicationName;
            })
                .SetSigningCredential(new X509Certificate2(Path.Combine(HostingEnvironment.ContentRootPath, "Identity", "idsvr3test.pfx"), "idsrv3test"))
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))

                .AddOperationalStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)));

            services.AddSwaggerGen(c =>
            {
                c.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = HostingEnvironment.ApplicationName,
                    Description = "REST Backend Quickstart Sample",
                    TermsOfService = ""
                });

                // oauth2
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{PublicHostUri}/connect/authorize",
                    TokenUrl = $"{PublicHostUri}/connect/token",
                    Scopes = new Dictionary<string, string>
                        {
                            { "api1", "api1" }
                        }
                });
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();

                // Setup Databases
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ConfigurationDbContext>().Database.Migrate();
                    serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
                    EnsureSeedData(serviceScope.ServiceProvider.GetService<ConfigurationDbContext>(), serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>());

                    //var dbContextOptions = app.ApplicationServices.GetRequiredService<DbContextOptions<PersistedGrantDbContext>>();
                    var options = serviceScope.ServiceProvider.GetService<DbContextOptions<PersistedGrantDbContext>>();
                    //var tokenCleanup = new TokenCleanup(options);
                    //tokenCleanup.Start();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors(policy =>
            {
                policy.WithOrigins("http://localhost:28895");
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // api
            app.MapWhen(context => context.Request.Path.Value.StartsWith("/api"), map =>
            {
                map.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {
                    Authority = PublicHostUri,
                    ScopeName = "api1",
                    ScopeSecret = "secret",

                    RequireHttpsMetadata = false,
                    SaveToken = true,
                    EnableCaching = true
                });

                map.UseMvc();
            });
            
            app.UseIdentity();
            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            app.UseTwitterAuthentication(new TwitterOptions
            {
                ConsumerKey = "6XaCTaLbMqfj6ww3zvZ5g",
                ConsumerSecret = "Il2eFzGIrYhz6BWjYhVXBPQSfZuS4xoHpSSyD9PI",
                AutomaticAuthenticate = false,
                AutomaticChallenge = false,
            });

            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                DisplayName = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false,
                ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
                ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo"
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUi();
        }

        private static void EnsureSeedData(ConfigurationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in IdentityConfig.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
                //
                foreach (var user in IdentityConfig.GetUsers())
                {
                    userManager.CreateAsync(new ApplicationUser { UserName = user.Username }, user.Password);
                }
            }

            if (!context.Scopes.Any())
            {
                foreach (var client in IdentityConfig.GetScopes().ToList())
                {
                    context.Scopes.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

        }
    }
}
