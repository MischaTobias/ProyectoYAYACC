using CustomCompiler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace CustomCompiler.Grammar_Structure
{
    public class GraphNode
    {
        public int NodeNumber = -1;
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

        public void AssignNumberToRule(int nodeNumber, Production prod)
        {
            var sameRuleNumber = GetSameProduction(prod);
            if (sameRuleNumber == -1) throw new System.Exception("Tried to use a production that doesn't exist");
            Rules[sameRuleNumber].NextState = nodeNumber;
        }

        public bool LookAheadIsTheSame(List<Production> productions)
        {
            bool sameLookAheads = true;
            foreach (var prod in productions)
            {
                var sameRuleNumber = GetSameProduction(prod);
                if (sameRuleNumber == -1) throw new System.Exception("Tried to use a production that doesn't exist");
                foreach (var lookAhead in prod.LookAhead)
                {
                    sameLookAheads = sameLookAheads && Rules[sameRuleNumber].LookAhead.Contains(lookAhead);
                    if (!sameLookAheads) return false;
                }
            }
            return true;
        }

        public void ChangeLookAheadForRules(List<Production> productions)
        {
            foreach (var prod in productions)
            {
                var sameRuleNumber = GetSameProduction(prod);
                if (sameRuleNumber == -1) throw new System.Exception("Tried to use a production that doesn't exist");
                foreach (var token in prod.LookAhead)
                {
                    if (!Rules[sameRuleNumber].LookAhead.Contains(token))
                    {
                        Rules[sameRuleNumber].LookAhead.Add(token);
                    }
                }
            }
        }

        private int GetSameProduction(Production prod)
        {
            bool isTheSameRule;

            for (int i = 0; i < Rules.Count; i++)
            {
                isTheSameRule = true;
                if (Rules[i].Result.Count == prod.Result.Count)
                {
                    for (int j = 0; j < Rules[i].Result.Count; j++)
                    {
                        isTheSameRule = isTheSameRule && Rules[i].Result[j].Value == prod.Result[j].Value;
                        if (!isTheSameRule) break;
                    }

                    if (isTheSameRule) return i;
                }
            }

            return -1;
        }
    }
}
