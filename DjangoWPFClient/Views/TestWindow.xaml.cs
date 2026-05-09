using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DjangoWPFClient.Services;

namespace DjangoWPFClient.Views
{
    public class AnswerVM
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public bool IsSelected { get; set; }
    }

    public partial class TestWindow : Window
    {
        private readonly AuthService _authService = new();
        private readonly StartedTest _test;
        private int _currentIndex = 0;
        private readonly Dictionary<int, List<int>> _selectedAnswers = new();
        private readonly Dictionary<int, string> _textAnswers = new();
        private DispatcherTimer? _timer;
        private int _tabSwitches = 0;
        private int _secondsLeft;
        private bool _isFinishing = false;

        public TestWindow(StartedTest test)
        {
            InitializeComponent();
            _test = test;
            TestTitleText.Text = test.Title;

            if (test.TimeLimit > 0)
{
    _secondsLeft = test.TimeLimit * 60;
    StartTimer();
}
else
{
    TimerText.Text = "∞";
    TimerBorder.Background = (SolidColorBrush)Application.Current.Resources["Success"];
}

            ShowQuestion(0);
            this.Deactivated += TestWindow_Deactivated;
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
            UpdateTimerDisplay();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _secondsLeft--;
            UpdateTimerDisplay();
            if (_secondsLeft <= 0)
            {
                _timer?.Stop();
                MessageBox.Show("Время вышло! Тест будет завершён.");
                FinishTest();
            }
        }

        private void UpdateTimerDisplay()
{
    int minutes = _secondsLeft / 60;
    int seconds = _secondsLeft % 60;
    TimerText.Text = $"{minutes:D2}:{seconds:D2}";

    if (_secondsLeft <= 60)
        TimerBorder.Background = (SolidColorBrush)Application.Current.Resources["Danger"];
    else if (_secondsLeft <= 300)
        TimerBorder.Background = (SolidColorBrush)Application.Current.Resources["Warning"];
    else
        TimerBorder.Background = (SolidColorBrush)Application.Current.Resources["Success"];
}
private void TestWindow_Deactivated(object? sender, EventArgs e)
{
    if (_isFinishing) return;
    _tabSwitches++;
    UpdateSwitchIndicator();
}

private void UpdateSwitchIndicator()
{
    if (_tabSwitches > 0)
    {
        SwitchBorder.Visibility = Visibility.Visible;
        SwitchText.Text = $"⚠ Переключений: {_tabSwitches}";
    }
}

        private void ShowQuestion(int index)
        {
            SaveCurrentAnswer();
            _currentIndex = index;
            var question = _test.Questions[index];

            QuestionNumberText.Text = $"Вопрос {index + 1} из {_test.Questions.Count}  •  {question.Points} балл(ов)";
            QuestionText.Text = question.Text;
            ProgressText.Text = $"{index + 1} / {_test.Questions.Count}";

            PrevButton.IsEnabled = index > 0;
            NextButton.IsEnabled = index < _test.Questions.Count - 1;
            FinishButton.Visibility = index == _test.Questions.Count - 1
                ? Visibility.Visible : Visibility.Collapsed;

            if (question.QuestionType == "text")
            {
                AnswersPanel.Visibility = Visibility.Collapsed;
                TextAnswerBorder.Visibility = Visibility.Visible;
                QuestionTypeHint.Text = "Введите развёрнутый ответ";
                TextAnswerBox.Text = _textAnswers.ContainsKey(question.Id)
                    ? _textAnswers[question.Id] : "";
            }
            else
            {
                AnswersPanel.Visibility = Visibility.Visible;
                TextAnswerBorder.Visibility = Visibility.Collapsed;
                QuestionTypeHint.Text = question.QuestionType == "single"
                    ? "Выберите один вариант ответа"
                    : "Выберите один или несколько вариантов ответа";

                var selected = _selectedAnswers.ContainsKey(question.Id)
                    ? _selectedAnswers[question.Id] : new List<int>();

                var answerVMs = question.Answers.Select(a => new AnswerVM
                {
                    Id = a.Id,
                    Text = a.Text,
                    IsSelected = selected.Contains(a.Id)
                }).ToList();

                AnswersPanel.ItemsSource = answerVMs;
                UpdateAnswerStyles();
            }
        }

        private void SaveCurrentAnswer()
        {
            if (_currentIndex >= _test.Questions.Count) return;
            var question = _test.Questions[_currentIndex];

            if (question.QuestionType == "text")
            {
                _textAnswers[question.Id] = TextAnswerBox.Text;
            }
        }

        private void Answer_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border) return;
            if (border.DataContext is not AnswerVM clicked) return;

            var question = _test.Questions[_currentIndex];

            if (!_selectedAnswers.ContainsKey(question.Id))
                _selectedAnswers[question.Id] = new List<int>();

            var selected = _selectedAnswers[question.Id];

            if (question.QuestionType == "single")
            {
                selected.Clear();
                selected.Add(clicked.Id);
                foreach (AnswerVM a in AnswersPanel.Items)
                    a.IsSelected = a.Id == clicked.Id;
            }
            else
            {
                clicked.IsSelected = !clicked.IsSelected;
                if (clicked.IsSelected)
                    selected.Add(clicked.Id);
                else
                    selected.Remove(clicked.Id);
            }

            UpdateAnswerStyles();
        }

        private void UpdateAnswerStyles()
{
    var question = _test.Questions[_currentIndex];
    var selected = _selectedAnswers.ContainsKey(question.Id)
        ? _selectedAnswers[question.Id] : new List<int>();

    var primaryBrush = (SolidColorBrush)Application.Current.Resources["Primary"];
    var borderBrush = (SolidColorBrush)Application.Current.Resources["Border"];
    var surfaceBrush = (SolidColorBrush)Application.Current.Resources["Surface"];

    for (int i = 0; i < AnswersPanel.Items.Count; i++)
    {
        var item = AnswersPanel.Items[i] as AnswerVM;
        if (item == null) continue;

        var element = AnswersPanel.ItemContainerGenerator
            .ContainerFromIndex(i) as ContentPresenter;
        if (element == null) continue;

        var border = FindVisualChild<Border>(element);
        if (border == null) continue;

        bool isSelected = selected.Contains(item.Id);
        border.BorderBrush = isSelected ? primaryBrush : borderBrush;
    }
}

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result) return result;
                var found = FindVisualChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex > 0) ShowQuestion(_currentIndex - 1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _test.Questions.Count - 1) ShowQuestion(_currentIndex + 1);
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            var unanswered = _test.Questions.Count(q =>
                q.QuestionType == "text"
                    ? !_textAnswers.ContainsKey(q.Id) || string.IsNullOrEmpty(_textAnswers[q.Id])
                    : !_selectedAnswers.ContainsKey(q.Id) || _selectedAnswers[q.Id].Count == 0);

            if (unanswered > 0)
            {
                var res = MessageBox.Show(
                    $"Вы не ответили на {unanswered} вопрос(ов). Завершить тест?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;
            }

            FinishTest();
        }

        private async void FinishTest()
{
    if (_isFinishing) return;
    _isFinishing = true;
    _timer?.Stop();
    SaveCurrentAnswer();

    FinishButton.IsEnabled = false;
    FinishButton.Content = "Отправка...";

    var answers = _test.Questions.Select(q => new SubmitAnswerModel
    {
        QuestionId = q.Id,
        SelectedAnswerIds = _selectedAnswers.ContainsKey(q.Id)
            ? _selectedAnswers[q.Id] : new List<int>(),
        TextAnswer = _textAnswers.ContainsKey(q.Id)
            ? _textAnswers[q.Id] : ""
    }).ToList();

    var result = await _authService.SubmitTestAsync(_test.ResultId, answers, _tabSwitches);

    if (result == null)
    {
        // Сохраняем в очередь оффлайн
        Services.OfflineCache.AddPendingResult(new Services.PendingResult
        {
            ResultId = _test.ResultId,
            Answers = answers,
            TabSwitches = _tabSwitches,
            TestTitle = _test.Title,
        });

        MessageBox.Show(
            "Нет связи с сервером. Результат сохранён локально и будет отправлен автоматически при подключении.",
            "Оффлайн режим",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        var listWindow = new TestListWindow();
        listWindow.Show();
        this.Close();
        return;
    }

    var resultWindow = new ResultWindow(result, _test.Title);
    resultWindow.Show();
    this.Close();
}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isFinishing)
            {
                var res = MessageBox.Show(
                    "Вы уверены что хотите закрыть тест? Прогресс будет потерян.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            _timer?.Stop();
        }
    }
}