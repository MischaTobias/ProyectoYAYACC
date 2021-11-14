using System.Collections.Generic;

namespace CustomCompiler.Grammar_Structure
{
    internal struct GraphNode
    {
        List<string> Rules;
        List<int> SharingNodes;
    }
}
