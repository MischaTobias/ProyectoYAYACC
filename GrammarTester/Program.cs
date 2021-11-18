using System;
using System.IO;
using CustomCompiler.CompilerPhases;
using CustomCompiler.Generator;

namespace GrammarTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Ingrese la dirección del archivo");
                var address = Path.GetFullPath(Console.ReadLine());
                Parser parser = new();
                var grammarResult = parser.Parse(address);
                Console.WriteLine(grammarResult.GetString());
                CompilerGenerator generator = new(address, grammarResult);
                generator.GenerateCompiler();
                Console.WriteLine("Se ha generado un archivo .csv en la direccion: " + address);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            Console.ReadLine();
        }
    }
}
