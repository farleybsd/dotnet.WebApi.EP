using AutoMapper;
using DevIo.Api.Configuration;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using DevIO.Api.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using DevIo.Api.Extensions;

namespace DevIo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            //{
            //    builder.WithOrigins("https://localhost:44336/").AllowAnyMethod().AllowAnyHeader();
            //}));

            services.AddDbContext<MeuDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityConfiguration(Configuration);
            services.AddAutoMapper(typeof(Startup));

            services.WebApiConfig();
            services.AddSwaggerConfig();
            /*
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<ApiBehaviorOptions>(options => {

                options.SuppressModelStateInvalidFilter = true;
            
            });

            services.AddCors(options => {

                options.AddPolicy("Development",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials()
                    );
            
            });*/

            // services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Title = "My API",
            //        Version = "v1"

            //    });
            //});

            //Elmah Logger
            services.AddLoggingConfiguration();
            services.ResolveDependencies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("Development");
            }
            else
            {
                app.UseCors("Production");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            //app.UseCors("ApiCorsPolicy");
            //app.UseCors("Development");
            app.UseAuthentication(); // obs tem que vir antes do useMvc
                                     // app.UseMvc();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UserMvcConfiguration();
            app.UseSwaggerConfig(provider);
            //Elmah Logger
            app.UseLoggingConfiguration();
            //app.UseSwagger();
            //app.UseSwaggerUI(c => {c.SwaggerEndpoint("/swagger/v1/swagger.json","My API V1");});
        }
    }
}
