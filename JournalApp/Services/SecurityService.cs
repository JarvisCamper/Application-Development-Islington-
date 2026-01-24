using JournalApp.Data;
using JournalApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly AppDbContext _context;
        private readonly ISessionStorage _sessionStorage;
        private bool _isAuthenticated = false;

        private const string AuthTokenKey = "AuthToken";
        private const string AuthExpirationKey = "AuthExpiration";
        private const int RememberMeDays = 30;

        public bool IsAuthenticated => _isAuthenticated;

        public SecurityService(AppDbContext context, ISessionStorage sessionStorage)
        {
            _context = context;
            _sessionStorage = sessionStorage;
        }

        public async Task LoginAsync(bool rememberMe)
        {
            _isAuthenticated = true;
            if (rememberMe)
            {
                var expirationDate = DateTime.UtcNow.AddDays(RememberMeDays);
                await _sessionStorage.SetAsync(AuthTokenKey, "valid");
                await _sessionStorage.SetAsync(AuthExpirationKey, expirationDate.ToString("O"));
            }
        }

        public void Logout()
        {
            _isAuthenticated = false;
            _sessionStorage.Remove(AuthTokenKey);
            _sessionStorage.Remove(AuthExpirationKey);
        }

        public async Task InitializeAsync()
        {
            var token = await _sessionStorage.GetAsync(AuthTokenKey);
            var expirationStr = await _sessionStorage.GetAsync(AuthExpirationKey);

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(expirationStr))
            {
                if (DateTime.TryParse(expirationStr, out var expiration))
                {
                    if (expiration > DateTime.UtcNow)
                    {
                        _isAuthenticated = true;
                        return;
                    }
                }
                Logout();
            }
        }

        public async Task<bool> IsPasswordSetAsync()
        {
            return await _context.UserSecurities.AnyAsync();
        }

        public async Task SetupPasswordAsync(string password, string displayName, string answer1, string answer2, string answer3)
        {
            if (await IsPasswordSetAsync())
            {
                throw new InvalidOperationException("Password is already set. Use ChangePasswordAsync to update it.");
            }

            var userSecurity = new UserSecurity
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Salt = string.Empty,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Journalist" : displayName.Trim(),
                SecurityAnswer1Hash = BCrypt.Net.BCrypt.HashPassword(answer1.ToLower().Trim()),
                SecurityAnswer2Hash = BCrypt.Net.BCrypt.HashPassword(answer2.ToLower().Trim()),
                SecurityAnswer3Hash = BCrypt.Net.BCrypt.HashPassword(answer3.ToLower().Trim())
            };

            _context.UserSecurities.Add(userSecurity);
            await _context.SaveChangesAsync();
            
            await LoginAsync(true);
        }

        public async Task<string> GetUserDisplayNameAsync()
        {
            var user = await _context.UserSecurities.AsNoTracking().FirstOrDefaultAsync();
            return user?.DisplayName ?? "Journalist";
        }

        public async Task<bool> VerifyPasswordAsync(string password)
        {
            var userSecurity = await _context.UserSecurities.FirstOrDefaultAsync();
            if (userSecurity == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, userSecurity.PasswordHash);
        }

        public async Task ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (!await VerifyPasswordAsync(currentPassword))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            var userSecurity = await _context.UserSecurities.FirstOrDefaultAsync();
            if (userSecurity == null)
            {
                throw new InvalidOperationException("No password is set.");
            }

            userSecurity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.UserSecurities.Update(userSecurity);
            await _context.SaveChangesAsync();
        }

        public async Task<(string Q1, string Q2, string Q3)> GetSecurityQuestionsAsync()
        {
            var userSecurity = await _context.UserSecurities.FirstOrDefaultAsync();
            if (userSecurity == null)
            {
                return ("", "", "");
            }
            return (userSecurity.SecurityQuestion1, userSecurity.SecurityQuestion2, userSecurity.SecurityQuestion3);
        }

        public async Task<bool> VerifySecurityAnswersAsync(string answer1, string answer2, string answer3)
        {
            var userSecurity = await _context.UserSecurities.FirstOrDefaultAsync();
            if (userSecurity == null) return false;

            var a1Valid = BCrypt.Net.BCrypt.Verify(answer1.ToLower().Trim(), userSecurity.SecurityAnswer1Hash);
            var a2Valid = BCrypt.Net.BCrypt.Verify(answer2.ToLower().Trim(), userSecurity.SecurityAnswer2Hash);
            var a3Valid = BCrypt.Net.BCrypt.Verify(answer3.ToLower().Trim(), userSecurity.SecurityAnswer3Hash);

            return a1Valid && a2Valid && a3Valid;
        }

        public async Task ResetPasswordAsync(string newPassword)
        {
            var userSecurity = await _context.UserSecurities.FirstOrDefaultAsync();
            if (userSecurity == null)
            {
                throw new InvalidOperationException("No password is set.");
            }

            userSecurity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.UserSecurities.Update(userSecurity);
            await _context.SaveChangesAsync();
        }
    }
}
