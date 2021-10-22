using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WinAuth.DataBase;

namespace WinAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // подключаем swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo{ Version = "1", Title = "WinAuth API" });
            });
            services.AddMvc();
            
            // подключаем EF
            services.AddDbContext<MyContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("Default")));
            
            // аутентификация Negotiate через библиотеку using Microsoft.AspNetCore.Authentication.Negotiate
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate(opt =>
                {
                     if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                     {
                        opt.EnableLdap(settings =>
                        {
                            settings.Domain = "domain.ru";
                            settings.MachineAccountName = Configuration.GetSection("LDAP")["Username"];
                            settings.MachineAccountPassword = Configuration.GetSection("LDAP")["Password"];
                        });
                     }
                });
            
            // чтобы корректно сериализовать в JSON объекты с циклическими референсами
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseAuthentication();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WinAuth");
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
