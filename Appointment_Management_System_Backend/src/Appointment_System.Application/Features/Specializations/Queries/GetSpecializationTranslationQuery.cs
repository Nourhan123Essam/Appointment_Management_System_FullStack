using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Queries
{
    public record GetSpecializationTranslationQuery(int SpecializationId, string Language) : IRequest<Result<SpecializationTranslationDto>>;

    public class GetSpecializationTranslationQueryHandler : IRequestHandler<GetSpecializationTranslationQuery, Result<SpecializationTranslationDto>>
    {
        private readonly ISpecializationRepository _repository;

        public GetSpecializationTranslationQueryHandler(ISpecializationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<SpecializationTranslationDto>> Handle(GetSpecializationTranslationQuery request, CancellationToken cancellationToken)
        {
            var translation = await _repository.GetTranslationAsync(request.SpecializationId, request.Language);
            if (translation == null)
                return Result<SpecializationTranslationDto>.Fail("Translation not found.");

            var dto = new SpecializationTranslationDto
            {
                Id = translation.Id,
                SpecializationId = translation.SpecializationId,
                Language = translation.LanguageValue.Value,
                Name = translation.Name
            };

            return Result<SpecializationTranslationDto>.Success(dto);
        }
    }

}
