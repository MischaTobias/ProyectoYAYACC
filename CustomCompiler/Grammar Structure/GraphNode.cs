using CustomCompiler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace CustomCompiler.Grammar_Structure
{
    public class GraphNode
    {
        public List<Production> Rules = new();
        public Dictionary<Production, int> SharingNodes = new();
        public bool Finished = false;

        public static bool RuleExists(List<Production> _tempRuleList, GraphNode newNode, Token nextToken)
        {
            bool existsInTempList = false;
            bool existsInNewNode = false;

            foreach (var rule in _tempRuleList.Where(r => r.Variable.Value == nextToken.Value))
            {
                if (rule.Result.FindIndex(t => t.Tag == TokenType.Dot) < 1)
                {
                    existsInTempList = true;
                }
            }

            foreach (var rule in newNode.Rules.Where(r => r.Variable.Value == nextToken.Value))
            {
                if (rule.Result.FindIndex(t => t.Tag == TokenType.Dot) == 0)
                {
                    existsInNewNode = true;
                }
            }

            return existsInTempList || existsInNewNode;
        }

        public void GenerateLookAheads()
        {
            var firstRuleLookAhead = Rules[0].LookAhead;
            var nextTokenIndex = Rules[0].Result.FindIndex(t => t.Tag == TokenType.Dot) + 1;
            if (nextTokenIndex < Rules[0].Result.Count)
            {
                var nextToken = Rules[0].Result[nextTokenIndex];
                if (nextToken.Tag == TokenType.NonTerminal)
                {

                }
            }
        }

        private void GenerateNextLookAhead()
        {

        }
    }
}
