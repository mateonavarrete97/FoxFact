using FoxFact.DTO;
using FoxFact.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FoxFact.Controller
{
    public class ApiController
    {
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        [Function("ApiController")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        [Function("GetEnergiaActiva")]
        [OpenApiOperation(
            operationId: nameof(GetEnergiaActiva),
            tags: new[] { nameof(ApiController) },
            Description = "Obtiene la cantidad de Energ�a Activa (EA) y la tarifa correspondiente para un mes espec�fico, seg�n los headers proporcionados."
        )]
        [OpenApiParameter(
            name: "year",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Required = true,
            Type = typeof(int),
            Description = "A�o para el que se obtendr�n los datos de Energ�a Activa."
        )]
        [OpenApiParameter(
            name: "mes",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Required = true,
            Type = typeof(int),
            Description = "Mes para el que se obtendr�n los datos de Energ�a Activa."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<EnergiaActivaDTO>),
            Description = "Retorna una lista con las cantidades de EA y tarifas."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.BadRequest,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Solicitud inv�lida."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.InternalServerError,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Error interno del servidor."
        )]
        [OpenApiResponseWithoutBody(
            HttpStatusCode.Unauthorized,
            Description = "Acceso no autorizado."
        )]
        public async Task<IActionResult> GetEnergiaActiva(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest input,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetEnergiaActiva");
            logger.LogInformation("C# HTTP trigger function 'GetEnergiaActiva' processed a request.");

            try
            {
                string yearHeader = input.Headers["year"];
                string mesHeader = input.Headers["mes"];

                if (string.IsNullOrWhiteSpace(yearHeader) || string.IsNullOrWhiteSpace(mesHeader))
                {
                    logger.LogError("Headers 'year' y 'mes' son requeridos.");
                    return new BadRequestObjectResult("Los headers 'year' y 'mes' son obligatorios.");
                }

                if (!int.TryParse(yearHeader, out int year) || !int.TryParse(mesHeader, out int mes))
                {
                    logger.LogError("Los valores de los headers 'year' y 'mes' deben ser enteros.");
                    return new BadRequestObjectResult("Los valores de los headers 'year' y 'mes' deben ser enteros.");
                }

                if (mes < 1 || mes > 12)
                {
                    logger.LogError("El valor de 'mes' debe estar entre 1 y 12.");
                    return new BadRequestObjectResult("El valor de 'mes' debe estar entre 1 y 12.");
                }

                ApiManager apiManager = new ApiManager();
                List<EnergiaActivaDTO> result = await apiManager.GetEnergiaActiva(year, mes);

                return new OkObjectResult(result);
            }
            catch (ArgumentException ex)
            {
                logger.LogError($"Bad request: {ex.Message}");
                return new BadRequestObjectResult($"Solicitud inv�lida: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError($"Unauthorized access: {ex.Message}");
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                logger.LogError($"Internal server error: {ex.Message}");
                return new ObjectResult("Error interno del servidor.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        [Function("GetComercializacionExcedentesEnergia")]
        [OpenApiOperation(
            operationId: nameof(GetComercializacionExcedentesEnergia),
            tags: new[] { "ComercializacionExcedentesEnergia" },
            Description = "Obtiene los datos de Comercializaci�n de Excedentes de Energ�a para un mes y a�o espec�ficos."
        )]
        [OpenApiParameter(
            name: "mes",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = true,
            Type = typeof(int),
            Description = "El mes para el c�lculo (1-12)."
        )]
        [OpenApiParameter(
            name: "year",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = true,
            Type = typeof(int),
            Description = "El a�o para el c�lculo."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<ComercializacionExcedentesEnergiaDTO>),
            Description = "Retorna una lista con los datos de comercializaci�n de excedentes."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.BadRequest,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Solicitud inv�lida (par�metros incorrectos o faltantes)."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.InternalServerError,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Error interno del servidor."
        )]
        public async Task<IActionResult> GetComercializacionExcedentesEnergia(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetComercializacionExcedentesEnergia");
            logger.LogInformation("C# HTTP trigger function 'GetComercializacionExcedentesEnergia' processed a request.");

            try
            {
                string mesQuery = req.Query["mes"];
                string yearQuery = req.Query["year"];

                if (string.IsNullOrWhiteSpace(mesQuery) || string.IsNullOrWhiteSpace(yearQuery))
                {
                    logger.LogError("Faltan los par�metros 'mes' o 'year'.");
                    return new BadRequestObjectResult("Los par�metros 'mes' y 'year' son obligatorios.");
                }

                if (!int.TryParse(mesQuery, out int mes) || !int.TryParse(yearQuery, out int year))
                {
                    logger.LogError("Los par�metros 'mes' y 'year' deben ser valores enteros.");
                    return new BadRequestObjectResult("Los par�metros 'mes' y 'year' deben ser valores enteros.");
                }

                if (mes < 1 || mes > 12)
                {
                    logger.LogError("El par�metro 'mes' debe estar entre 1 y 12.");
                    return new BadRequestObjectResult("El par�metro 'mes' debe estar entre 1 y 12.");
                }

                ApiManager apiManager = new ApiManager();
                List<ComercializacionExcedentesEnergiaDTO> result = await apiManager.GetComercializacionExcedentesEnergia(mes, year);

                return new OkObjectResult(result);
            }
            catch (ArgumentException ex)
            {
                logger.LogError($"Solicitud inv�lida: {ex.Message}");
                return new BadRequestObjectResult($"Solicitud inv�lida: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error interno del servidor: {ex.Message}");
                return new ObjectResult("Error interno del servidor.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
        [Function("GetExcedentesEnergiaTipoUno")]
        [OpenApiOperation(
            operationId: nameof(GetExcedentesEnergiaTipoUno),
            tags: new[] { "ExcedentesEnergia" },
            Description = "Obtiene los datos de Excedentes de Energ�a Tipo 1 (EE1) para un mes, un a�o y un servicio espec�ficos."
        )]
                [OpenApiParameter(
            name: "mes",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = true,
            Type = typeof(int),
            Description = "El mes del c�lculo (1-12)."
        )]
                [OpenApiParameter(
            name: "year",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = true,
            Type = typeof(int),
            Description = "El a�o del c�lculo."
        )]
                [OpenApiParameter(
            name: "idservice",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = true,
            Type = typeof(int),
            Description = "El identificador del servicio."
        )]
                [OpenApiResponseWithBody(
            HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<ExcedentesEnergiaTipoUnoDTO>),
            Description = "Lista con los datos de Excedentes de Energ�a Tipo 1."
        )]
                [OpenApiResponseWithBody(
            HttpStatusCode.BadRequest,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Solicitud inv�lida debido a par�metros incorrectos o faltantes."
        )]
                [OpenApiResponseWithBody(
            HttpStatusCode.InternalServerError,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Error interno del servidor."
        )]
            public async Task<IActionResult> GetExcedentesEnergiaTipoUno(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        FunctionContext executionContext)
            {
                var logger = executionContext.GetLogger("GetExcedentesEnergiaTipoUno");
                logger.LogInformation("C# HTTP trigger function 'GetExcedentesEnergiaTipoUno' processed a request.");

                try
                {
                    string mesQuery = req.Query["mes"];
                    string yearQuery = req.Query["year"];
                    string idServiceQuery = req.Query["idservice"];

                    if (string.IsNullOrWhiteSpace(mesQuery) || string.IsNullOrWhiteSpace(yearQuery) || string.IsNullOrWhiteSpace(idServiceQuery))
                    {
                        logger.LogError("Faltan los par�metros 'mes', 'year' o 'idservice'.");
                        return new BadRequestObjectResult("Los par�metros 'mes', 'year' y 'idservice' son obligatorios.");
                    }

                    if (!int.TryParse(mesQuery, out int mes) || !int.TryParse(yearQuery, out int year) || !int.TryParse(idServiceQuery, out int idservice))
                    {
                        logger.LogError("Los par�metros 'mes', 'year' e 'idservice' deben ser valores enteros.");
                        return new BadRequestObjectResult("Los par�metros 'mes', 'year' e 'idservice' deben ser valores enteros.");
                    }

                    if (mes < 1 || mes > 12)
                    {
                        logger.LogError("El par�metro 'mes' debe estar entre 1 y 12.");
                        return new BadRequestObjectResult("El par�metro 'mes' debe estar entre 1 y 12.");
                    }

                    ApiManager apiManager = new ApiManager();
                    List<ExcedentesEnergiaTipoUnoDTO> result = await apiManager.ExcedentesEnergiaTipoUnoDAO(mes, year, idservice);

                    return new OkObjectResult(result);
                }
                catch (ArgumentException ex)
                {
                    logger.LogError($"Solicitud inv�lida: {ex.Message}");
                    return new BadRequestObjectResult($"Solicitud inv�lida: {ex.Message}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error interno del servidor: {ex.Message}");
                    return new ObjectResult("Error interno del servidor.") { StatusCode = (int)HttpStatusCode.InternalServerError };
                }
            }

    }
}

