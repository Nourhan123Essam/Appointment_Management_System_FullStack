using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorAvailabilities.Queries
{
    //Query
    public class GetDoctorAvailabilityByIdQuery : IRequest<DoctorAvailabilityDto?>
    {
        public int Id { get; set; }

        public GetDoctorAvailabilityByIdQuery(int id)
        {
            Id = id;
        }
    }

    //Handler
    public class GetDoctorAvailabilityByIdHandler : IRequestHandler<GetDoctorAvailabilityByIdQuery, DoctorAvailabilityDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDoctorAvailabilityByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorAvailabilityDto?> Handle(GetDoctorAvailabilityByIdQuery request, CancellationToken cancellationToken)
        {
            var availability = await _unitOfWork.AvailabilityRepository.GetByIdAsync(request.Id);
            return availability == null ? null : new DoctorAvailabilityDto(availability);
        }
    }

}
