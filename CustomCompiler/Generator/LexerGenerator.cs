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
            sw.WriteLine(WS("public string Value;"));

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
            sw.WriteLine(WS("private int _state = 0;"));

            //Fin Lexer
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Fin namespace
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));
        }
    }
}
