using Microsoft.AspNetCore.Mvc;
using RuleEngine.Application.DTOs;
using RuleEngine.Application.Interfaces;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Contexts;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Api.Controllers
{
    [ApiController]
    [Route("rules")]
    public class RuleController : ControllerBase
    {
        private readonly IRulePersistenceService _ruleService;
        private readonly IRuleCompilerService<DynamicEvaluationContext> _compiler;
        private static readonly Dictionary<string, RuleTreeDto> _inMemoryCache = new();

        public RuleController(
            IRulePersistenceService ruleService,
            IRuleCompilerService<DynamicEvaluationContext> compiler)
        {
            _ruleService = ruleService;
            _compiler = compiler;
        }

        #region Rule Persistence

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRule(string id)
        {
            if (_inMemoryCache.TryGetValue(id, out var cachedRule))
                return Ok(cachedRule);

            var rule = await _ruleService.LoadAsync(id);
            if (rule == null)
                return NotFound();

            _inMemoryCache[id] = rule;
            return Ok(rule);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SaveRule(string id, [FromBody] RuleTreeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ruleService.SaveAsync(id, dto);
            _inMemoryCache[id] = dto;

            return Ok();
        }

        #endregion

        #region Rule Evaluation

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateRule([FromBody] RuleEvaluationRequest request)
        {
            if (!_inMemoryCache.TryGetValue(request.RuleId, out var dto))
            {
                dto = await _ruleService.LoadAsync(request.RuleId);
                if (dto == null)
                    return NotFound($"Rule with ID '{request.RuleId}' not found.");

                _inMemoryCache[request.RuleId] = dto;
            }

            var context = new DynamicEvaluationContext(request.Input);
            var node = RuleTreeParser.Parse<DynamicEvaluationContext>(dto);

            try
            {
                _compiler.Compile(node);
            }
            catch (Exception ex)
            {
                return BadRequest($"Compilation failed: {ex.Message}");
            }

            var success = node.Evaluate(context);
            var failed = success ? new List<string>() : new List<string> { dto.Name };

            return Ok(new RuleEvaluationResponse
            {
                Success = success,
                FailedRules = failed
            });
        }

        #endregion
    }
}
