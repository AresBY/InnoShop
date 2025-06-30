namespace InnoShop.Users.Application.Configurations
{
    public class AccountConfirmationSettings
    {
        public int TokenLifetimeMinutes { get; set; }
        public string ConfirmationBaseUrl { get; set; } = null!;
    }
}
