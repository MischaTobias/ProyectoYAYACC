using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomCompiler.Grammar_Structure;
using CustomCompiler.Tokens;

namespace CustomCompiler.Generator
{
    public class CompilerGenerator
    {
        private readonly string _newParserAddress;
        private readonly string _newFileName = "lexerResult.cs";
        private readonly string _newFileFullAddress;
        private readonly GrammarObj _currentGrammar;
        private StreamWriter _sw;
        private string _prevIndentation;
        private List<Production> _tempRuleList;
        private List<GraphNode> _LR0Graph;
        private Dictionary<Token, List<Token>> _firsts;
        private Dictionary<Token, List<Token>> _follows;

        private string WS(string line) => $"{_prevIndentation}{line}";

        public CompilerGenerator(string address, GrammarObj grammar)
        {
            _newParserAddress = Path.GetDirectoryName(address);
            _newFileFullAddress = $"{_newParserAddress}\\{_newFileName}";
            _prevIndentation = string.Empty;
            _currentGrammar = grammar.GenerateExtendedGrammar();
        }

        private void WriteNewArea(StreamWriter sw, string newArea)
        {
            sw.WriteLine(WS(newArea));
            sw.WriteLine(WS("{"));
            _prevIndentation += '\t';
        }

        private void EndArea(StreamWriter sw)
        {
            _prevIndentation = _prevIndentation[1..];
            sw.WriteLine(WS("}"));
            sw.WriteLine();
        }

        private void CheckNonTerminals()
        {
            foreach (var nonTerminal in _currentGrammar.Variables)
            {
                if (!_currentGrammar.Productions.Any(p => p.Variable.Value == nonTerminal.Value))
                {
                    throw new Exception($"Variable { nonTerminal } doesn't have a production");
                }
            }
        }

        public void GenerateCompiler()
        {
            if (File.Exists(_newFileFullAddress))
            {
                File.Delete(_newFileFullAddress);
            }

            CheckNonTerminals();

            try
            {
                _sw = new(_newFileFullAddress);
                _sw.WriteLine(WS("using System;"));
                _sw.WriteLine(WS("using System.IO;"));
                _sw.WriteLine(WS("using System.Collections.Generic;"));
                _sw.WriteLine();
                //Comienzo namespace
                WriteNewArea(_sw, "namespace NewGrammar");

                //TokenType
                WriteTokenType();

                //Token
                WriteToken();

                //Symbol
                WriteSymbol();

                //Scanner
                WriteLexer();

                //LR 0
                _currentGrammar.GenerateFirsts();
                GenerateLR0();

                //LALR 1
                GenerateLALR();

                //WriteParser
                WriteParserClass();

                //Console Program
                WriteConsoleProgram();

                //Fin namespace
                EndArea(_sw);

                _sw.Close();
            }
            catch (Exception ex)
            {
                _sw.Close();
                throw new Exception(ex.Message);
            }
        }

        private void WriteSymbol()
        {
            WriteNewArea(_sw, "struct Symbol");
            _sw.WriteLine(WS("public string SymbolType;"));
            _sw.WriteLine(WS("public List<Token> Tokens;"));
            EndArea(_sw);
        }

