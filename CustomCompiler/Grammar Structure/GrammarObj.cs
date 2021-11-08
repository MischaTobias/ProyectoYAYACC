using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    struct GrammarObj
    {
        public string InitialState { get; set; }
        public List<string> Terminals { get; set; }
        public List<string> Variables { get; set; }
        public List<Production> Productions { get; set; }
    }
}
