//using Microsoft.AspNetCore.Mvc;
//using RuleEngine.Application.Services;
//using RuleEngine.Domain.Entities;

//namespace RuleEngine.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class RulesController : ControllerBase
//    {
//        private readonly RuleService _ruleService;

//        public RulesController(RuleService ruleService)
//        {
//            _ruleService = ruleService;
//        }

//        //[HttpPost("evaluate")]
//        //public IActionResult Evaluate([FromBody] RuleEvaluationRequest request)
//        //{
//        //   // var result = _ruleService.Execute(request.Rule, request.Context);
//        //   // return Ok(result);
//        //}
//    }
//    public class RuleEvaluationRequest
//    {
//       // public Rule Rule { get; set; }
//        public Dictionary<string, object> Context { get; set; }
//    }
//}