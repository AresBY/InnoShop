using InnoShop.Users.Domain.Enums;

namespace InnoShop.Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.None;
        public bool IsActive { get; set; } = true;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiryTime { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public DateTime? EmailConfirmationTokenExpiryTime { get; set; }

    }
}
