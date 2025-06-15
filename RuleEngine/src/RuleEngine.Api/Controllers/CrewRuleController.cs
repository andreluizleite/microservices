using Microsoft.AspNetCore.Mvc;
using RuleEngine.Application.DTOs;
using RuleEngine.Application.Evaluators;
using RuleEngine.Application.Interfaces;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;

namespace RuleEngine.Api.Controllers
{
    [ApiController]
    [Route("crew-rules")]
    public class CrewRuleController : ControllerBase
    {
        private readonly IRulePersistenceService _ruleService;

        public CrewRuleController(IRulePersistenceService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> Evaluate([FromBody] RuleEvaluationRequest request)
        {
            var dto = await _ruleService.LoadAsync(request.RuleId);
            if (dto == null)
                return NotFound($"Rule with ID '{request.RuleId}' not found.");

            var rule = new CalculationRule<CrewManagementEvaluationContext, decimal>(
                name: dto.Name,
                expressionString: dto.Expression!
            );

            try
            {
                rule.Compile();

                var context = new CrewManagementEvaluationContext
                {
                    Assignments = request.Input["Assignments"] as List<Assignment> ?? new List<Assignment>()
                };

                var evaluator = new CalculationEvaluator<CrewManagementEvaluationContext, decimal>();
                var result = evaluator.Evaluate(rule, context);

                return Ok(new RuleEvaluationResponse
                {
                    Success = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RuleEvaluationResponse
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }
    }
}
