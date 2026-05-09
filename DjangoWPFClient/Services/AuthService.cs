using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using DjangoWPFClient.Models;
using System.Text.Json.Serialization;
namespace DjangoWPFClient.Services
{
    public class AnswerModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}

public class QuestionModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
    [JsonPropertyName("question_type")]
    public string QuestionType { get; set; } = "single";
    [JsonPropertyName("points")]
    public int Points { get; set; }
    [JsonPropertyName("answers")]
    public List<AnswerModel> Answers { get; set; } = new();
}

public class TestListItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = "";
    [JsonPropertyName("time_limit")]
    public int TimeLimit { get; set; }
    [JsonPropertyName("attempts_allowed")]
    public int AttemptsAllowed { get; set; }
    [JsonPropertyName("questions_count")]
    public int QuestionsCount { get; set; }
}

public class StartedTest
{
    [JsonPropertyName("result_id")]
    public int ResultId { get; set; }
    [JsonPropertyName("test_id")]
    public int TestId { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
    [JsonPropertyName("time_limit")]
    public int TimeLimit { get; set; }
    [JsonPropertyName("questions")]
    public List<QuestionModel> Questions { get; set; } = new();
}

public class SubmitAnswerModel
{
    public int QuestionId { get; set; }
    public List<int> SelectedAnswerIds { get; set; } = new();
    public string TextAnswer { get; set; } = "";
}

public class TestResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("score")]
    public float Score { get; set; }
    [JsonPropertyName("max_score")]
    public float MaxScore { get; set; }
    [JsonPropertyName("grade")]
    public int Grade { get; set; }
    [JsonPropertyName("percent")]
    public float Percent { get; set; }
}
public class ResultItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("test_title")]
    public string TestTitle { get; set; } = "";
    [JsonPropertyName("score")]
    public float Score { get; set; }
    [JsonPropertyName("max_score")]
    public float MaxScore { get; set; }
    [JsonPropertyName("grade")]
    public int Grade { get; set; }
    [JsonPropertyName("finished_at")]
    public string? FinishedAt { get; set; }
}
public class ResultAnswerDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("question_id")]
    public int QuestionId { get; set; }
    [JsonPropertyName("question_text")]
    public string QuestionText { get; set; } = "";
    [JsonPropertyName("question_type")]
    public string QuestionType { get; set; } = "";
    [JsonPropertyName("points")]
    public int Points { get; set; }
    [JsonPropertyName("all_answers")]
    public List<AllAnswerInfo> AllAnswers { get; set; } = new();
    [JsonPropertyName("selected_answer_ids")]
    public List<int> SelectedAnswerIds { get; set; } = new();
    [JsonPropertyName("text_answer")]
    public string TextAnswer { get; set; } = "";
    [JsonPropertyName("is_correct")]
    public bool? IsCorrect { get; set; }
}

public class AllAnswerInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }
}

public class ResultDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";
    [JsonPropertyName("test_title")]
    public string TestTitle { get; set; } = "";
    [JsonPropertyName("score")]
    public float Score { get; set; }
    [JsonPropertyName("max_score")]
    public float MaxScore { get; set; }
    [JsonPropertyName("grade")]
    public int Grade { get; set; }
    [JsonPropertyName("answers")]
    public List<ResultAnswerDetail> Answers { get; set; } = new();
}
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public string BaseUrl { get; set; } = "http://127.0.0.1:8000/api";
        public int UserId { get; private set; }
        public string? Username { get; private set; }
        public string? Role { get; private set; }
        public string? AccessToken { get; private set; }
        public string? LastError { get; private set; }

        public AuthService()
{
    _httpClient = new HttpClient();
    // Берём токен из сессии если есть
    if (!string.IsNullOrEmpty(UserSession.Current.AccessToken))
    {
        AccessToken = UserSession.Current.AccessToken;
    }
}

private void SetAuthHeader()
{
    var token = AccessToken ?? UserSession.Current.AccessToken;
    if (!string.IsNullOrEmpty(token))
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var data = new { username, email, password };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{BaseUrl}/register/", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent, options);

                if (response.IsSuccessStatusCode && result != null &&
                    result.TryGetValue("success", out var s) && s.GetBoolean())
                {
                    return true;
                }
                LastError = responseContent;
                return false;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var data = new { username, password };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{BaseUrl}/login/", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent, options);

                if (response.IsSuccessStatusCode && result != null)
                {
                    UserId = result["user_id"].GetInt32();
                    Username = result["username"].GetString() ?? "";
                    Role = result["role"].GetString() ?? "student";
                    AccessToken = result["access"].GetString() ?? "";
                    return true;
                }
                LastError = result?["error"].GetString() ?? "Ошибка входа";
                return false;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public async Task<List<TestListItem>> GetTestsAsync()
{
    try
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"{BaseUrl}/tests/");
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var tests = JsonSerializer.Deserialize<List<TestListItem>>(responseContent, options);
        var result = tests ?? new List<TestListItem>();

        // Сохраняем в кэш
        OfflineCache.SaveTests(result);
        IsOnline = true;
        return result;
    }
    catch (Exception ex)
    {
        LastError = ex.Message;
        IsOnline = false;
        // Возвращаем кэш
        return OfflineCache.LoadTests();
    }
}

public bool IsOnline { get; private set; } = true;

public async Task<int> SyncPendingResultsAsync()
{
    var pending = OfflineCache.LoadPendingResults();
    if (pending.Count == 0) return 0;

    int synced = 0;
    var stillPending = new List<PendingResult>();

    foreach (var p in pending)
    {
        var result = await SubmitTestAsync(p.ResultId, p.Answers, p.TabSwitches);
        if (result != null)
            synced++;
        else
            stillPending.Add(p);
    }

    OfflineCache.SavePendingResults(stillPending);
    return synced;
}
public async Task<ResultDetail?> GetResultDetailAsync(int resultId)
{
    try
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"{BaseUrl}/results/{resultId}/");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            LastError = responseContent;
            return null;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ResultDetail>(responseContent, options);
    }
    catch (Exception ex)
    {
        LastError = ex.Message;
        return null;
    }
}
        public async Task<StartedTest?> StartTestAsync(int testId)
        {
            try
            {
                SetAuthHeader();
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{BaseUrl}/tests/{testId}/start/", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var options2 = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var err = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent, options2);
                    LastError = err?["error"].GetString() ?? "Ошибка старта теста";
                    return null;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var test = JsonSerializer.Deserialize<StartedTest>(responseContent, options);
                return test;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }
        public async Task<List<ResultItem>> GetResultsAsync()
{
    try
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"{BaseUrl}/results/");
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var results = JsonSerializer.Deserialize<List<ResultItem>>(responseContent, options);
        return results ?? new List<ResultItem>();
    }
    catch (Exception ex)
    {
        LastError = ex.Message;
        return new List<ResultItem>();
    }
}

        public async Task<TestResult?> SubmitTestAsync(int resultId, List<SubmitAnswerModel> answers, int tabSwitches = 0)
{
    try
    {
        SetAuthHeader();
        var data = new
        {
            result_id = resultId,
            tab_switches = tabSwitches,
            answers = answers.Select(a => new
            {
                question_id = a.QuestionId,
                selected_answer_ids = a.SelectedAnswerIds,
                text_answer = a.TextAnswer
            }).ToList()
        };
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/tests/submit/", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            LastError = responseContent;
            return null;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<TestResult>(responseContent, options);
        return result;
    }
    catch (Exception ex)
    {
        LastError = ex.Message;
        return null;
    }
}
    }
}