        private void WriteParserClass()
        {
            //Inicio Parser
            WriteNewArea(_sw, "public class Parser");
            _sw.WriteLine(WS("private Scanner _scanner;"));
            _sw.WriteLine(WS("private Dictionary<int, Dictionary<string, string>> _lalrTable;"));
            _sw.WriteLine(WS("private List<List<string>> _rules;"));
            _sw.WriteLine(WS("private Stack<int> _states;"));
            _sw.WriteLine(WS("private Stack<Symbol> _symbols;"));
            _sw.WriteLine(WS("private Queue<Token> _input;"));

            //Inicio Constructor Parser
            WriteNewArea(_sw, "public Parser()");
            _sw.WriteLine(WS("InitializeTable();"));
            _sw.WriteLine(WS("InitializeRules();"));
            _sw.WriteLine(WS("_states = new();"));
            _sw.WriteLine(WS("_symbols = new();"));
            _sw.WriteLine(WS("_input = new();"));

            //Finaliza constructor del parser
            EndArea(_sw);

            //Inicialización de reglas
            WriteNewArea(_sw, "private void InitializeRules()");
            _sw.WriteLine(WS("_rules = new();"));
            string productionInList;
            _currentGrammar.PrepareRules();
            foreach (var production in _currentGrammar.Productions)
            {
                productionInList = $"\"{ production.Variable.Value }\", ";
                for (int i = 0; i < production.Result.Count; i++)
                {
                    productionInList += $"\"{ production.Result[i].Value }\"";
                    if (i != production.Result.Count - 1) productionInList += ", ";
                }
                _sw.WriteLine(WS($"_rules.Add(new List<string> {{ { productionInList } }});"));
            }
            //Finaliza inicialización de reglas
            EndArea(_sw);

            //Inicialización de tabla
            WriteNewArea(_sw, "private void InitializeTable()");
            _sw.WriteLine(WS("_lalrTable = new();"));
            _sw.WriteLine(WS("var tempRow = new Dictionary<string, string>();"));


            Dictionary<string, int> shifts;
            Dictionary<string, int> gotos;
            Dictionary<string, int> reduces;

            foreach (var state in _LR0Graph)
            {
                shifts = new();
                gotos = new();
                reduces = new();

                WriteNewArea(_sw, "tempRow = new Dictionary<string, string>");

                for (int i = 0; i < state.Rules.Count; i++)
                {
                    var nextTokenIndex = state.Rules[i].Result.FindIndex(t => t.Tag == TokenType.Dot) + 1;
                    if (nextTokenIndex < state.Rules[i].Result.Count)
                    {
                        //SHIFT O GOTO
                        var nextToken = state.Rules[i].Result[nextTokenIndex];
                        if (nextToken.Tag == TokenType.Terminal)
                        {
                            //Shift
                            //_sw.WriteLine(WS($"{{ \"{ nextToken.Value }\", \"S{ state.Rules[i].NextState }\" }},"));
                            shifts.TryAdd(nextToken.Value, state.Rules[i].NextState);
                        }
                        else
                        {
                            //Goto
                            //_sw.WriteLine(WS($"{{ \"{ nextToken.Value }\", \"{ state.Rules[i].NextState }\" }},"));
                            gotos.TryAdd(nextToken.Value, state.Rules[i].NextState);
                        }
                    }
                    else
                    {
                        //REDUCE
                        foreach (var lookAhead in state.Rules[i].LookAhead)
                        {
                            var ruleNumber = _currentGrammar.GetRuleIndex(state.Rules[i]);
                            if (lookAhead.Tag == TokenType.Dollar)
                            {
                                if (ruleNumber == 0)//Regla Inicial
                                {
                                    _sw.WriteLine(WS($"{{ \"'{ lookAhead.Value }'\", \"ACCEPT\" }},"));
                                }
                                else
                                {
                                    _sw.WriteLine(WS($"{{ \"'{ lookAhead.Value }'\", \"R{ ruleNumber }\" }},"));
                                }
                            }
                            else
                            {
                                if (ruleNumber == 0)
                                {
                                    _sw.WriteLine(WS($"{{ \"{ lookAhead.Value }\", \"ACCEPT\" }},"));
                                }
                                else
                                {
                                    _sw.WriteLine(WS($"{{ \"{ lookAhead.Value }\", \"R{ ruleNumber }\" }},"));
                                }
                            }
                        }
                    }
                }

                foreach (var item in shifts)
                {
                    _sw.WriteLine(WS($"{{ \"{ item.Key }\", \"S{ item.Value }\" }},"));
                }

                foreach (var item in gotos)
                {
                    _sw.WriteLine(WS($"{{ \"{ item.Key }\", \"{ item.Value }\" }},"));
                }
                _prevIndentation = _prevIndentation[1..];
                _sw.WriteLine(WS("};"));
                _sw.WriteLine(WS($"_lalrTable.Add({ state.NodeNumber }, tempRow);"));
                _sw.WriteLine();
            }
            
            EndArea(_sw);
            //Finaliza inicialización de tabla

            //Método Parse
            WriteNewArea(_sw, "public void Parse(string regexp)");
            _sw.WriteLine(WS("_scanner = new Scanner(regexp + (char)TokenType.EOF);"));
            _sw.WriteLine(WS("Token token;"));
            _sw.WriteLine(WS("while ((token = _scanner.GetToken()).Tag != TokenType.EOF) _input.Enqueue(token);"));
            _sw.WriteLine(WS("_input.Enqueue(new Token { Tag = TokenType.Dollar, Value = '$' });"));
            _sw.WriteLine(WS("_states.Push(0);"));
            _sw.WriteLine(WS("_symbols.Push(new Symbol { SymbolType = \"#\" });"));

            //Inicio while
            WriteNewArea(_sw, "while (_input.Count != 0 || _states.Count != 0)");

            //Inicio try
            WriteNewArea(_sw, "try");

            _sw.WriteLine(WS("var inputPeek = _input.Peek();"));
            _sw.WriteLine(WS("var action_goto = _lalrTable[_states.Peek()][$\"'{ inputPeek.Value }'\"];"));

            //Inicio if
            WriteNewArea(_sw, "if (action_goto.Contains(\"S\"))");

            _sw.WriteLine(WS("_states.Push(Convert.ToInt32(action_goto.Remove(0, 1)));"));
            _sw.WriteLine(WS("_symbols.Push(new Symbol { SymbolType = $\"'{ inputPeek.Value }'\", Tokens = new List<Token> { _input.Dequeue() } });"));

            //fin if
            EndArea(_sw);
            //Inicio else if
            WriteNewArea(_sw, "else if (action_goto.Contains(\"R\"))");

            _sw.WriteLine(WS("var ruleNumber = Convert.ToInt32(action_goto.Remove(0, 1));"));
            _sw.WriteLine(WS("var rule = _rules[ruleNumber];"));
            _sw.WriteLine(WS("var productionQty = rule.Count - 1;"));
            _sw.WriteLine(WS("List<Token> temp = new();"));

            //Inicio for-loop
            WriteNewArea(_sw, "for (int i = 0; i < productionQty; i++)");

            _sw.WriteLine(WS("var prevTokens = _symbols.Pop().Tokens;"));
            _sw.WriteLine(WS("foreach (var tokenP in prevTokens) temp.Add(tokenP);"));
            _sw.WriteLine(WS("_states.Pop();"));

            //Fin for-loop
            EndArea(_sw);

            _sw.WriteLine(WS("_symbols.Push(new Symbol { SymbolType = rule[0], Tokens = temp});"));
            _sw.WriteLine(WS("_states.Push(Convert.ToInt32(_lalrTable[_states.Peek()][rule[0]]));"));

            //Fin else if
            EndArea(_sw);
            //Inicio else if ACCEPT
            WriteNewArea(_sw, "else if (action_goto == \"ACCEPT\")");

            _sw.WriteLine(WS("return; //Cadena ok"));

            //Fin else if
            EndArea(_sw);

            //Fin try
            EndArea(_sw);
            //Inicio catch
            WriteNewArea(_sw, "catch");

            _sw.WriteLine(WS("throw new Exception(\"Acción no encontrada en tabla.Parse Error.\");"));

            //Fin catch
            EndArea(_sw);

            //Fin while
            EndArea(_sw);

            //Fin Método Parse
            EndArea(_sw);

            //Fin clase
            EndArea(_sw);
        }

