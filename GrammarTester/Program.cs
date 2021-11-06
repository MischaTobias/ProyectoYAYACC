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
            Token nextToken;
            var address = Path.GetFullPath(Console.ReadLine());
            Console.WriteLine(address);
            using (StreamReader sr = new (address))
            {
                string ln = "";
                while ((ln = sr.ReadLine()) != null)
                {
                    Scanner scanner = new(ln);
                    do
                    {
                        nextToken = scanner.GetToken();
                        Console.WriteLine("Token: {0}, Valor {1}", nextToken.Tag, nextToken.Value);
                    } while (nextToken.Tag != TokenType.EOF);
                }
            }
            Console.ReadLine();
        }
    }
}
