using Appointment_System.Application.DTOs.Office;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Office.Queries
{
    // Query
    public record GetOfficeByIdQuery(int OfficeId) : IRequest<Result<OfficeWithTranslationsDto>>;

    // Handler
    public class GetOfficeByIdQueryHandler
    : IRequestHandler<GetOfficeByIdQuery, Result<OfficeWithTranslationsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOfficeByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OfficeWithTranslationsDto>> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
        {
            var office = await _unitOfWork.OfficeRepository.GetByIdAsync(request.OfficeId);
            if (office is null)
                return Result<OfficeWithTranslationsDto>.Fail("Office not found.");

            var translations = await _unitOfWork.OfficeRepository.GetTranslationsByOfficeIdAsync(request.OfficeId);

            var dto = new OfficeWithTranslationsDto
            {
                Id = office.Id,
                Translations = translations.Select(t => new OfficeTranslationDto
                {
                    Language = t.Language.Value,
                    Name = t.Name,
                    StreetName = t.StreetName,
                    City = t.City,
                    State = t.State,
                    Country = t.Country
                }).ToList()
            };

            return Result<OfficeWithTranslationsDto>.Success(dto);
        }
    }

}
