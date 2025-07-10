using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Transactions.Queries.GetTransactions;
using System.Text;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTransactions()
        {
            var result = await _mediator.Send(new GetTransactionsQuery());
            return Ok(result);
        }

        [HttpPost("import")]
        [Consumes("text/csv")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ImportTransactions([FromBody] string csvContent)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            await _mediator.Send(new ImportTransactionsCommand(stream));
            return Ok();
        }
    }
}

