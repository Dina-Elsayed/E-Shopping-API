namespace E_Shopping_API.DTOs
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
    }
}
