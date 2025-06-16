namespace XPMTest.Application.Dtos
{

    public class UserRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                    return string.Empty;

                string firstName = "", lastName = "";

                if (FullName.Contains(',')) // "FirstName, LastName"
                {
                    var parts = FullName.Split(',');
                    if (parts.Length == 2)
                    {
                        firstName = parts[0].Trim();
                        lastName = parts[1].Trim();
                    }
                }
                else if (FullName.Contains(' ')) // "FirstName LastName"
                {
                    var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        firstName = parts[0].Trim();
                        lastName = parts[1].Trim();
                    }
                }

                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                    return $"{lastName.ToLower()}.{firstName.ToLower()}";

                return FullName.ToLower(); // fallback for single names
            }
        }


        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
