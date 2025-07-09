using Microsoft.AspNetCore.Mvc;
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
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;
        public TransactionsController(ITransactionService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            return Ok(await _service.GetTransactionsAsync());
            
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportTransactions([FromBody] List<Transaction> transactions)
        {
            await _service.ImportTransactionsAsync(transactions);
            return Ok($"{transactions.Count} transactions imported.");
        }
    }
}
