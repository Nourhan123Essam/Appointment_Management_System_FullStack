namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorFilterOptions
    {
        public string? Name { get; set; }
        public int? SpecializationId { get; set; }
        public string? Language { get; set; }

        // Sorting: "rating", "experience", "initialFee", "name"
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
