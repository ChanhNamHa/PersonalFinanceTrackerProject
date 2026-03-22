public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; } // Chỉ hiện khi ở môi trường Development

    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}