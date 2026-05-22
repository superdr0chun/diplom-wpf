using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DjangoWPFClient.Services;

namespace DjangoWPFClient.Views
{
    public partial class ResultDetailWindow : Window
    {
        private readonly AuthService _authService = new();
        private readonly int _resultId;

        public ResultDetailWindow(int resultId)
        {
            InitializeComponent();
            _resultId = resultId;
            LoadDetail();
        }

        private async void LoadDetail()
{
    var detail = await _authService.GetResultDetailAsync(_resultId);
    if (detail == null)
    {
        MessageBox.Show($"Не удалось загрузить детали: {_authService.LastError}");
        Close();
        return;
    }

    TestTitleText.Text = detail.TestTitle;
    ScoreSummaryText.Text = $"Баллов: {detail.Score}/{detail.MaxScore}  •  Вопросов: {detail.Answers.Count}";
    GradeText.Text = detail.Grade.ToString();

    var success = (SolidColorBrush)Application.Current.Resources["Success"];
    var primary = (SolidColorBrush)Application.Current.Resources["Primary"];
    var warning = (SolidColorBrush)Application.Current.Resources["Warning"];
    var danger = (SolidColorBrush)Application.Current.Resources["Danger"];

    GradeBorder.Background = detail.Grade switch
    {
        5 => success,
        4 => primary,
        3 => warning,
        _ => danger,
    };

    int qNum = 1;
    foreach (var ans in detail.Answers)
    {
        AnswersPanel.Children.Add(BuildQuestionCard(ans, qNum));
        qNum++;
    }
}

private Border BuildQuestionCard(ResultAnswerDetail ans, int number)
{
    var success = (SolidColorBrush)Application.Current.Resources["Success"];
    var warning = (SolidColorBrush)Application.Current.Resources["Warning"];
    var danger = (SolidColorBrush)Application.Current.Resources["Danger"];
    var surface = (SolidColorBrush)Application.Current.Resources["Surface"];
    var surface2 = (SolidColorBrush)Application.Current.Resources["Surface2"];
    var border = (SolidColorBrush)Application.Current.Resources["Border"];
    var textStrong = (SolidColorBrush)Application.Current.Resources["TextStrong"];
    var text = (SolidColorBrush)Application.Current.Resources["Text"];
    var textMuted = (SolidColorBrush)Application.Current.Resources["TextMuted"];

    SolidColorBrush statusBrush;
    string statusText;

    if (ans.IsCorrect == true)
    {
        statusBrush = success;
        statusText = "Правильно";
    }
    else if (ans.IsCorrect == false)
    {
        statusBrush = danger;
        statusText = "Ошибка";
    }
    else
    {
        statusBrush = warning;
        statusText = "Ожидает проверки";
    }

    var outerBorder = new Border
    {
        Background = surface,
        CornerRadius = new CornerRadius(10),
        Padding = new Thickness(20),
        Margin = new Thickness(0, 0, 0, 12),
        BorderBrush = statusBrush,
        BorderThickness = new Thickness(4, 0, 0, 0)
    };

    var stack = new StackPanel();

    var titleText = new TextBlock
    {
        Text = $"Вопрос {number}. {ans.QuestionText}",
        FontSize = 14,
        FontWeight = FontWeights.SemiBold,
        Foreground = textStrong,
        TextWrapping = TextWrapping.Wrap,
        Margin = new Thickness(0, 0, 0, 12)
    };
    stack.Children.Add(titleText);

    var statusBar = new Border
    {
        Background = statusBrush,
        CornerRadius = new CornerRadius(4),
        Padding = new Thickness(10, 5, 10, 5),
        HorizontalAlignment = HorizontalAlignment.Left,
        Margin = new Thickness(0, 0, 0, 12)
    };
    statusBar.Child = new TextBlock
    {
        Text = $"{statusText}  •  {ans.Points} балл(ов)",
        Foreground = Brushes.White,
        FontSize = 11,
        FontWeight = FontWeights.Bold
    };
    stack.Children.Add(statusBar);

    if (ans.QuestionType == "text")
    {
        var box = new Border
        {
            Background = surface2,
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14)
        };
        box.Child = new TextBlock
        {
            Text = string.IsNullOrEmpty(ans.TextAnswer) ? "(нет ответа)" : ans.TextAnswer,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13,
            Foreground = text
        };
        stack.Children.Add(box);
    }
    else
    {
        foreach (var a in ans.AllAnswers)
        {
            bool wasSelected = ans.SelectedAnswerIds.Contains(a.Id);
            var row = new Border
            {
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 6),
                CornerRadius = new CornerRadius(6),
                BorderThickness = new Thickness(1)
            };

            string icon;
            SolidColorBrush rowBg, rowBorder, textColor;

            if (wasSelected && a.IsCorrect)
            {
                icon = "+";
                rowBg = new SolidColorBrush(Color.FromArgb(40, success.Color.R, success.Color.G, success.Color.B));
                rowBorder = success;
                textColor = textStrong;
            }
            else if (wasSelected && !a.IsCorrect)
            {
                icon = "x";
                rowBg = new SolidColorBrush(Color.FromArgb(40, danger.Color.R, danger.Color.G, danger.Color.B));
                rowBorder = danger;
                textColor = textStrong;
            }
            else if (!wasSelected && a.IsCorrect)
            {
                icon = "->";
                rowBg = new SolidColorBrush(Color.FromArgb(40, warning.Color.R, warning.Color.G, warning.Color.B));
                rowBorder = warning;
                textColor = textStrong;
            }
            else
            {
                icon = " ";
                rowBg = surface2;
                rowBorder = border;
                textColor = textMuted;
            }

            row.Background = rowBg;
            row.BorderBrush = rowBorder;

            var dock = new DockPanel();
            var iconBlock = new TextBlock
            {
                Text = icon,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = textColor,
                Width = 22,
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(iconBlock, Dock.Left);
            dock.Children.Add(iconBlock);

            var textBlock = new TextBlock
            {
                Text = a.Text,
                FontSize = 13,
                Foreground = textColor,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (wasSelected) textBlock.FontWeight = FontWeights.SemiBold;
            dock.Children.Add(textBlock);

            row.Child = dock;
            stack.Children.Add(row);
        }

        var legend = new TextBlock
        {
            Text = "+ — выбрано и правильно   x — выбрано и неправильно   -> — правильный, не выбран",
            FontSize = 11,
            Foreground = textMuted,
            Margin = new Thickness(0, 8, 0, 0),
            TextWrapping = TextWrapping.Wrap
        };
        stack.Children.Add(legend);
    }

    outerBorder.Child = stack;
    return outerBorder;
}

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new MyResultsWindow();
            win.Show();
            this.Close();
        }
    }
}