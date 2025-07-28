using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Models.Problems;
using Pfm.Application.Common;
using Pfm.Application.Interfaces;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using Pfm.Application.UseCases.Shared;
using System.ComponentModel.DataAnnotations;
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

        /*[HttpPost("import")]
        [Consumes("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        [ProducesResponseType(typeof(BusinessProblem), 440)]
        public async Task<IActionResult> ImportCategories([FromBody] string csvContent)
        {
            await _mediator.Send(new ImportCategoriesCommand(csvContent));
            return Ok();
        }*/

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        [ProducesResponseType(typeof(BusinessProblem), 440)]
        public async Task<IActionResult> ImportCategoriesFromFile([FromForm] FileImportDto dto, [FromServices] IFileValidator validator)
        {
            validator.Validate(dto.File);
            using var stream = new StreamReader(dto.File.OpenReadStream());
            var csvContent = await stream.ReadToEndAsync();
            await _mediator.Send(new ImportCategoriesCommand(csvContent));
            return Ok();
        }
    }
}
