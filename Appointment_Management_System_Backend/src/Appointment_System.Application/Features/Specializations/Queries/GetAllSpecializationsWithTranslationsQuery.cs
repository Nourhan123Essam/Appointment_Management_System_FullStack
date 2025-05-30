using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Queries
{
    public record GetAllSpecializationsWithTranslationsQuery : IRequest<Result<List<SpecializationWithTranslationsDto>>>;

    public class GetAllSpecializationsWithTranslationsQueryHandler
        : IRequestHandler<GetAllSpecializationsWithTranslationsQuery, Result<List<SpecializationWithTranslationsDto>>>
    {
        private readonly ISpecializationRepository _repository;

        public GetAllSpecializationsWithTranslationsQueryHandler(ISpecializationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<SpecializationWithTranslationsDto>>> Handle(
            GetAllSpecializationsWithTranslationsQuery request,
            CancellationToken cancellationToken)
        {
            var specializations = await _repository.GetAllWithTranslationsAsync();
            return Result<List<SpecializationWithTranslationsDto>>.Success(specializations);
        }
    }
}
