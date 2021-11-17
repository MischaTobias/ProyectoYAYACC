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
        Hash = '#',
        EOF = (char)0,
        Terminal = (char)1,
        NonTerminal = (char)2,
        Dot = '•'
    }
}
