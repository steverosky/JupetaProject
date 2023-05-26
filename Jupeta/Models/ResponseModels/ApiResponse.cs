namespace Jupeta.Models.ResponseModels
{
    public class ApiResponse
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public object? ResponseData { get; set; }
    }
    public enum ResponseType
    {
        Success,
        NotFound,
        Error,
        Failure,
        Unauthorized
    }
}
