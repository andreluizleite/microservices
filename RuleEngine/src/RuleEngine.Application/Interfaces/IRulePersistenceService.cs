using RuleEngine.Application.DTOs;

namespace RuleEngine.Application.Interfaces
{
    public interface IRulePersistenceService
    {
        Task SaveAsync(string id, RuleTreeDto dto);
        Task<RuleTreeDto?> LoadAsync(string id);
    }
}
