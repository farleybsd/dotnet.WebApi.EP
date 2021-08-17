using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIo.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApiVersioning(optios => {
                optios.AssumeDefaultVersionWhenUnspecified = true;
                optios.DefaultApiVersion = new ApiVersion(2, 0);
                optios.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options => {

                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {

                options.SuppressModelStateInvalidFilter = true;

            });

            services.AddCors(options =>
            {

                options.AddPolicy("Development",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials()
                    );
                options.AddPolicy("Production",
                   builder =>
                       builder
                           .WithMethods("GET","Post")
                           .WithOrigins("http://desenvolvedor.io","www.teste.com")
                           .SetIsOriginAllowedToAllowWildcardSubdomains()
                           //.WithHeaders(HeaderNames.ContentType, "x-custom-header")
                           .AllowAnyHeader());

            });

            return services;
        }

        public static IApplicationBuilder UserMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            //app.UseCors("Development");
            app.UseMvc();
            return app;
        }
    }
}
