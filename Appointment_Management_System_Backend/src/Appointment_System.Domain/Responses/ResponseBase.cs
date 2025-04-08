using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Responses
{
    public class ResponseBase<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string? InnerError { get; set; }
        public T? Data { get; set; }

        public ResponseBase(int statusCode, string message, string status, string? innerError = null)
        {
            StatusCode = statusCode;
            Message = message;
            Status = status;
            InnerError = innerError;
        }
    }

}
