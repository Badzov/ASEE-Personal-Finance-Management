using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Models.Problems;
using Pfm.Application.Common;
using Pfm.Application.UseCases.SpendingAnalytics.Queries;

namespace Pfm.Api.Controllers
{
    [ApiController]
    [Route("spending-analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SpendingAnalysisDto>), 200)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        public async Task<IActionResult> GetSpendingAnalytics(
            [FromQuery] string? catcode,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? direction)
        {
            var query = new GetSpendingAnalyticsQuery(catcode, startDate, endDate, direction);
            var result = await _mediator.Send(query);
            return Ok(new { groups = result }); // Matches OAS schema
        }
            
    }
}
