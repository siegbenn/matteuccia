﻿using MassTransit;
using Matteuccia.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Matteuccia.Components
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> _logger;
        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.LogInformation("CONSUMED MESSAGE");
        }
    }
}