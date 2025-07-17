using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Models.Problems;
using Pfm.Application.Common;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using System.Text;

namespace Pfm.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("import")]
        [Consumes("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        [ProducesResponseType(typeof(BusinessProblem), 440)]
        public async Task<IActionResult> ImportCategories([FromBody] string csvContent)
        {
            await _mediator.Send(new ImportCategoriesCommand(csvContent));
            return Ok();
        }
    }
}
