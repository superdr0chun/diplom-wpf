using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DjangoWPFClient.Models;
using DjangoWPFClient.Services;

namespace DjangoWPFClient.Views
{
    public class ResultVM
    {
        public int Id { get; set; }
        public string TestTitle { get; set; } = "";
        public string ScoreText { get; set; } = "";
        public string DateText { get; set; } = "";
        public int Grade { get; set; }
        public Brush GradeColor { get; set; } = Brushes.Gray;
    }

    public partial class MyResultsWindow : Window
    {
        private readonly AuthService _authService = new();

        public MyResultsWindow()
        {
            InitializeComponent();
            UserLabel.Text = $"👤 {UserSession.Current.Username}";
            LoadResults();
        }

        private async void LoadResults()
{
    LoadingText.Visibility = Visibility.Visible;
    ResultsListView.Visibility = Visibility.Collapsed;
    EmptyText.Visibility = Visibility.Collapsed;

    var results = await _authService.GetResultsAsync();

    LoadingText.Visibility = Visibility.Collapsed;

    if (results.Count == 0)
    {
        EmptyText.Visibility = Visibility.Visible;
        return;
    }

    var success = (Brush)Application.Current.Resources["Success"];
    var primary = (Brush)Application.Current.Resources["Primary"];
    var warning = (Brush)Application.Current.Resources["Warning"];
    var danger = (Brush)Application.Current.Resources["Danger"];

    var items = new List<ResultVM>();
    foreach (var r in results)
    {
        Brush color = r.Grade switch
        {
            5 => success,
            4 => primary,
            3 => warning,
            _ => danger,
        };

        string dateText = "";
        if (!string.IsNullOrEmpty(r.FinishedAt) &&
            DateTime.TryParse(r.FinishedAt, out DateTime dt))
        {
            dateText = dt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
        }

        items.Add(new ResultVM
        {
            Id = r.Id,
            TestTitle = r.TestTitle,
            ScoreText = $"{r.Score}/{r.MaxScore}",
            DateText = dateText,
            Grade = r.Grade,
            GradeColor = color,
        });
    }

    ResultsListView.ItemsSource = items;
    ResultsListView.Visibility = Visibility.Visible;
}

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadResults();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var testListWindow = new TestListWindow();
            testListWindow.Show();
            this.Close();
        }
        private void ResultBorder_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
{
    if (sender is System.Windows.Controls.Border border && border.Tag is int resultId)
    {
        var detailWindow = new ResultDetailWindow(resultId);
        detailWindow.Show();
        this.Close();
    }
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