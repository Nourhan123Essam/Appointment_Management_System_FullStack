namespace Appointment_System.Application.DTOs.Specialization
{
    public record SpecializationDto
    {
        public string Name { get; set; } = null!;
        public SpecializationDto(){}
        public SpecializationDto(Domain.Entities.Specialization specialization)
        {
            Name = specialization.Name;
        }
    }
}
