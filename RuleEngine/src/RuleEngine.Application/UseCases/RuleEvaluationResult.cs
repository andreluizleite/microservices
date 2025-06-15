using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine.Application.UseCases
{
    public class RuleEvaluationResult
    {
        public bool Success { get; set; }
        public List<string> FailedRules { get; set; } = new();
    }

}
