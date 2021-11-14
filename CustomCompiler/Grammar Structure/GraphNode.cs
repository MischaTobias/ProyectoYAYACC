using CustomCompiler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace CustomCompiler.Grammar_Structure
{
    public class GraphNode
    {
        public List<Production> Rules = new();
        public List<int> SharingNodes = new();
        public bool Finished = false;

        public static bool RuleExists(List<Production> _tempRuleList, GraphNode newNode, Token nextToken)
        {
            var existsInTempList = _tempRuleList.Any(p => p.Variable.Value == nextToken.Value && p.Result.Any(t => t.Tag == TokenType.Dot));
            var existsInNewNode = newNode.Rules.Any(p => p.Variable.Value == nextToken.Value && p.Result.Any(t => t.Tag == TokenType.Dot));
            return existsInTempList || existsInNewNode;
        }
    }
}
