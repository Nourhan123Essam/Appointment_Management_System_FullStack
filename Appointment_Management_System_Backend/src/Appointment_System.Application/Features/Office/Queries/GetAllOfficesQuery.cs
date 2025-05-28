using Appointment_System.Application.DTOs.Office;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Office.Queries
{
    // Query
    public record GetAllOfficesQuery : IRequest<Result<List<OfficeWithTranslationsDto>>>;

    // Hnadler
    public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, Result<List<OfficeWithTranslationsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllOfficesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<OfficeWithTranslationsDto>>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
        {
            var offices = await _unitOfWork.OfficeRepository.GetAllWithTranslationsAsync();

            return Result<List<OfficeWithTranslationsDto>>.Success(offices, "Offices retrieved successfully.");
        }
    }

}
