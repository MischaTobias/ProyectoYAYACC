using CustomCompiler.Tokens;
using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    public class Production
    {
        public Token Variable { get; set; }
        public List<Token> Result = new();
    }
}
