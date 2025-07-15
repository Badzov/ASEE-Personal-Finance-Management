namespace Pfm.Infrastructure.Exceptions
{
    public class ConcurrentUpdateException : PersistenceException
    {
        public ConcurrentUpdateException() : base("update", "Concurrent modification detected") { }
    }
}
