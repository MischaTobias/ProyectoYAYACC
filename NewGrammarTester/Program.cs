using System;
using System.IO;

namespace NewGrammar
{
	public enum TokenType
	{
		EOF = (char)0,
		Terminal = (char)1,
		NonTerminal = (char)2
	}

	public struct Token
	{
		public TokenType Tag;
		public char Value;
	}

	public class Scanner
	{
		private readonly string _regexp = "";
		private int _index = 0;
		public Scanner(string regexp)
		{
			_regexp = regexp + (char)TokenType.EOF;
			_index = 0;
		}
		public Token GetToken()
		{
			Token result = new() { Value = (char)0 };
			bool tokenFound = false;
			while (!tokenFound)
			{
				char peek = _regexp[_index];
				switch (peek)
				{
					case '\n':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case '\t':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case 'o':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case 'a':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case 'n':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case '(':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case ')':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case '\\':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case '\'':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case 'B':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case (char)0:
						tokenFound = true;
						result.Tag = TokenType.EOF;
						result.Value = peek;
						break;
					default:
						throw new Exception("Lex Error");
				}
				_index++;
			}
			return result;
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			string regexp = Console.ReadLine();
			Scanner scanner = new(regexp);

			Token nextToken;
			do
			{
				nextToken = scanner.GetToken();
				Console.WriteLine($"Token: {nextToken.Tag}, Value: {nextToken.Value}");
			} while (nextToken.Tag != TokenType.EOF);

			Console.ReadLine();
		}
	}
}
