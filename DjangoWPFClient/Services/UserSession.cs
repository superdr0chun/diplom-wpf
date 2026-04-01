namespace DjangoWPFClient.Models
{
    public class UserSession
    {
        public static UserSession Current { get; } = new UserSession();
        public bool IsLoggedIn { get; set; }
        public string? Username { get; set; }
        public int? UserId { get; set; }

        public void Login(string username, int userId)
        {
            IsLoggedIn = true;
            Username = username;
            UserId = userId;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            Username = null;
            UserId = null;
        }
    }
}
