using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DjangoWPFClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public string BaseUrl { get; set; } = "http://127.0.0.1:8000/api";
        public int UserId { get; private set; }
        public string? Username { get; private set; } // nullable
        public string? LastError { get; private set; } // nullable

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var registerData = new { username, email, password };
                var json = JsonSerializer.Serialize(registerData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/register/", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent, options);

                    if (result != null && result.TryGetValue("success", out JsonElement successElement) &&
                successElement.ValueKind == JsonValueKind.True)
            {
                // Извлекаем ID пользователя
                if (result.TryGetValue("user_id", out JsonElement idElement) &&
                    idElement.ValueKind == JsonValueKind.Number)
                {
                    UserId = idElement.GetInt32();
                }

                // Безопасное извлечение Username
                if (result.TryGetValue("username", out JsonElement usernameElement) &&
            usernameElement.ValueKind != JsonValueKind.Null)
                {
            Username = usernameElement.GetString() ?? string.Empty;
                }

                return true;
            }
            else
            {
                LastError = "Неизвестный ответ сервера";
                return false;
            }
        }
        else
        {
            // Обработка ошибок сервера
            var errorContent = await response.Content.ReadAsStringAsync();
            LastError = errorContent;
            return false;
        }
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
        var loginData = new { username, password };
        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BaseUrl}/login/", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent, options);

            if (result != null && result.TryGetValue("user_id", out JsonElement idElement) &&
                idElement.ValueKind == JsonValueKind.Number)
            {
                UserId = idElement.GetInt32();

                if (result.TryGetValue("username", out JsonElement usernameElement) &&
                    usernameElement.ValueKind != JsonValueKind.Null)
                {
                    Username = usernameElement.GetString() ?? string.Empty;
                }
                return true;
            }
            LastError = "Неизвестный ответ сервера";
            return false;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            LastError = errorContent;
            return false;
        }
    }
    catch (Exception ex)
    {
        LastError = ex.Message;
        return false;
    }
}
}
}