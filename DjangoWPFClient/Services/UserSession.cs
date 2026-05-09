namespace DjangoWPFClient.Models
{
    public class UserSession
    {
        public static UserSession Current { get; } = new UserSession();

        public bool IsLoggedIn { get; set; }
        public string? Username { get; set; }
        public int? UserId { get; set; }
        public string? Role { get; set; }
        public string? AccessToken { get; set; }

        public void Login(string username, int userId, string role, string accessToken)
        {
            IsLoggedIn = true;
            Username = username;
            UserId = userId;
            Role = role;
            AccessToken = accessToken;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            Username = null;
            UserId = null;
            Role = null;
            AccessToken = null;
        }
    }
}