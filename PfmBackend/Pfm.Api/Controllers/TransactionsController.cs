using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Pfm.Application.Services;
using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;
        public TransactionsController(ITransactionService service)
        { 
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        // GET /api/transactions
        public async Task<IActionResult> GetTransactions()
        {
            return Ok(await _service.GetTransactionsAsync());
            
        }

        [HttpPost("import")]
        [Consumes("text/csv")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        // POST /api/transactions/import
        public async Task<IActionResult> ImportTransactions([FromBody] string csvContent)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            await _service.ImportTransactionsAsync(stream);
            return Ok();
        }
    }
}
