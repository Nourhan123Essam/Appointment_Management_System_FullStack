using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Queries
{
    public record GetSpecializationsWithDoctorCountQuery(string Language)
     : IRequest<Result<List<SpecializationWithDoctorCountDto>>>;

    public class GetSpecializationsWithDoctorCountQueryHandler
        : IRequestHandler<GetSpecializationsWithDoctorCountQuery, Result<List<SpecializationWithDoctorCountDto>>>
    {
        private readonly ISpecializationRepository _repository;

        public GetSpecializationsWithDoctorCountQueryHandler(ISpecializationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<SpecializationWithDoctorCountDto>>> Handle(
            GetSpecializationsWithDoctorCountQuery request,
            CancellationToken cancellationToken)
        {
            // Call repository method (already implemented)
            var specializations = await _repository.GetWithDoctorCountAsync(request.Language);

            return Result<List<SpecializationWithDoctorCountDto>>.Success(specializations);
        }
    }
}
