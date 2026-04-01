// RegisterWindow.xaml.cs
using System.Windows;
using DjangoWPFClient.Services;

namespace DjangoWPFClient.Views
{
    public partial class RegisterWindow : Window
    {
        private bool _isRegistering = false;
        public AuthService AuthService { get; private set; } = new();
        
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public string Username => UsernameTextBox.Text;
        public string Email => EmailTextBox.Text;
        public string Password => PasswordBox.Password;
        public string ConfirmPassword => ConfirmPasswordBox.Password;

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRegistering) return;
            _isRegistering = true;
            RegisterButton.IsEnabled = false;

            try
            {
                if (string.IsNullOrEmpty(Username))
                {
                    MessageBox.Show("Введите имя пользователя!");
                    return;
                }
                if (string.IsNullOrEmpty(Email))
                {
                    MessageBox.Show("Введите email!");
                    return;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Введите пароль!");
                    return;
                }
                if (Password != ConfirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                // ✅ Регистрация ТОЛЬКО здесь
                bool success = await AuthService.RegisterAsync(Username, Email, Password);
                
                if (success)
                {
                    MessageBox.Show($"Регистрация прошла успешно!\nВаш логин: {Username}");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    if (!string.IsNullOrEmpty(AuthService.LastError))
                    {
                        MessageBox.Show($"Ошибка регистрации: {AuthService.LastError}");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка регистрации. Попробуйте позже.");
                    }
                }
            }
            finally
            {
                _isRegistering = false;
                RegisterButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}