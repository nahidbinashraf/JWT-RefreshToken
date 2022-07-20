namespace JWTAuth.Models
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreatedAt { get; set; } = DateTime.Now;
        public DateTime TokenExpriedOn { get; set; }
    }
}
