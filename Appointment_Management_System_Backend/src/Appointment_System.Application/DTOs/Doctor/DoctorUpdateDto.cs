using MediatR;

namespace Appointment_System.Application.DTOs.Doctor
{
    public record DoctorUpdateDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DoctorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; }
        public string? Bio { get; set; }
        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int MaxFollowUps { get; set; }
        public int YearsOfExperience { get; set; }
        public string Phone {  get; set; }

        public DoctorUpdateDto(){}

        public void ToEnitiy(Domain.Entities.Doctor doctor)
        {
            doctor.FirstName = FirstName;
            doctor.LastName = LastName;
            doctor.Bio = Bio;
            doctor.Phone = Phone;
            doctor.Email = Email;
            doctor.YearsOfExperience = YearsOfExperience;
            doctor.InitialFee = InitialFee;
            doctor.FollowUpFee = FollowUpFee;
            doctor.MaxFollowUps = MaxFollowUps;
        }
    }

}
