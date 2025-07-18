using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Application.Common;
using Pfm.Domain.Common;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Transactions.Queries.GetTransactions;
using System.Text;
using Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction;
using Pfm.Api.Models.Problems;

namespace Pfm.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<TransactionDto>), 200)]
        [ProducesResponseType(typeof(ValidationProblem),400)]
        [ProducesResponseType(typeof(BusinessProblem), 440)]
        public async Task<IActionResult> GetTransactions(
            [FromQuery(Name = "kinds")] List<string>? kindStrings,
            [FromQuery(Name = "start-date")] DateTime? startDate,
            [FromQuery(Name = "end-date")] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery(Name = "page-size")] int pageSize = 10,
            [FromQuery(Name = "sort-by")] string? sortBy = null,
            [FromQuery(Name = "sort-order")] string? sortOrder = "asc")
        {
            var filters = TransactionFilters.Create(
                startDate,
                endDate,
                kindStrings,
                sortBy,
                sortOrder,
                page,
                pageSize
            );

            var result = await _mediator.Send(new GetTransactionsQuery(filters));
            return Ok(result);

        }

        [HttpPost("import")]
        [Consumes("text/csv")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        [ProducesResponseType(typeof(BusinessProblem), 440)]
        public async Task<IActionResult> ImportTransactions([FromBody] string csvContent)
        {
            await _mediator.Send(new ImportTransactionsCommand(csvContent));
            return Ok();
        }


        [HttpPost("{id}/categorize")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblem), 400)]
        [ProducesResponseType(typeof(BusinessProblem), 440)]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromBody] TransactionCategoryDto dto)
        {
            await _mediator.Send(new CategorizeTransactionCommand(id, dto.CategoryCode));
            return Ok();
        }
            
    }
}

