namespace JournalApp.Entities
{
    /// <summary>
    /// Represents user security credentials for application authentication
    /// </summary>
    public class UserSecurity : BaseEntity
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        
        // User Profile
        public string DisplayName { get; set; } = "Journalist";
        
        // Security Questions for password recovery
        public string SecurityQuestion1 { get; set; } = "What is your favorite color?";
        public string SecurityAnswer1Hash { get; set; } = string.Empty;
        
        public string SecurityQuestion2 { get; set; } = "What is your pet's name?";
        public string SecurityAnswer2Hash { get; set; } = string.Empty;
        
        public string SecurityQuestion3 { get; set; } = "What city were you born in?";
        public string SecurityAnswer3Hash { get; set; } = string.Empty;
    }
}
