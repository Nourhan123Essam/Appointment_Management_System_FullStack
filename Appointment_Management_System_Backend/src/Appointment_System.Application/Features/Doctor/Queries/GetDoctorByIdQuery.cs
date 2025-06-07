using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    //Query
    public class GetDoctorByIdQuery : IRequest<Result<DoctorAdminDto>>
    {
        public int Id { get; }

        public GetDoctorByIdQuery(int id)
        {
            Id = id;
        }
    }

    //Handler
    public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, Result<DoctorAdminDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _cacheService;

        public GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork, IRedisCacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<DoctorAdminDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var allDoctors = await _cacheService.GetAllDoctorsAsync();

            if (allDoctors is null)
            {
                allDoctors = await _unitOfWork.DoctorRepository.GetAllForCacheAsync();
                await _cacheService.SetAllDoctorsAsync(allDoctors);
            }

            var doctor = allDoctors.FirstOrDefault(d => d.Id == request.Id);
            if (doctor is null)
                return Result<DoctorAdminDto>.Fail("Doctor not found.");

            var mapped = DoctorAdminDto.FromCacheModel(doctor);
           
            return Result<DoctorAdminDto>.Success(mapped);
        }

    }
}