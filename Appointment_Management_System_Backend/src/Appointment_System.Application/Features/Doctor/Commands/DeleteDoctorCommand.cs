using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands
{
    //Command
    public class DeleteDoctorCommand : IRequest<Result<string>>
    {
        public int DoctorId { get; set; }

        public DeleteDoctorCommand(int doctorId)
        {
            DoctorId = doctorId;
        }
    }

    //Handler
    public class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redis;

        public DeleteDoctorHandler(IUnitOfWork unitOfWork, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _redis = redisCacheService;
        }

        public async Task<Result<string>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctorId = request.DoctorId;

            // Step 1: Check if the doctor exists
            var doctor = await _unitOfWork.DoctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor is null)
                return Result<string>.Fail("Doctor not found.");

            // Step 2: Delete from DB
            await _unitOfWork.DoctorRepository.DeleteAsync(doctorId);
            await _unitOfWork.SaveChangesAsync();

            // Step 3: Update Redis cache
            var cachedDoctors = await _redis.GetAllDoctorsAsync();
            if (cachedDoctors is not null)
            {
                cachedDoctors = cachedDoctors
                    .Where(d => d.Id != doctorId) // Remove the deleted doctor
                    .ToList();

                await _redis.SetAllDoctorsAsync(cachedDoctors);
            }

            return Result<string>.Success("", "Doctor deleted.");
        }
    }

}
