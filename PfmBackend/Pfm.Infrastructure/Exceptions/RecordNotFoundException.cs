namespace Pfm.Infrastructure.Exceptions
{
    public class RecordNotFoundException : PersistenceException
    {
        public string EntityType { get; }
        public string EntityId { get; }

        public RecordNotFoundException(string entityType, string entityId) : base("fetch", $"{entityType} with id {entityId} not found")
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}
