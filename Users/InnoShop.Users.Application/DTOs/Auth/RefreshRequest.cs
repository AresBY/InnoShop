namespace InnoShop.Users.Application.DTOs.Auth
{
    public record RefreshRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
