using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestPlanService.Services.Sessions;
using TestPlanService.Exceptions;
using TestPlanService.Logging;
using TestPlanService.Services.Db;
using TestPlanService.Services.Cleaning;
using TestPlanService.Services.Scheduling;
using TestPlanService.Services.Config;
using TestPlanService.Services.Setup;
using Microsoft.EntityFrameworkCore;
using System; 

namespace TestPlanService
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
            services.AddTransient<ErrorHandlerMiddleware>();

            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddCors();
            services.AddAuthorization(opts => { });
            services.AddSingleton<ConfigService>();

            services.AddDbContextPool<DatabaseContext>((builder) => {
                var config = new ConfigService(Configuration);
                DatabaseContext.Setup(builder, config);
            });

            services.AddScoped<DatabaseService>();
            services.AddScoped<AccessService>();

            //services.AddDbContext<DatabaseContext>(ServiceLifetime.Singleton);
            //services.AddSingleton<CleaningService>(); 
            //services.AddSingleton<ISchedulerWorker>(o => o.GetService<CleaningService>()); 
            //services.AddSingleton<DatabaseService>();

            services.AddHostedService<SetupService>();

            services.AddSwaggerDocument();
            services.AddControllers().AddNewtonsoftJson(p =>
            {
                p.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            loggerFactory.AddFile();
            // app.ApplicationServices.GetRequiredService<RestApiService>();

            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(builder => builder.WithOrigins("http://localhost:4200")
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );

            app.UseAuthentication();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
