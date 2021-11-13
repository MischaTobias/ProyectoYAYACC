using System;
using CustomCompiler.Tokens;
using System.Text.RegularExpressions;
using System.Linq;

namespace CustomCompiler.CompilerPhases
{
    public class Scanner
    {
        private readonly string _regexp = "";
        private int _index = 0;
        private int _state = 0;

        public Scanner(string regexp)
        {
            _regexp = regexp + (char)TokenType.EOF;
            _index = 0;
            _state = 0;
        }

        public Token GetToken()
        {
            _state = 0;
            Token result = new() { Value = new string((char)0, 1) };
            bool tokenFound = false;
            while (!tokenFound)
            {
                while (string.IsNullOrWhiteSpace(new string(_regexp[_index], 1)))
                {
                    if (result.Tag == TokenType.NonTerminal) return result;
                    _index++;
                }
                if (_index == _regexp.Length - 1) return result;
                char peek = _regexp[_index];

                if (peek == (char)TokenType.EOF) return result;

                if (result.Tag == TokenType.NonTerminal)
                {
                    if (new[] { TokenType.SemiColon, TokenType.Apostrophe, TokenType.Colon, TokenType.Pipe }
                        .Any(token => (char)token == peek))
                    {
                        return result;
                    }
                }

                switch (_state)
                {
                    case 0:
                        switch (peek)
                        {
                            case (char)TokenType.Colon:
                            case (char)TokenType.SemiColon:
                            case (char)TokenType.Pipe:
                                tokenFound = true;
                                result.Tag = (TokenType)peek;
                                break;
                            case (char)TokenType.Apostrophe:
                                result.Tag = TokenType.Terminal;
                                result.Value = new string(peek, 1);
                                _state = 1;
                                break;
                            case var someVal when new Regex(@"[_A-Za-z]").IsMatch(new string(someVal, 1)):
                                result.Tag = TokenType.NonTerminal;
                                result.Value = new string(peek, 1);
                                _state = 4;
                                break;
                            default:
                                throw new Exception("Lex Error");
                        }
                        break;
                    case 1:
                        switch (peek)
                        {
                            case (char)TokenType.BackSlash:
                                result.Value += peek;
                                _state = 3;
                                break;
                            case var someVal when new Regex(@"[A-Za-z0-9 !”#%&\(\)\*\+,\-\./:;<=>\?\[\]^_\{\|\}~]").IsMatch(new string(someVal, 1)):
                                result.Value += peek;
                                _state = 2;
                                break;
                            default:
                                throw new Exception("Lex Error");
                        }
                        break;
                    case 2:
                        switch (peek)
                        {
                            case (char)TokenType.Apostrophe:
                                tokenFound = true;
                                result.Value += peek;
                                break;
                            default:
                                throw new Exception("Lex Error");
                        }
                        break;
                    case 3:
                        switch (peek)
                        {
                            case (char)TokenType.BackSlash:
                            case (char)TokenType.Apostrophe:
                            case 'n':
                            case 't':
                                result.Value += peek;
                                _state = 2;
                                break;
                            default:
                                throw new Exception("Lex Error");
                        }
                        break;
                    case 4:
                        result.Value += peek switch
                        {
                            var someVal when new Regex(@"[_A-Za-z0-9]").IsMatch(new string(someVal, 1)) => peek,
                            _ => throw new Exception("Lex Error"),
                        };
                        break;
                }
                _index++;
            }
            return result;
        }
    }
}
