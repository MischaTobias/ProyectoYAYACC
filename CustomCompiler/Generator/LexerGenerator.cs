using System.IO;
using CustomCompiler.Grammar_Structure;

namespace CustomCompiler.Generator
{
    public class LexerGenerator
    {
        private string _newParserAddress;
        private string _newFileName = "lexerResult.cs";
        private string _newFileFullAddress;
        private string _prevIndentation;
        private string WS(string line) => $"{_prevIndentation}{line}";

        public LexerGenerator(string address)
        {
            _newParserAddress = Path.GetDirectoryName(address);
            _newFileFullAddress = $"{_newParserAddress}\\{_newFileName}";
            _prevIndentation = string.Empty;
        }

        public void WriteLexer(GrammarObj grammar)
        {
            if (File.Exists(_newFileFullAddress))
            {
                File.Delete(_newFileFullAddress);
            }

            using StreamWriter sw = new(_newFileFullAddress);
            sw.WriteLine(WS("using System;"));
            sw.WriteLine(WS("using System.IO;"));
            sw.WriteLine();
            //Comienzo namespace
            sw.WriteLine(WS("namespace NewGrammar"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            //Inicio TokenType
            sw.WriteLine(WS("public enum TokenType"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("EOF = (char)0,"));
            sw.WriteLine(WS("Terminal = (char)1,"));
            sw.WriteLine(WS("NonTerminal = (char)2"));

            //Fin TokenType
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));
            sw.WriteLine();

            //Inicio Token
            sw.WriteLine(WS("public struct Token"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("public TokenType Tag;"));
            sw.WriteLine(WS("public char Value;"));

            //Fin Token
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));
            sw.WriteLine();

            //Inicio Lexer
            sw.WriteLine(WS("public class Scanner"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("private readonly string _regexp = \"\";"));
            sw.WriteLine(WS("private int _index = 0;"));

            //Inicio Constructor Scanner
            sw.WriteLine(WS("public Scanner(string regexp)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("_regexp = regexp + (char)TokenType.EOF;"));
            sw.WriteLine(WS("_index = 0;"));

            //Fin Constructor Scanner
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Inicio Procedimiento GetToken
            sw.WriteLine(WS("public Token GetToken()"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("Token result = new() { Value = (char)0 };"));
            sw.WriteLine(WS("bool tokenFound = false;"));

            //Inicio While(Token)
            sw.WriteLine(WS("while (!tokenFound)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("char peek = _regexp[_index];"));

            //Inicio switch(peek)
            sw.WriteLine(WS("switch (peek)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            foreach (var terminal in grammar.Terminals)
            {
                sw.WriteLine(WS($"case {terminal}:"));
                _prevIndentation += '\t';
                sw.WriteLine(WS("tokenFound = true;"));
                sw.WriteLine(WS("result.Tag = TokenType.Terminal;"));
                sw.WriteLine(WS("result.Value = peek;"));
                sw.WriteLine(WS("break;"));

                _prevIndentation = _prevIndentation[1..];
            }

            sw.WriteLine(WS($"case (char)0:"));
            _prevIndentation += '\t';
            sw.WriteLine(WS("tokenFound = true;"));
            sw.WriteLine(WS("result.Tag = TokenType.EOF;"));
            sw.WriteLine(WS("result.Value = peek;"));
            sw.WriteLine(WS("break;"));

            _prevIndentation = _prevIndentation[1..];

            sw.WriteLine(WS("default:"));
            _prevIndentation += '\t';
            sw.WriteLine(WS("throw new Exception(\"Lex Error\");"));

            //Fin switch(peek)
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            sw.WriteLine(WS("_index++;"));

            //Fin While(tokenFound)
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            sw.WriteLine(WS("return result;"));

            //Fin Procedimiento GetToken
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Fin Lexer
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));
            sw.WriteLine();

            //Inicio Programa
            sw.WriteLine(WS("class Program"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            //Inicio Main
            sw.WriteLine(WS("static void Main(string[] args)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("string regexp = Console.ReadLine();"));
            sw.WriteLine(WS("Scanner scanner = new(regexp);"));
            sw.WriteLine();

            sw.WriteLine(WS("Token nextToken;"));

            //Inicio Do-While
            sw.WriteLine(WS("do"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("nextToken = scanner.GetToken();"));
            sw.WriteLine(WS("Console.WriteLine($\"Token: {nextToken.Tag}, Value: {nextToken.Value}\");"));

            //Fin Do-While
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("} while (nextToken.Tag != TokenType.EOF);"));

            sw.WriteLine();
            sw.WriteLine(WS("Console.ReadLine();"));

            //Fin Main
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Fin Programa
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Fin namespace
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));
        }
    }
}
