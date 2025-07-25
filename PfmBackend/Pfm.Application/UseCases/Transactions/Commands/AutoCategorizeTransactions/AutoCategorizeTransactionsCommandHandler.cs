using MediatR;
using Pfm.Application.Interfaces;


namespace Pfm.Application.UseCases.Transactions.Commands.AutoCategorizeTransactions
{
    public class AutoCategorizeTransactionsHandler : IRequestHandler<AutoCategorizeTransactionsCommand, AutoCategorizeResultDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IRulesProvider _rulesProvider;
        private readonly IRulesToSqlTranslator _sqlTranslator;

        public AutoCategorizeTransactionsHandler(IUnitOfWork uow, IRulesProvider rulesProvider, IRulesToSqlTranslator sqlTranslator)
        {
            _uow = uow;
            _rulesProvider = rulesProvider;
            _sqlTranslator = sqlTranslator;
        }

        public async Task<AutoCategorizeResultDto> Handle(AutoCategorizeTransactionsCommand request, CancellationToken ct)
        {
            // 1: Get total uncategorized transactions before processing
            int initialUncategorized = await _uow.Transactions.CountUncategorizedAsync(ct);

            if (initialUncategorized == 0)
            {
                return new AutoCategorizeResultDto(0, 0, 0.0);
            }

            // 2: Apply rules via SQL
            var rules = _rulesProvider.GetRules();
            var (sql, parameters) = _sqlTranslator.Translate(rules);
            await _uow.Transactions.ExecuteUpdateAsync(sql, parameters);

            // Calculate results
            int remainingUncategorized = await _uow.Transactions.CountUncategorizedAsync(ct);
            int trulyCategorized = initialUncategorized - remainingUncategorized;
            double percentage = Math.Round((trulyCategorized / (double)initialUncategorized) * 100, 2);

            return new AutoCategorizeResultDto(initialUncategorized, trulyCategorized, percentage);
        }
    }
}
