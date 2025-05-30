using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Queries
{
    // Query
    public record GetSpecializationCountQuery() : IRequest<Result<int>>;

    // Handler
    public class GetSpecializationCountQueryHandler : IRequestHandler<GetSpecializationCountQuery, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSpecializationCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(GetSpecializationCountQuery request, CancellationToken cancellationToken)
        {
            int count = await _unitOfWork.SpecializationRepository.CountAsync();
            return Result<int>.Success(count);
        }
    }
}
