using Appointment_System.Application.DTOs.Office;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Office.Queries
{
    // Query
    public record GetLocalizedOfficeQuery(int OfficeId, string Language)
    : IRequest<Result<OfficeTranslationDto>>;

    // Handler
    public class GetLocalizedOfficeQueryHandler
    : IRequestHandler<GetLocalizedOfficeQuery, Result<OfficeTranslationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLocalizedOfficeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OfficeTranslationDto>> Handle(GetLocalizedOfficeQuery request, CancellationToken cancellationToken)
        {
            // Optional: normalize language (if you still use the Language ValueObject, update here)
            var language = request.Language;

            var officeExists = await _unitOfWork.OfficeRepository.ExistsAsync(request.OfficeId);
            if (!officeExists)
                return Result<OfficeTranslationDto>.Fail("Office not found.");

            var translation = await _unitOfWork.OfficeRepository
                .GetTranslationAsync(request.OfficeId, language);

            if (translation is null)
                return Result<OfficeTranslationDto>.Fail("Translation not found for the specified language.");

            var dto = new OfficeTranslationDto
            {
                Id = translation.Id,
                OfficeId = request.OfficeId,
                Language = translation.Language.Value,
                Name = translation.Name,
                StreetName = translation.StreetName,
                City = translation.City,
                State = translation.State,
                Country = translation.Country
            };

            return Result<OfficeTranslationDto>.Success(dto, "Translation loaded.");
        }
    }

}
