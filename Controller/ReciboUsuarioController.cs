using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FoxFact.Manager;
using FoxFact.DTO;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace FoxFact.Controller
{
    public class ReciboUsuarioController
    {
        private readonly ILogger<ReciboUsuarioController> _logger;

        public ReciboUsuarioController(ILogger<ReciboUsuarioController> logger)
        {
            _logger = logger;
        }

        [Function("CalculateInvoice")]
        [OpenApiOperation(
            operationId: nameof(CalculateInvoice),
            tags: new[] { "Invoice" },
            Description = "Calcula la factura de un cliente para un mes específico."
        )]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(CalculateInvoiceRequestDTO),
            Required = true,
            Description = "Parámetros requeridos para calcular la factura."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(ReciboUsuarioDTO),
            Description = "La factura generada con los conceptos calculados."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.BadRequest,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Solicitud inválida (parámetros incorrectos o faltantes)."
        )]
        [OpenApiResponseWithBody(
            HttpStatusCode.InternalServerError,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "Error interno del servidor."
        )]
        public async Task<IActionResult> CalculateInvoice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CalculateInvoice");
            logger.LogInformation("C# HTTP trigger function 'CalculateInvoice' processed a request.");

            try
            {
                // Leer y deserializar el cuerpo de la solicitud
                var requestBody = await req.ReadFromJsonAsync<CalculateInvoiceRequestDTO>();
                if (requestBody == null)
                {
                    logger.LogError("El cuerpo de la solicitud está vacío o es inválido.");
                    return new BadRequestObjectResult("El cuerpo de la solicitud debe incluir year, mes y idService.");
                }

                // Validar los parámetros del cuerpo
                if (requestBody.Mes < 1 || requestBody.Mes > 12)
                {
                    logger.LogError("El parámetro 'mes' debe estar entre 1 y 12.");
                    return new BadRequestObjectResult("El parámetro 'mes' debe estar entre 1 y 12.");
                }

                // Llamar al manager para calcular la factura
                ReciboUsuarioManager manager = new ReciboUsuarioManager();
                ReciboUsuarioDTO recibo = await manager.ObtenerReciboUsuario(requestBody);

                // Retornar la factura generada
                return new OkObjectResult(recibo);
            }
            catch (ArgumentException ex)
            {
                logger.LogError($"Solicitud inválida: {ex.Message}");
                return new BadRequestObjectResult($"Solicitud inválida: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error interno del servidor: {ex.Message}");
                return new ObjectResult("Error interno del servidor.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
    }

    // DTO para la solicitud del endpoint
    public class CalculateInvoiceRequestDTO
    {
        public int Year { get; set; }
        public int Mes { get; set; }
        public int IdService { get; set; }
    }
}
