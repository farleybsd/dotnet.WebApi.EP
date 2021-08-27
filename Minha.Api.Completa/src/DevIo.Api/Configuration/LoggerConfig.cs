using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIo.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "dabbd16fee15422fbca38cf3ed3e9c45";
                o.LogId = new Guid("b2108008-87aa-4d9a-a6bc-5af63025579b");
            });

            //Adiconando os Logs Aspnet criado por mim para o Elmah
            services.AddLogging(builder => {

                builder.AddElmahIo(o =>
                {
                    o.ApiKey = "dabbd16fee15422fbca38cf3ed3e9c45";
                    o.LogId = new Guid("b2108008-87aa-4d9a-a6bc-5af63025579b");
                });

                builder.AddFilter<ElmahIoLoggerProvider>(null,LogLevel.Warning);
            });


            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}
