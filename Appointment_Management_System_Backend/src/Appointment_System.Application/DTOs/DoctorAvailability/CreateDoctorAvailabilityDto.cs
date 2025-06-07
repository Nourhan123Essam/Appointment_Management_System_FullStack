namespace Appointment_System.Application.DTOs.DoctorAvailability
{
    public class CreateDoctorAvailabilityDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DoctorId { get; set; }
        public int OfficeId { get; set; }

        public Domain.Entities.Availability ToEntity()
        {
            return new Domain.Entities.Availability()
            {
                Id = 0,
                DayOfWeek = DayOfWeek,
                StartTime = StartTime,
                EndTime = EndTime,
                DoctorId = DoctorId,
                OfficeId = OfficeId

            };
        }
    }
}