        private void WriteTokenType()
        {
            //Inicio TokenType
            WriteNewArea(_sw, "public enum TokenType");

            _sw.WriteLine(WS("EOF = (char)0,"));
            _sw.WriteLine(WS("Terminal = (char)1,"));
            _sw.WriteLine(WS("NonTerminal = (char)2,"));
            _sw.WriteLine(WS("Dollar = '$'"));

            //Fin TokenType
            EndArea(_sw);
        }

        private void WriteToken()
        {
            //Inicio Token
            WriteNewArea(_sw, "public struct Token");

            _sw.WriteLine(WS("public TokenType Tag;"));
            _sw.WriteLine(WS("public char Value;"));

            //Fin Token
            EndArea(_sw);
        }

        private void WriteConsoleProgram()
        {
            //Inicio Programa
            WriteNewArea(_sw, "class Program");

            //Inicio Main
            WriteNewArea(_sw, "static void Main(string[] args)");

            _sw.WriteLine(WS("string regexp = Console.ReadLine();"));
            _sw.WriteLine(WS("Parser parser = new Parser();"));
            _sw.WriteLine();

            WriteNewArea(_sw, "try");

            _sw.WriteLine(WS("parser.Parse(regexp);"));
            _sw.WriteLine(WS("Console.WriteLine(\"Expresión OK\");"));

            EndArea(_sw);
            
            WriteNewArea(_sw, "catch (Exception ex)");
            _sw.WriteLine(WS("Console.WriteLine(ex.Message);"));
            EndArea(_sw);

            _sw.WriteLine();
            _sw.WriteLine(WS("Console.ReadLine();"));

            //Fin Main
            EndArea(_sw);

            //Fin Programa
            EndArea(_sw);
        }

