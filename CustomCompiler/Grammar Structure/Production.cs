using CustomCompiler.Tokens;
using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    public class Production
    {
        public Token Variable { get; set; }
        public List<Token> Result = new();
        public List<Token> LookAhead = new();
        public int NextState = -1;


        public static bool CompareProductionResult(Production prod1, Production prod2)
        {
            if (prod1.Result.Count != prod2.Result.Count || prod1.Variable.Value != prod2.Variable.Value) return false;

            for (int i = 0; i < prod1.Result.Count; i++)
            {
                if (prod1.Result[i].Value != prod2.Result[i].Value) return false;
            }

            return true;
        }
    }
}
