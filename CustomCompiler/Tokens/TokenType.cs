namespace CustomCompiler.Tokens
{
    public enum TokenType
    {
        Colon = ':',
        SemiColon = ';',
        Pipe = '|',
        Apostrophe = '\'',
        BackSlash = '\\',
        Dollar = '$',
        EOF = (char)0,
        Terminal = (char)1,
        NonTerminal = (char)2
    }
}
