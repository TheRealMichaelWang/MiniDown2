namespace MiniDown.Parsing
{
    internal class Scanner
    {
        private readonly string source;
        private int position;

        public Token LastToken { get; private set; }

        public Scanner(string source)
        {
            this.source = source;
            position = 0;

            ScanToken();
        }

        private char ScanChar()
        {
            if (position == source.Length)
                return '\0';
            return source[position++];
        }

        public Token ScanToken()
        {
            char c;
            do
            {
                c = ScanChar();
            }
            while (c == '\r');

            return LastToken = new Token(c switch
            {
                ' ' => TokenType.Space,
                '\t' => TokenType.Tab,
                '#' => TokenType.Hashtag,
                '*' => TokenType.Bullet,
                '`' => TokenType.Backtick,
                '\n' => TokenType.Newline,
                '\0' => TokenType.Eof,
                _ => TokenType.OtherCharacters
            }, c);
        }
    }
}
