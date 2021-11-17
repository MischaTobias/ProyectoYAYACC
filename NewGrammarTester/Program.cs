using System;
using System.IO;
using System.Collections.Generic;

namespace NewGrammar
{
	public enum TokenType
	{
		EOF = (char)0,
		Terminal = (char)1,
		NonTerminal = (char)2,
		Dollar = '$'
	}

	public struct Token
	{
		public TokenType Tag;
		public char Value;
	}

	struct Symbol
	{
		public string SymbolType;
		public List<Token> Tokens;
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
					case 'n':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case ':':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case ';':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case '|':
						tokenFound = true;
						result.Tag = TokenType.Terminal;
						result.Value = peek;
						break;
					case 't':
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

	public class Parser
	{
		private Scanner _scanner;
		private Dictionary<int, Dictionary<string, string>> _lalrTable;
		private List<List<string>> _rules;
		private Stack<int> _states;
		private Stack<Symbol> _symbols;
		private Queue<Token> _input;
		public Parser()
		{
			InitializeTable();
			InitializeRules();
			_states = new();
			_symbols = new();
			_input = new();
		}

		private void InitializeRules()
		{
			_rules = new();
			_rules.Add(new List<string> { "G'", "G" });
			_rules.Add(new List<string> { "G", "S", "G" });
			_rules.Add(new List<string> { "G", "S" });
			_rules.Add(new List<string> { "S", "'n'", "':'", "SP", "';'" });
			_rules.Add(new List<string> { "SP", "C", "'|'", "SP" });
			_rules.Add(new List<string> { "SP", "C" });
			_rules.Add(new List<string> { "C", "SC", "C" });
			_rules.Add(new List<string> { "C", "SC" });
			_rules.Add(new List<string> { "SC", "'n'" });
			_rules.Add(new List<string> { "SC", "'t'" });
		}

		private void InitializeTable()
		{
			_lalrTable = new();
			var tempRow = new Dictionary<string, string>();
			tempRow = new Dictionary<string, string>
			{
				{ "'n'", "S4" },
				{ "G", "1" },
				{ "S", "2" },
			};
			_lalrTable.Add(0, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'$'", "ACCEPT" }
			};
			_lalrTable.Add(1, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'$'", "R2" },
				{ "'n'", "S4" },
				{ "G", "3" },
				{ "S", "2" },
			};
			_lalrTable.Add(2, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'$'", "R1" },
			};
			_lalrTable.Add(3, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "':'", "S5" },
			};
			_lalrTable.Add(4, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'n'", "S13" },
				{ "'t'", "S14" },
				{ "SP", "6" },
				{ "C", "8" },
				{ "SC", "11" },
			};
			_lalrTable.Add(5, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "';'", "S7" },
			};
			_lalrTable.Add(6, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'n'", "R3" },
				{ "'$'", "R3" },
			};
			_lalrTable.Add(7, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "';'", "R5" },
				{ "'|'", "S9" },
			};
			_lalrTable.Add(8, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'n'", "S13" },
				{ "'t'", "S14" },
				{ "SP", "10" },
				{ "C", "8" },
				{ "SC", "11" },
			};
			_lalrTable.Add(9, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "';'", "R4" },
			};
			_lalrTable.Add(10, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'|'", "R7" },
				{ "';'", "R7" },
				{ "'n'", "S13" },
				{ "'t'", "S14" },
				{ "C", "12" },
				{ "SC", "11" },
			};
			_lalrTable.Add(11, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'|'", "R6" },
				{ "';'", "R6" },
			};
			_lalrTable.Add(12, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'n'", "R8" },
				{ "'t'", "R8" },
				{ "'|'", "R8" },
				{ "';'", "R8" },
			};
			_lalrTable.Add(13, tempRow);

			tempRow = new Dictionary<string, string>
			{
				{ "'n'", "R9" },
				{ "'t'", "R9" },
				{ "'|'", "R9" },
				{ "';'", "R9" },
			};
			_lalrTable.Add(14, tempRow);

		}

		public void Parse(string regexp)
		{
			_scanner = new Scanner(regexp + (char)TokenType.EOF);
			Token token;
			while ((token = _scanner.GetToken()).Tag != TokenType.EOF) _input.Enqueue(token);
			_input.Enqueue(new Token { Tag = TokenType.Dollar, Value = '$' });
			_states.Push(0);
			_symbols.Push(new Symbol { SymbolType = "#" });
			while (_input.Count != 0 || _states.Count != 0)
			{
				try
				{
					var inputPeek = _input.Peek();
					var action_goto = _lalrTable[_states.Peek()][$"'{ inputPeek.Value }'"];
					if (action_goto.Contains("S"))
					{
						_states.Push(Convert.ToInt32(action_goto.Remove(0, 1)));
						_symbols.Push(new Symbol { SymbolType = $"'{ inputPeek.Value }'", Tokens = new List<Token> { _input.Dequeue() } });
					}

					else if (action_goto.Contains("R"))
					{
						var ruleNumber = Convert.ToInt32(action_goto.Remove(0, 1));
						var rule = _rules[ruleNumber];
						var productionQty = rule.Count - 1;
						List<Token> temp = new();
						for (int i = 0; i < productionQty; i++)
						{
							var prevTokens = _symbols.Pop().Tokens;
							foreach (var tokenP in prevTokens) temp.Add(tokenP);
							_states.Pop();
						}

						_symbols.Push(new Symbol { SymbolType = rule[0], Tokens = temp });
						_states.Push(Convert.ToInt32(_lalrTable[_states.Peek()][rule[0]]));
					}

					else if (action_goto == "ACCEPT")
					{
						return; //Cadena ok
					}

				}

				catch
				{
					throw new Exception("Acción no encontrada en tabla.Parse Error.");
				}

			}

		}

	}

	class Program
	{
		static void Main(string[] args)
		{
			string regexp = Console.ReadLine();
			Parser parser = new Parser();

			try
			{
				parser.Parse(regexp);
				Console.WriteLine("Expresión OK");
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}


			Console.ReadLine();
		}

	}

}

