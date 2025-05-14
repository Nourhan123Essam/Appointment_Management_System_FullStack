using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Queries
{
    // Query
    public class GetUserIdByEmailQuery: IRequest<string>
    {
        public string email;
        public GetUserIdByEmailQuery(string _email) { email = _email; }
    }

    // Handler
    public class GetUserIdByEmailHandler: IRequestHandler<GetUserIdByEmailQuery, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserIdByEmailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(GetUserIdByEmailQuery request, CancellationToken cancellationToken)
        {
            var uerId = await _unitOfWork.Authentication.GetUserIdByEmailAsync(request.email);
            return uerId;
        }
    }
}
