namespace MiniDown.Parsing
{
    public enum TokenType
    {
        OtherCharacters,
        Hashtag,
        Bullet,
        Backtick,
        Space,
        Tab,
        Newline,
        Eof
    }

    public struct Token
    {
        public TokenType Type { get; private set; }
        public char Char { get;private set; }

        public Token(TokenType type, char @char)
        {
            Type = type;
            Char = @char;
        }
    }
}
