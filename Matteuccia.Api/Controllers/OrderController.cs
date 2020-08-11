using System;
using System.Threading.Tasks;
using MassTransit;
using Matteuccia.Api.Dto;
using Matteuccia.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Matteuccia.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderController(ILogger<OrderController> logger, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto orderDto)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:submit-order"));
            
            var id = Guid.NewGuid();
            
            await endpoint.Send<SubmitOrder>(new
            {
                Id = id
            });

            return Accepted(new SubmitOrderResponseDto
                {
                    Id = id
                }
            );
        }
    }
}