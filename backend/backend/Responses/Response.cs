namespace backend.Responses;

public class Response<T> where T : class
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public T? ReturnedObject { get; set; }
}