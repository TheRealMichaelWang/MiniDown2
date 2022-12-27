namespace MiniDown.Parsing
{
    public sealed class UnexpectedTokenException : Exception
    {
        public TokenType RecievedToken { get; private set; }
        public TokenType? ExpectedToken { get; private set; }

        public UnexpectedTokenException(TokenType recievedToken, TokenType? expectedToken) : base($"Got token {recievedToken}{(expectedToken.HasValue ? $", expected {expectedToken.GetValueOrDefault()}" : string.Empty)}.")
        {
            RecievedToken = recievedToken;
            ExpectedToken = expectedToken;
        }
    }
}
