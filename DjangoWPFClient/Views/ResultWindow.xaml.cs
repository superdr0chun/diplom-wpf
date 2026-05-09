using System.Windows;
using System.Windows.Media;
using DjangoWPFClient.Models;
using DjangoWPFClient.Services;

namespace DjangoWPFClient.Views
{
    public partial class ResultWindow : Window
    {
        public ResultWindow(TestResult result, string testTitle)
{
    InitializeComponent();

    TestTitleText.Text = testTitle;
    ScoreText.Text = $"{result.Score}/{result.MaxScore}";
    PercentText.Text = $"{result.Percent}%";
    GradeText.Text = result.Grade.ToString();

    var success = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["Success"];
    var primary = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["Primary"];
    var warning = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["Warning"];
    var danger = (System.Windows.Media.SolidColorBrush)Application.Current.Resources["Danger"];

    switch (result.Grade)
    {
        case 5:
            ResultEmoji.Text = "🎉";
            GradeBorder.Background = success;
            break;
        case 4:
            ResultEmoji.Text = "👍";
            GradeBorder.Background = primary;
            break;
        case 3:
            ResultEmoji.Text = "😐";
            GradeBorder.Background = warning;
            break;
        default:
            ResultEmoji.Text = "😔";
            GradeBorder.Background = danger;
            break;
    }
}

        private void BackToTests_Click(object sender, RoutedEventArgs e)
        {
            var testListWindow = new TestListWindow();
            testListWindow.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Current.Logout();
            var main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}