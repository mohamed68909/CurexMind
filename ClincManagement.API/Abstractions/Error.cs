namespace ClincManagement.API.Abstractions
{
    public sealed record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty, StatusCodes.Status200OK);

        public string Code { get; }
        public string Message { get; }
        public int StatusCode { get; set; }

        public Error(string code, string message, int statusCode = StatusCodes.Status400BadRequest)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
        }

        public override string ToString() => $"{Code}: {Message} (Status: {StatusCode})";
    }
}
