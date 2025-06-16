namespace XPMTest.Application.Dtos
{

    public class LoginResponseDto
    {
        public string JWToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsLoggedIn { get; set; }
        public string Name { get; set; }
    }
}
