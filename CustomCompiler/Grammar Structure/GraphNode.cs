using CustomCompiler.Tokens;
using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    public class GraphNode
    {
        public List<Production> Rules = new();
        public List<int> SharingNodes = new();
        public bool Finished = false;
    }
}