        private void WriteLexer()
        {
            //Inicio Lexer
            WriteNewArea(_sw, "public class Scanner");

            _sw.WriteLine(WS("private readonly string _regexp = \"\";"));
            _sw.WriteLine(WS("private int _index = 0;"));

            //Inicio Constructor Scanner
            WriteNewArea(_sw, "public Scanner(string regexp)");

            _sw.WriteLine(WS("_regexp = regexp + (char)TokenType.EOF;"));
            _sw.WriteLine(WS("_index = 0;"));

            //Fin Constructor Scanner
            EndArea(_sw);

            //Inicio Procedimiento GetToken
            WriteNewArea(_sw, "public Token GetToken()");

            _sw.WriteLine(WS("Token result = new() { Value = (char)0 };"));
            _sw.WriteLine(WS("bool tokenFound = false;"));

            //Inicio While(Token)
            WriteNewArea(_sw, "while (!tokenFound)");

            _sw.WriteLine(WS("char peek = _regexp[_index];"));

            //Inicio switch(peek)
            WriteNewArea(_sw, "switch (peek)");

            foreach (var terminal in _currentGrammar.Terminals)
            {
                _sw.WriteLine(WS($"case { terminal.Value }:"));
                _prevIndentation += '\t';
                _sw.WriteLine(WS("tokenFound = true;"));
                _sw.WriteLine(WS("result.Tag = TokenType.Terminal;"));
                _sw.WriteLine(WS("result.Value = peek;"));
                _sw.WriteLine(WS("break;"));

                _prevIndentation = _prevIndentation[1..];
            }

            _sw.WriteLine(WS($"case (char)0:"));
            _prevIndentation += '\t';
            _sw.WriteLine(WS("tokenFound = true;"));
            _sw.WriteLine(WS("result.Tag = TokenType.EOF;"));
            _sw.WriteLine(WS("result.Value = peek;"));
            _sw.WriteLine(WS("break;"));

            _prevIndentation = _prevIndentation[1..];

            _sw.WriteLine(WS("default:"));
            _prevIndentation += '\t';
            _sw.WriteLine(WS("throw new Exception(\"Lex Error\");"));
            _prevIndentation = _prevIndentation[1..];
            //Fin switch(peek)
            EndArea(_sw);

            _sw.WriteLine(WS("_index++;"));

            //Fin While(tokenFound)
            EndArea(_sw);

            _sw.WriteLine(WS("return result;"));

            //Fin Procedimiento GetToken
            EndArea(_sw);

            //Fin Lexer
            EndArea(_sw);
        }

        private void GenerateLR0()
        {
            _LR0Graph = new List<GraphNode>();

            _tempRuleList = new List<Production>
            {
                new Production
                {
                    Variable = _currentGrammar.Productions[0].Variable,
                    Result = _currentGrammar.Productions[0].Result
                }
            };

            GenerateNewState(_tempRuleList);
        }

        private void GenerateLALR()
        {
            _LR0Graph[0].Rules[0].LookAhead = new List<Token> { new Token { Tag = TokenType.Dollar, Value = "$" } };
            ModifyStateLookAhead(_LR0Graph[0]);
        }

        private void ModifyStateLookAhead(GraphNode node)
        {
            var nextNodes = new Dictionary<int, List<Production>>();

            foreach (var rule in node.Rules)
            {
                var nextTokenIndex = rule.Result.FindIndex(t => t.Tag == TokenType.Dot) + 1;
                if (nextTokenIndex < rule.Result.Count)
                {
                    var nextToken = rule.Result[nextTokenIndex];
                    if (nextToken.Tag == TokenType.NonTerminal)
                    {
                        List<Token> lookAhead;
                        var lookAheadTokenIndex = nextTokenIndex + 1;
                        if (lookAheadTokenIndex < rule.Result.Count)
                        {
                            var lookAheadToken = rule.Result[lookAheadTokenIndex];
                            if (lookAheadToken.Tag == TokenType.NonTerminal)
                            {
                                lookAhead = _currentGrammar.FirstList[lookAheadToken];
                            }
                            else
                            {
                                lookAhead = new List<Token> { lookAheadToken };
                            }
                        }
                        else
                        {
                            lookAhead = rule.LookAhead;
                        }

                        foreach (var nodeRule in node.Rules)
                        {
                            if (nodeRule.Variable.Value == nextToken.Value)
                            {
                                foreach (var token in lookAhead)
                                {
                                    if (!nodeRule.LookAhead.Contains(token))
                                    {
                                        nodeRule.LookAhead.Add(token);
                                    }
                                }
                            }
                        }
                    }
                }

                if (rule.NextState != -1)
                {
                    if (nextNodes.ContainsKey(rule.NextState))
                    {
                        nextNodes[rule.NextState].Add(rule);
                    }
                    else
                    {
                        nextNodes.Add(rule.NextState, new List<Production> { rule });
                    }
                }
            }

            foreach (var state in nextNodes)
            {
                var compProdList = new List<Production>();
                foreach (var prod in state.Value)
                {
                    var newProd = new Production
                    {
                        Variable = prod.Variable,
                        LookAhead = prod.LookAhead
                    };

                    prod.Result.ForEach(t => newProd.Result.Add(t));

                    var dotPosition = newProd.Result.FindIndex(t => t.Tag == TokenType.Dot);
                    var nextCharacter = newProd.Result[dotPosition + 1];
                    newProd.Result.Reverse(dotPosition, 2);
                    compProdList.Add(newProd);
                }

                if (!_LR0Graph[state.Key].LookAheadIsTheSame(compProdList))
                {
                    _LR0Graph[state.Key].ChangeLookAheadForRules(compProdList);
                    ModifyStateLookAhead(_LR0Graph[state.Key]);
                }
            }
        }

