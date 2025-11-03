using PRN232.Lab2.CoffeeStore.Services.Configuration;
using PRN232.Lab2.CoffeeStore.Services.Models.User;

namespace PRN232.Lab2.CoffeeStore.Services.AuthService
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId, string accessToken, TimeSpan accessTokenTtl);
        Task<UserProfileResponse> GetUserProfileAsync(string userId);
    }
}
