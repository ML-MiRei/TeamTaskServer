namespace Getaway.Core.Common
{
    public interface IAuditableEntity : IBaseEntity
    {
        public DateTime DateCreated { get; set; }
        public DateTime? LastModified { get; set; }

    }
}
