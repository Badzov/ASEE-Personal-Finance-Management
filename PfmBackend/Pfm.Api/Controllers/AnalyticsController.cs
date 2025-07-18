using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Models.Problems;
using Pfm.Application.Common;
using Pfm.Application.UseCases.Queries;
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
        [ProducesResponseType(typeof(SpendingsByCategoryDto), 200)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        public async Task<IActionResult> GetSpendingsByCategory(
            [FromQuery(Name = "cat-code")] string? catCode,
            [FromQuery(Name = "start-date")] DateTime? startDate,
            [FromQuery(Name = "end-date")] DateTime? endDate,
            [FromQuery(Name = "direction")] string? direction)
        {

            // This part is somewhat interesting, the start and end dates with small integer values silently turn into null, so here, we turn them into junk 
            // so our validator can handle and not just let it pass as a null

            if (Request.Query.ContainsKey("start-date") && !startDate.HasValue)
                startDate = DateTime.MinValue; // junk date 

            if (Request.Query.ContainsKey("end-date") && !endDate.HasValue)
                endDate = DateTime.MinValue; // junk date

            var result = await _mediator.Send(new GetSpendingAnalyticsQuery(
                catCode,
                startDate,
                endDate,
                direction
            ));
            return Ok(result);
        }

    }
}
