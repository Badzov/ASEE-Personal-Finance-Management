using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Schemas;
using Pfm.Application.Common;
using Pfm.Application.UseCases.SpendingAnalytics.Queries;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 422)]
        public async Task<IActionResult> GetSpendingAnalytics(
            [FromQuery] string? catcode,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? direction)
        {
            try
            {
                var query = new GetSpendingAnalyticsQuery(catcode, startDate, endDate, direction);
                var result = await _mediator.Send(query);
                return Ok(new { groups = result }); // Matches OAS schema
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e =>
                    new AppError(e.PropertyName, e.ErrorCode, e.ErrorMessage));
                return BadRequest(new ApiErrorResponse(errors));
            }
        }
    }
}
