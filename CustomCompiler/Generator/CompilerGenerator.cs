﻿using System;
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
        private int _newStateNumber;
        private List<Production> _tempRuleList;
        private List<GraphNode> _LR0Graph;

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
                if (!_currentGrammar.Productions.Any(p => p.Variable.Value == nonTerminal))
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
                _sw.WriteLine();
                //Comienzo namespace
                WriteNewArea(_sw, "namespace NewGrammar");

                //TokenType
                WriteTokenType();

                //Token
                WriteToken();

                //Scanner
                WriteLexer();

                //Console Program
                WriteConsoleProgram();

                GenerateLR0();

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

        private void WriteTokenType()
        {
            //Inicio TokenType
            WriteNewArea(_sw, "public enum TokenType");

            _sw.WriteLine(WS("EOF = (char)0,"));
            _sw.WriteLine(WS("Terminal = (char)1,"));
            _sw.WriteLine(WS("NonTerminal = (char)2"));

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
            _sw.WriteLine(WS("Scanner scanner = new(regexp);"));
            _sw.WriteLine();

            _sw.WriteLine(WS("Token nextToken;"));

            //Inicio Do-While
            WriteNewArea(_sw, "do");

            _sw.WriteLine(WS("nextToken = scanner.GetToken();"));
            _sw.WriteLine(WS("Console.WriteLine($\"Token: {nextToken.Tag}, Value: {nextToken.Value}\");"));

            //Fin Do-While
            _prevIndentation = _prevIndentation[1..];
            _sw.WriteLine(WS("} while (nextToken.Tag != TokenType.EOF);"));

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
                _sw.WriteLine(WS($"case {terminal}:"));
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

            _newStateNumber = 0;

            _tempRuleList = new List<Production>
            {
                new Production 
                {
                    Variable = _currentGrammar.Productions[0].Variable,
                    Result = _currentGrammar.Productions[0].Result
                }
            };

            GenerateNewState(_tempRuleList);

            var x = 0;
        }

        private void GenerateNewState(List<Production> kernelProductions)
        {
            _tempRuleList = new List<Production>();
            _tempRuleList.AddRange(kernelProductions);

            var newNode = new GraphNode();

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

                    if (nextToken.Tag == TokenType.NonTerminal)
                    {
                        if (!GraphNode.RuleExists(_tempRuleList, newNode, nextToken))
                        {
                            _tempRuleList.AddRange(_currentGrammar.Productions.Where(p => p.Variable.Value == nextToken.Value));
                        }
                    }
                }
            }

            var nextCharacters = new Dictionary<Token, List<Production>>();
            foreach (var prod in newNode.Rules)
            {
                var newProd = new Production
                {
                    Variable = prod.Variable,
                };

                prod.Result.ForEach(t => newProd.Result.Add(t));

                var dotPosition = newProd.Result.FindIndex(t => t.Tag == TokenType.Dot);
                if (dotPosition < newProd.Result.Count - 1)
                {
                    var nextCharacter = newProd.Result[dotPosition + 1];
                    newProd.Result.Reverse(dotPosition, 2);

                    if (nextCharacters.ContainsKey(nextCharacter))
                    {
                        nextCharacters[nextCharacter].Add(newProd);
                    }
                    else
                    {
                        nextCharacters.Add(nextCharacter, new List<Production> { newProd });
                    }
                }
            }

            _LR0Graph.Add(newNode);

            foreach (var prod in nextCharacters)
            {
                GenerateNewState(prod.Value);
            }
        }
    }
}
