namespace InnoShop.Users.Application.DTOs.Auth
{
    public record AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
