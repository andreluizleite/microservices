using RuleEngine.Application.DTOs;
using RuleEngine.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace RuleEngine.Application.Services
{
    public class InMemoryRulePersistenceService : IRulePersistenceService
    {
        private readonly string _directory = Path.Combine(AppContext.BaseDirectory, "data", "rules");

        public InMemoryRulePersistenceService()
        {
            Directory.CreateDirectory(_directory);
        }

        public async Task SaveAsync(string id, RuleTreeDto dto)
        {
            var filePath = GetFilePath(id);
            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<RuleTreeDto?> LoadAsync(string id)
        {
            var filePath = GetFilePath(id);
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<RuleTreeDto>(json);
        }

        private string GetFilePath(string id) => Path.Combine(_directory, $"{id}.json");
    }
}
