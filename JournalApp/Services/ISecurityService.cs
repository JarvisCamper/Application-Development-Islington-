namespace JournalApp.Services
{
    /// <summary>
    /// Security service for password management and authentication
    /// </summary>
    public interface ISecurityService
    {
        bool IsAuthenticated { get; }

        Task LoginAsync(bool rememberMe);
        void Logout();
        Task InitializeAsync();

        Task<bool> IsPasswordSetAsync();
        Task SetupPasswordAsync(string password, string displayName, string answer1, string answer2, string answer3);
        Task<bool> VerifyPasswordAsync(string password);
        Task ChangePasswordAsync(string currentPassword, string newPassword);
        Task<string> GetUserDisplayNameAsync();
        
        // Security Questions
        Task<(string Q1, string Q2, string Q3)> GetSecurityQuestionsAsync();
        Task<bool> VerifySecurityAnswersAsync(string answer1, string answer2, string answer3);
        Task ResetPasswordAsync(string newPassword);
    }
}
