using DevIo.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIo.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        private ILogger _logger;
        public TesteController(INotificador notificador,
            ILogger<TesteController> logger,
            IUser appUser) : base(notificador, appUser)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Valor()
        {
            _logger.LogTrace("Log de Trace"); // Apenas em Dev
            _logger.LogDebug("Log de Debug");// Apenas em Dev
            _logger.LogInformation("Log de Informação");
            _logger.LogWarning("Log de Aviso");
            _logger.LogError("Log de Erro");
            _logger.LogCritical("Log de Problema Critico");
            return "Sou a V2";
        }
        
    }
}
