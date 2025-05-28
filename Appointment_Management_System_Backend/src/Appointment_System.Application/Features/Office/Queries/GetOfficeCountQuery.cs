using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Office.Queries
{
    // Query
    public record GetOfficeCountQuery() : IRequest<Result<int>>;

    // Handler
    public class GetOfficeCountQueryHandler
    : IRequestHandler<GetOfficeCountQuery, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOfficeCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetOfficeCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.OfficeRepository.CountAsync();
            return Result<int>.Success(count);
        }
    }

}
