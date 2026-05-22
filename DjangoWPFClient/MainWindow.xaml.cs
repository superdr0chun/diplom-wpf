using System.Windows;
using DjangoWPFClient.Models;
using DjangoWPFClient.Services;
using DjangoWPFClient.Views;

namespace DjangoWPFClient
{
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService = new();

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
            UpdateThemeButton();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            if (registerWindow.ShowDialog() == true)
            {
                UpdateUIForGuestUser();
                MessageBox.Show("Регистрация успешна! Теперь войдите в систему.");
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                if (await _authService.LoginAsync(loginWindow.Username, loginWindow.Password))
                {
                    UserSession.Current.Login(
                        _authService.Username ?? "Unknown",
                        _authService.UserId,
                        _authService.Role ?? "student",
                        _authService.AccessToken ?? ""
                    );
                    UpdateUIForLoggedInUser();
                }
                else
                {
                    MessageBox.Show($"Ошибка входа: {_authService.LastError}");
                }
            }
        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            var testListWindow = new TestListWindow();
            testListWindow.Show();
            this.Hide();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Current.Logout();
            UpdateUIForGuestUser();
            MessageBox.Show("Вы вышли из системы.");
        }
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
{
    Services.ThemeManager.Toggle();
    UpdateThemeButton();
}

private void UpdateThemeButton()
{
    ThemeButton.Content = Services.ThemeManager.CurrentTheme == "Dark" ? "Светлая" : "Тёмная";
}

        private void UpdateUIForLoggedInUser()
        {
            RegisterButton.Visibility = Visibility.Collapsed;
            LoginButton.Visibility = Visibility.Collapsed;
            LogoutButton.Visibility = Visibility.Visible;
            TestsButton.Visibility = Visibility.Visible;
            WelcomeText.Text = $"Добро пожаловать, {UserSession.Current.Username}!";
        }

        private void UpdateUIForGuestUser()
        {
            RegisterButton.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Collapsed;
            TestsButton.Visibility = Visibility.Collapsed;
            WelcomeText.Text = "Войдите в систему";
        }

        private void UpdateUI()
        {
            if (UserSession.Current.IsLoggedIn)
                UpdateUIForLoggedInUser();
            else
                UpdateUIForGuestUser();
        }
    }
}