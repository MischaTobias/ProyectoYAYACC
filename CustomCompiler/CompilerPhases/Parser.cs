using System;
using System.IO;
using CustomCompiler.Tokens;
using System.Collections.Generic;

namespace CustomCompiler.CompilerPhases
{
    public class Parser
    {
        Scanner _scanner;
        Dictionary<int, Dictionary<string, string>> _lalrTable;
        List<List<string>> _rules;
        Stack<int> _states;
        Stack<string> _symbols;
        Queue<Token> _input;

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
            _rules.Add(new List<string> { "G", "S", "G" });
            _rules.Add(new List<string> { "G", "S" });
            _rules.Add(new List<string> { "S", "NT", ":", "S'", ";" });
            _rules.Add(new List<string> { "S'", "C", "|", "S'" });
            _rules.Add(new List<string> { "S'", "C" });
            _rules.Add(new List<string> { "C", "SC", "C" });
            _rules.Add(new List<string> { "C", "SC" });
            _rules.Add(new List<string> { "SC", "NT" });
            _rules.Add(new List<string> { "SC", "T" });
        }

        private void InitializeTable()
        {
            _lalrTable = new();
            var tempRow = new Dictionary<string, string>
            {
                { "NT", "S3" },
                { "G", "1" },
                { "S", "2" }
            };
            _lalrTable.Add(0, tempRow);
            tempRow = new Dictionary<string, string> { { "$", "ACCEPT" } };
            _lalrTable.Add(1, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "S3" },
                { "$", "R2" },
                { "G", "7" },
                { "S", "2" }
            };
            _lalrTable.Add(2, tempRow);
            tempRow = new Dictionary<string, string> { { ":", "S8" } };
            _lalrTable.Add(3, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { ";", "R5" },
                { "|", "S9" }
            };
            _lalrTable.Add(4, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "S11" },
                { "T", "S6" },
                { ";", "R7" },
                { "|", "R7" },
                { "C", "10" },
                { "SC", "5" }
            };
            _lalrTable.Add(5, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "R9" },
                { "T", "R9" },
                { ";", "R9" },
                { "|", "R9" }
            };
            _lalrTable.Add(6, tempRow);
            tempRow = new Dictionary<string, string> { { "$", "R1" } };
            _lalrTable.Add(7, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "S11" },
                { "T", "S6" },
                { "S'", "12" },
                { "C", "4" },
                { "SC", "5" }
            };
            _lalrTable.Add(8, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "S11" },
                { "T", "S6" },
                { "S'", "13" },
                { "C", "4" },
                { "SC", "5" }
            };
            _lalrTable.Add(9, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { ";", "R6" },
                { "|", "R6" }
            };
            _lalrTable.Add(10, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "R8" },
                { "T", "R8" },
                { ";", "R8" },
                { "|", "R8" }
            };
            _lalrTable.Add(11, tempRow);
            tempRow = new Dictionary<string, string> { { ";", "S14" } };
            _lalrTable.Add(12, tempRow);
            tempRow = new Dictionary<string, string> { { ";", "R4" } };
            _lalrTable.Add(13, tempRow);
            tempRow = new Dictionary<string, string>
            {
                { "NT", "R3" },
                { "$", "R3" }
            };
            _lalrTable.Add(14, tempRow);
        }

        public void Parse(string address)
        {
            using StreamReader sr = new(address);
            string ln = "";
            string regexp = "";
            while ((ln = sr.ReadLine()) != null) regexp += ln;
            _scanner = new Scanner(regexp + (char)TokenType.EOF);
            Token token;
            while ((token = _scanner.GetToken()).Tag != TokenType.EOF) _input.Enqueue(token);
            _input.Enqueue(new Token { Tag = TokenType.Dollar });
            _states.Push(0);
            _symbols.Push("#");


            while (_input.Count != 0 || _states.Count != 0)
            {
                try
                {
                    var inputPeek = GetKey(_input.Peek());
                    var action_goto = _lalrTable[_states.Peek()][inputPeek];
                    if (action_goto.Contains("S"))//Shift
                    {
                        _symbols.Push(inputPeek);
                        _states.Push(Convert.ToInt32(action_goto.Remove(0, 1)));
                        _input.Dequeue();
                    }
                    else if (action_goto.Contains("R"))//Reduce
                    {
                        var rule = _rules[Convert.ToInt32(action_goto.Remove(0, 1)) - 1];
                        var productionQty = rule.Count - 1;
                        for (int i = 0; i < productionQty; i++)
                        {
                            _symbols.Pop();
                            _states.Pop();
                        }
                        _symbols.Push(rule[0]);
                        _states.Push(Convert.ToInt32(_lalrTable[_states.Peek()][rule[0]]));
                    }
                    else if (action_goto == "ACCEPT")
                    {
                        return;
                    }
                }
                catch
                {
                    throw new Exception("Acción no encontrada en tabla. Parse Error.");
                }
            }
        }

        private static string GetKey(Token token)
        {
            switch (token.Tag)
            {
                case TokenType.Colon:
                case TokenType.SemiColon:
                case TokenType.Pipe:
                case TokenType.Dollar:
                    return new string((char)token.Tag, 1);
                case TokenType.Terminal:
                    return "T";
                case TokenType.NonTerminal:
                    return "NT";
            }
            return "";
        }
    }
}
