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

        public RuleController(IRulePersistenceService ruleService, IRuleCompilerService<Dictionary<string, object>> compiler)
        {
            _ruleService = ruleService;
        }

        #region Rule Persistence

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ruleService.SaveAsync(id, dto);
            return Ok();
        }

        #endregion

        #region Rule Evaluation

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateRule([FromBody] RuleEvaluationRequest request)
        {
            var dto = await _ruleService.LoadAsync(request.RuleId);
            if (dto == null)
                return NotFound($"Rule with ID '{request.RuleId}' not found.");

            var context = new DynamicEvaluationContext(request.Input);
            var node = RuleTreeParser.Parse<DynamicEvaluationContext>(dto);

            var compiler = new RuleCompilerService<DynamicEvaluationContext>();
            compiler.Compile(node);

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
