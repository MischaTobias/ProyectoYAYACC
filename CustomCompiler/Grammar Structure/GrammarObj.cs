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
                resultString += $"\t{production.Variable} -> {production.Result} \n";
            }
            return resultString;
        }
    }
}
