namespace XPMTest.Application.Dtos
{
    public class UserCreateDto
    {
        public string FullName { get; set; }
        public string SolID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Encrypt later
        public string Department { get; set; }
        public string Role { get; set; }
    }
}
