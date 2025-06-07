using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorAvailabilities.Queries
{
    //Query
    public class GetDoctorAvailabilitiesByDoctorIdQuery : IRequest<IEnumerable<DoctorAvailabilityDto>>
    {
        public int DoctorId { get; set; }

        public GetDoctorAvailabilitiesByDoctorIdQuery(int doctorId)
        {
            DoctorId = doctorId;
        }
    }

    //Handler
    public class GetDoctorAvailabilitiesByDoctorIdHandler : IRequestHandler<GetDoctorAvailabilitiesByDoctorIdQuery, IEnumerable<DoctorAvailabilityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDoctorAvailabilitiesByDoctorIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DoctorAvailabilityDto>> Handle(GetDoctorAvailabilitiesByDoctorIdQuery request, CancellationToken cancellationToken)
        {
            if (request.DoctorId <= 0)
                throw new ArgumentException("DoctorId should be greater than 0");

            var doctorExists = await _unitOfWork.DoctorRepository.GetDoctorByIdAsync(request.DoctorId) != null;
            if (!doctorExists)
                throw new KeyNotFoundException($"Doctor with ID {request.DoctorId} does not exist.");

            var availabilities = await _unitOfWork.AvailabilityRepository.GetByDoctorIdAsync(request.DoctorId);
            return availabilities.Select(a => new DoctorAvailabilityDto(a));
        }
    }

}
