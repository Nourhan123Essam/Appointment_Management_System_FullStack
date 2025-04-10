
namespace Appointment_System.Application.DTOs.Patient
{
    public class PatientQueryParams
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
