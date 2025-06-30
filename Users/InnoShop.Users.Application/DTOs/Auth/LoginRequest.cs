namespace InnoShop.Users.Application.DTOs.Auth
{
    public record LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
