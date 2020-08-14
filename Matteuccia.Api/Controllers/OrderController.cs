using System;
using System.Threading.Tasks;
using MassTransit;
using Matteuccia.Api.Dto;
using Matteuccia.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Matteuccia.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderController(ILogger<OrderController> logger, IConfiguration configuration, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _sendEndpointProvider = sendEndpointProvider;
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new order submission request",
            Description = "Order submission request is processed with the order worker service"
        )]
        [SwaggerResponse(202, "The order submission was created", typeof(SubmitOrderResponseDto))]
        [SwaggerResponse(400, "The order submission is invalid")]
        public async Task<IActionResult> CreateOrder([FromBody, SwaggerRequestBody("The order submission payload", Required = true)] SubmitOrderDto orderDto)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_configuration["MassTransitEndpoints:SubmitOrderEndpoint"]));
            
            var id = Guid.NewGuid();
            
            await endpoint.Send<SubmitOrder>(new
            {
                Id = id
            });

            _logger.LogInformation("Submitted Order with ID: {id}", id);

            return Accepted(new SubmitOrderResponseDto
                {
                    Id = id
                }
            );
        }
    }
}