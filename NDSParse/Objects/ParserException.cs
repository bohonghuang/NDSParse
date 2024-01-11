using Serilog;

namespace NDSParse.Objects;

public class ParserException : Exception
{
    public ParserException(string message) : base(message)
    {
        Log.Error("Parser Exception: {Message}", message);
    }
}