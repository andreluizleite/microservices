using Microsoft.AspNetCore.Mvc;
using RuleEngine.Application.DTOs;
using RuleEngine.Application.Interfaces;

namespace RuleEngine.Api.Controllers
{
    [ApiController]
    [Route("rules")]
    public class RuleController : ControllerBase
    {
        private readonly IRulePersistenceService _ruleService;

        public RuleController(IRulePersistenceService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRule(string id)
        {
            var rule = await _ruleService.LoadAsync(id);
            if (rule == null)
                return NotFound();
            return Ok(rule);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SaveRule(string id, [FromBody] RuleTreeDto dto)
        {
            await _ruleService.SaveAsync(id, dto);
            return Ok();
        }
    }

}