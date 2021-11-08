using CustomCompiler.Tokens;
using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    struct Symbol
    {
        public string SymbolType;
        public List<Token> Tokens;
    }
}
