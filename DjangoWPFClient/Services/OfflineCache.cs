using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DjangoWPFClient.Services
{
    public class PendingResult
    {
        public int ResultId { get; set; }
        public List<SubmitAnswerModel> Answers { get; set; } = new();
        public int TabSwitches { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;
        public string TestTitle { get; set; } = "";
    }

    public static class OfflineCache
    {
        private static readonly string CacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DjangoWPFClient");

        private static string TestsFile => Path.Combine(CacheDir, "tests_cache.json");
        private static string PendingFile => Path.Combine(CacheDir, "pending_results.json");

        private static void EnsureDir()
        {
            if (!Directory.Exists(CacheDir))
                Directory.CreateDirectory(CacheDir);
        }

        public static void SaveTests(List<TestListItem> tests)
        {
            try
            {
                EnsureDir();
                var json = JsonSerializer.Serialize(tests);
                File.WriteAllText(TestsFile, json);
            }
            catch { }
        }

        public static List<TestListItem> LoadTests()
        {
            try
            {
                if (!File.Exists(TestsFile)) return new List<TestListItem>();
                var json = File.ReadAllText(TestsFile);
                return JsonSerializer.Deserialize<List<TestListItem>>(json) ?? new List<TestListItem>();
            }
            catch { return new List<TestListItem>(); }
        }

        public static void AddPendingResult(PendingResult result)
        {
            try
            {
                EnsureDir();
                var pending = LoadPendingResults();
                pending.Add(result);
                var json = JsonSerializer.Serialize(pending);
                File.WriteAllText(PendingFile, json);
            }
            catch { }
        }

        public static List<PendingResult> LoadPendingResults()
        {
            try
            {
                if (!File.Exists(PendingFile)) return new List<PendingResult>();
                var json = File.ReadAllText(PendingFile);
                return JsonSerializer.Deserialize<List<PendingResult>>(json) ?? new List<PendingResult>();
            }
            catch { return new List<PendingResult>(); }
        }

        public static void SavePendingResults(List<PendingResult> pending)
        {
            try
            {
                EnsureDir();
                var json = JsonSerializer.Serialize(pending);
                File.WriteAllText(PendingFile, json);
            }
            catch { }
        }

        public static void ClearPendingResults()
        {
            try
            {
                if (File.Exists(PendingFile)) File.Delete(PendingFile);
            }
            catch { }
        }
    }
}