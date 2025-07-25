namespace Pfm.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITransactionRepository Transactions { get; }
        ICategoryRepository Categories { get; }
        ISplitRepository Splits { get; }
        Task<int> CompleteAsync();
    }
}
