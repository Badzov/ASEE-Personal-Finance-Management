using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Schemas;
using Pfm.Application.Common;
using Pfm.Domain.Common;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Transactions.Queries.GetTransactions;
using System.Text;
using Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction;
using Pfm.Domain.Exceptions;

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
        [ProducesResponseType(typeof(PaginatedResult<TransactionDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse),400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 422)]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery(Name = "kinds")] List<string>? kindStrings,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filters = TransactionFilters.Create(
                    startDate,
                    endDate,
                    kindStrings,
                    page,
                    pageSize
                );

                var result = await _mediator.Send(new GetTransactionsQuery(filters));
                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("is not a valid value"))
            {
                return UnprocessableEntity(new ApiErrorResponse(
                    new[] { new AppError("kinds", "invalid-kind", "Invalid transaction kind value") }
                ));
            }
        }

        [HttpPost("import")]
        [Consumes("text/csv")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 409)]
        [ProducesResponseType(typeof(ApiErrorResponse), 422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ImportTransactions([FromBody] string csvContent)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            await _mediator.Send(new ImportTransactionsCommand(stream));
            return Ok();
        }


        [HttpPost("{id}/categorize")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromBody] TransactionCategoryDto dto)
        {
            try
            {
                await _mediator.Send(new CategorizeTransactionCommand(id, dto.CategoryCode));
                return Ok();
            }
            catch (DomainException ex) when (ex.ErrorCode == "transaction-not-found" || ex.ErrorCode == "category-not-found")
            {
                return NotFound(new ApiErrorResponse([new AppError("categorization", ex.ErrorCode, ex.Message)]));
            }
            catch (DomainException ex)
            {
                return UnprocessableEntity(new ApiErrorResponse([new AppError("categorization", ex.ErrorCode, ex.Message)]));
            }
        }
    }
}

