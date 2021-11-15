using CustomCompiler.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace CustomCompiler.Grammar_Structure
{
    public class GrammarObj
    {
        public string InitialVariable { get; set; }
        public List<Token> Terminals { get; set; }
        public List<Token> Variables { get; set; }
        public List<Production> Productions { get; set; }
        public Dictionary<Token, List<Token>> FirstList = new();

        public string GetString()
        {
            string resultString = "";
            resultString += "Initial State: \n";
            resultString += $"\t{InitialVariable}\n";
            resultString += "Terminals:\n";
            foreach (var terminal in Terminals)
            {
                resultString += $"\t{ terminal.Value } \n";
            }
            resultString += "Variables:\n";
            foreach (var variable in Variables)
            {
                resultString += $"\t{ variable.Value } \n";
            }
            resultString += "Productions:\n";
            foreach (var production in Productions)
            {
                resultString += $"\t{ production.Variable.Value } ->";
                production.Result.ForEach(t =>
                {
                    resultString += $" { t.Value }";
                });
                resultString += "\n";
            }
            return resultString;
        }

        public GrammarObj GenerateExtendedGrammar()
        {
            var exGrammarProduction = new Production 
            {
                Variable = new Token
                {
                    Value = Productions[0].Variable.Value + "'",
                    Tag = Productions[0].Variable.Tag
                },

                Result = new List<Token>()
                {
                    Productions[0].Variable
                }
            };

            Productions.Insert(0, exGrammarProduction);
            return this;
        }

        public void GenerateFirsts()
        {
            foreach (var variable in Variables)
            {
                GetFirst(variable);
            }
        }

        private void GetFirst(Token token)
        {
            if (FirstList.ContainsKey(token)) return;

            var tokenList = new List<Token>();
            var rulesByNonTerminal = Productions.Where(p => p.Variable.Value == token.Value);
            foreach (var rule in rulesByNonTerminal)
            {
                var firstElement = rule.Result[0];

                if (firstElement.Tag == TokenType.Terminal)
                {
                    tokenList.Add(firstElement);
                }
                else if (firstElement.Tag == TokenType.NonTerminal)
                {
                    if (!FirstList.ContainsKey(firstElement))
                    {
                        GetFirst(firstElement);
                    }
                    tokenList.AddRange(FirstList[firstElement]);
                }
            }
            FirstList.Add(token, tokenList.Distinct().ToList());
        }
    }
}
