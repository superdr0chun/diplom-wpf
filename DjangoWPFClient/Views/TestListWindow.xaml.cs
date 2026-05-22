using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DjangoWPFClient.Models;
using DjangoWPFClient.Services;
using System.Linq;
namespace DjangoWPFClient.Views
{
    public class TestListItemVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Subject { get; set; } = "";
        public string TimeLimitText { get; set; } = "";
        public string QuestionsText { get; set; } = "";
        public string AttemptsText { get; set; } = "";
    }

    public partial class TestListWindow : Window
    {
        private readonly AuthService _authService = new();
        private List<TestListItemVM> _allTests = new();
        public TestListWindow()
        {
            InitializeComponent();
            UserLabel.Text = $"👤 {UserSession.Current.Username}";
            LoadTests();
            UpdateThemeButton();
        }

private async void LoadTests()
{
    LoadingText.Visibility = Visibility.Visible;
    TestsListView.Visibility = Visibility.Collapsed;
    EmptyText.Visibility = Visibility.Collapsed;

    var tests = await _authService.GetTestsAsync();

    LoadingText.Visibility = Visibility.Collapsed;

    // Обновляем индикатор сети
    UpdateNetworkStatus();

    // Если онлайн — пытаемся синхронизировать накопленные результаты
    if (_authService.IsOnline)
    {
        var pending = OfflineCache.LoadPendingResults();
        if (pending.Count > 0)
        {
            int synced = await _authService.SyncPendingResultsAsync();
            if (synced > 0)
            {
                MessageBox.Show($"Синхронизировано результатов: {synced}");
            }
        }
    }

    _allTests = new List<TestListItemVM>();
    foreach (var t in tests)
    {
        _allTests.Add(new TestListItemVM
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Subject = t.Subject,
            TimeLimitText = t.TimeLimit > 0 ? $"{t.TimeLimit} мин." : "Без ограничения",
            QuestionsText = $"{t.QuestionsCount} вопр.",
            AttemptsText = $"{t.AttemptsAllowed} поп.",
        });
    }

    ApplyFilter();
}

private void UpdateNetworkStatus()
{
    var pending = OfflineCache.LoadPendingResults();
    if (_authService.IsOnline)
    {
        NetworkStatus.Background = (System.Windows.Media.Brush)
            Application.Current.Resources["Success"];
        NetworkStatusText.Text = pending.Count > 0
            ? $"● Онлайн (синхронизация {pending.Count})"
            : "● Онлайн";
    }
    else
    {
        NetworkStatus.Background = (System.Windows.Media.Brush)
            Application.Current.Resources["Danger"];
        NetworkStatusText.Text = pending.Count > 0
            ? $"● Оффлайн ({pending.Count} ждут)"
            : "● Оффлайн";
    }
}

private void ApplyFilter()
{
    string query = SearchBox?.Text?.Trim().ToLower() ?? "";

    var filtered = string.IsNullOrEmpty(query)
        ? _allTests
        : _allTests.Where(t =>
            t.Title.ToLower().Contains(query) ||
            t.Subject.ToLower().Contains(query)).ToList();

    if (filtered.Count == 0)
    {
        TestsListView.Visibility = Visibility.Collapsed;
        EmptyText.Text = string.IsNullOrEmpty(query)
            ? "Нет доступных тестов."
            : "Тесты не найдены.";
        EmptyText.Visibility = Visibility.Visible;
    }
    else
    {
        EmptyText.Visibility = Visibility.Collapsed;
        TestsListView.ItemsSource = filtered;
        TestsListView.Visibility = Visibility.Visible;
    }
}

private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
{
    ApplyFilter();
}
        private async void StartTest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int testId)
            {
                btn.IsEnabled = false;
                btn.Content = "Загрузка...";

                var startedTest = await _authService.StartTestAsync(testId);

                if (startedTest == null)
                {
                    MessageBox.Show($"Ошибка: {_authService.LastError}");
                    btn.IsEnabled = true;
                    btn.Content = "Начать тест";
                    return;
                }

                var testWindow = new TestWindow(startedTest);
                testWindow.Show();
                this.Hide();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTests();
        }
        private void MyResultsButton_Click(object sender, RoutedEventArgs e)
{
    var myResults = new MyResultsWindow();
    myResults.Show();
    this.Close();
}

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Current.Logout();
            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void TestsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TestsListView.SelectedItem = null;
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    if (UserSession.Current.IsLoggedIn && Application.Current.Windows.Count == 1)
    {
        Application.Current.Shutdown();
    }
}
    }
}