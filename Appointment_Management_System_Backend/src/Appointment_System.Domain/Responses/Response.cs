namespace Appointment_System.Domain.Responses
{
    public class Response<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public Response(bool succeeded, string message = "", T? data = default)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }

        // Optional static helpers
        public static Response<T> Success(T data, string message = "") => new(true, message, data);
        public static Response<T> Fail(string message) => new(false, message);
    }
}
