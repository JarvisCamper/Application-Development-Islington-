namespace JournalApp.Entities
{

    /// Base entity class with common properties for all domain entities
    
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