        private int GenerateNewState(List<Production> kernelProductions)
        {
            _tempRuleList = new List<Production>();
            foreach (var prod in kernelProductions)
            {
                var newProd = new Production
                {
                    Variable = prod.Variable,
                };

                prod.Result.ForEach(t => newProd.Result.Add(t));

                var dotPosition = newProd.Result.FindIndex(t => t.Tag == TokenType.Dot);
                if (dotPosition != -1)
                {
                    var nextCharacter = newProd.Result[dotPosition + 1];
                    newProd.Result.Reverse(dotPosition, 2);
                }
                _tempRuleList.Add(newProd);
            }

            var newNode = new GraphNode() { NodeNumber = _LR0Graph.Count };
            var nextTokens = new Dictionary<Token, List<Production>>();

            while (_tempRuleList.Count != 0)
            {
                var production = _tempRuleList[0];
                _tempRuleList.RemoveAt(0);

                if (!production.Result.Any(t => t.Tag == TokenType.Dot))
                {
                    production.Result.Insert(0, new Token { Tag = TokenType.Dot });
                }

                newNode.Rules.Add(production);

                var nextTokenIndex = production.Result.FindIndex(t => t.Tag == TokenType.Dot) + 1;

                if (nextTokenIndex < production.Result.Count)
                {
                    var nextToken = production.Result[nextTokenIndex];
                    if (nextTokens.ContainsKey(nextToken))
                    {
                        nextTokens[nextToken].Add(production);
                    }
                    else
                    {
                        nextTokens.Add(nextToken, new List<Production> { production });
                    }

                    if (nextToken.Tag == TokenType.NonTerminal)
                    {
                        if (!GraphNode.RuleExists(_tempRuleList, newNode, nextToken))
                        {
                            _tempRuleList.AddRange(_currentGrammar.Productions.Where(p => p.Variable.Value == nextToken.Value));
                        }
                    }
                }
            }

            _LR0Graph.Add(newNode);

            foreach (var prod in nextTokens)
            {
                var stateNumber = AlreadyExists(prod.Value);
                if (stateNumber == -1)
                {
                    stateNumber = GenerateNewState(prod.Value); //Obtener estado
                }

                foreach (var production in prod.Value)
                {
                    newNode.AssignNumberToRule(stateNumber, production);
                }
            }

            return newNode.NodeNumber;
        }

        private int AlreadyExists(List<Production> productions)
        {
            var compProdList = new List<Production>();
            foreach (var prod in productions)
            {
                var newProd = new Production
                {
                    Variable = prod.Variable,
                };

                prod.Result.ForEach(t => newProd.Result.Add(t));

                var dotPosition = newProd.Result.FindIndex(t => t.Tag == TokenType.Dot);
                var nextCharacter = newProd.Result[dotPosition + 1];
                newProd.Result.Reverse(dotPosition, 2);
                compProdList.Add(newProd);
            }
            bool exists;

            foreach (var node in _LR0Graph)
            {
                if (node.Rules.Count >= compProdList.Count)
                {
                    exists = true;
                    for (int i = 0; i < compProdList.Count; i++)
                    {
                        exists = exists && Production.CompareProductionResult(compProdList[i], node.Rules[i]);
                        if (!exists) break;
                    }
                    if (exists) return node.NodeNumber;
                }
            }

            return -1;
        }
    }
}