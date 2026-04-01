// MainWindow.xaml.cs
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
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            if (registerWindow.ShowDialog() == true)
            {
                // ❌ УБРАЛИ повторный вызов RegisterAsync!
                // Регистрация уже прошла в RegisterWindow
                // Просто обновляем UI
                UpdateUIForGuestUser(); // Пользователь ещё не вошёл, только зарегистрировался
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
                    UserSession.Current.Login(_authService.Username ?? "Unknown", _authService.UserId);
                    UpdateUIForLoggedInUser();
                }
                else
                {
                    MessageBox.Show("Ошибка авторизации. Проверьте логин и пароль.");
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Current.Logout();
            WelcomeText.Text = "Войдите в систему";
            UpdateUIForGuestUser();
            MessageBox.Show("Вы вышли из системы.");
        }

        private void UpdateUIForLoggedInUser()
        {
            RegisterButton.Visibility = Visibility.Collapsed;
            LoginButton.Visibility = Visibility.Collapsed;
            LogoutButton.Visibility = Visibility.Visible;
            WelcomeText.Text = $"Добро пожаловать, {UserSession.Current.Username}!";
        }

        private void UpdateUIForGuestUser()
        {
            RegisterButton.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Collapsed;
            WelcomeText.Text = "Войдите в систему";
        }

        private void UpdateUI()
        {
            if (UserSession.Current.IsLoggedIn)
            {
                UpdateUIForLoggedInUser();
            }
            else
            {
                UpdateUIForGuestUser();
            }
        }
    }
}