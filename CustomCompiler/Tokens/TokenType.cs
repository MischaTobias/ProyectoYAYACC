namespace CustomCompiler.Tokens
{
    public enum TokenType
    {
        Colon = ':',
        SemiColon = ';',
        Pipe = '|',
        Apostrophe = '\'',
        BackSlash = '\\',
        EOF = (char)0,
        Terminal = (char)1,
        NonTerminal = (char)2,
        Dollar = (char)3
    }
}
