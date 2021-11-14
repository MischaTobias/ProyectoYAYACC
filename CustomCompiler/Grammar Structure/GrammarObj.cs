using CustomCompiler.Tokens;
using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    public class GrammarObj
    {
        public string InitialState { get; set; }
        public List<string> Terminals { get; set; }
        public List<string> Variables { get; set; }
        public List<Production> Productions { get; set; }

        public string GetString()
        {
            string resultString = "";
            resultString += "Initial State: \n";
            resultString += $"\t{InitialState}\n";
            resultString += "Terminals:\n";
            foreach (var terminal in Terminals)
            {
                resultString += $"\t{terminal} \n";
            }
            resultString += "Variables:\n";
            foreach (var variable in Variables)
            {
                resultString += $"\t{variable} \n";
            }
            resultString += "Productions:\n";
            foreach (var production in Productions)
            {
                resultString += $"\t{production.Variable.Value} ->";
                production.Result.ForEach(t =>
                {
                    resultString += $" {t.Value}";
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
    }
}
