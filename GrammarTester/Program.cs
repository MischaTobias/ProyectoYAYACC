using System;
using System.IO;
using CustomCompiler.CompilerPhases;
using CustomCompiler.Tokens;

namespace GrammarTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese la dirección del archivo");
            var address = Path.GetFullPath(Console.ReadLine());
            Parser parser = new();
            Console.WriteLine(parser.Parse(address).GetString());
            Console.ReadLine();
        }
    }
}
