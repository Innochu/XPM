namespace XPMTest.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string PasswordResetToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}