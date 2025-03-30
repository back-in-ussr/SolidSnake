using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolidSnake
{
    public class GameScores
    {
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }
    }

    public class ScoreManager
    {
        private const string ScoresFileName = "highscores.json";
        private readonly string _filePath;
        private List<GameScores> _scores;

        public ScoreManager()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "SolidSnake");

            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, ScoresFileName);

            _scores = LoadScores();
        }

        public void AddScore(GameScores score)
        {
            if (score == null)
                throw new ArgumentNullException(nameof(score));

            _scores.Add(score);
            _scores = _scores
                .OrderByDescending(s => s.Score)
                .ThenBy(s => s.Date)
                .Take(10)
                .ToList();

            SaveScores();
        }

        public IEnumerable<GameScores> GetAllScores()
        {
            return _scores.OrderByDescending(s => s.Score);
        }

        public IEnumerable<GameScores> GetTopScores(int count = 10)
        {
            return _scores.Take(count);
        }

        public void ClearScores()
        {
            _scores.Clear();
            SaveScores();
        }

        private List<GameScores> LoadScores()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<GameScores>();

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<GameScores>>(json)
                       ?? new List<GameScores>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading scores: {ex.Message}");
                return new List<GameScores>();
            }
        }

        private void SaveScores()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var json = JsonSerializer.Serialize(_scores, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving scores: {ex.Message}");
            }
        }
    }
}

