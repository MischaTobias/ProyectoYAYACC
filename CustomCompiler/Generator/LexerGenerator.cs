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

            //Inicio Constructor Scanner
            sw.WriteLine(WS("public Scanner(string regexp)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';
            sw.WriteLine(WS("_regexp = regexp + (char)TokenType.EOF;"));
            sw.WriteLine(WS("_index = 0;"));
            sw.WriteLine(WS("_state = 0;"));
            //Fin Constructor Scanner
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Inicio Procedimiento Token
            sw.WriteLine(WS("public Token GetToken()"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';
            sw.WriteLine(WS("Token result = new() { Value = new string((char)0, 1) };"));
            sw.WriteLine(WS("bool tokenFound = false;"));

            //Inicio While(Token)
            sw.WriteLine(WS("while (!tokenFound)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            //Inicio While(string.isnull)
            sw.WriteLine(WS("while (string.IsNullOrWhiteSpace(new string(_regexp[_index], 1)))"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            sw.WriteLine(WS("if (result.Tag == TokenType.NonTerminal) return result;"));
            sw.WriteLine(WS("_index++;"));

            //Fin While(string.isnull)
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            sw.WriteLine(WS("if (_index == _regexp.Length - 1) return result;"));
            sw.WriteLine(WS("char peek = _regexp[_index];"));
            sw.WriteLine(WS("if (peek == (char)TokenType.EOF) return result;"));

            //Inicio if(result.Tag)
            sw.WriteLine(WS("if (result.Tag == TokenType.NonTerminal)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            //Inicio if(new [] )
            sw.WriteLine(WS("if (new[] { TokenType.SemiColon, TokenType.Apostrophe, TokenType.Colon, TokenType.Pipe }"));
            _prevIndentation += '\t';
            sw.WriteLine(WS(".Any(token => (char)token == peek))"));
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';
            sw.WriteLine(WS("return result;"));

            //Fin if(new[])
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Fin if(result.Tag)
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Inicio switch(peek)
            sw.WriteLine(WS("switch (peek)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            //Fin switch(peek)
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            sw.WriteLine(WS("_index++;"));

            //Fin While(Token)
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            sw.WriteLine(WS("return result;"));

            //Fin Procedimiento Token
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Fin Lexer
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));

            //Inicio Programa
            sw.WriteLine(WS("class Program"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

            //Inicio Main
            sw.WriteLine(WS("static void Main(string[] args)"));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';

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
