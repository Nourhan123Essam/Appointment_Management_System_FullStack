using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Patient.Queries
{
    //Query
    public class GetPatientsQuery : IRequest<PagedResult<PatientDto>>
    {
        public PatientQueryParams QueryParams { get; }

        public GetPatientsQuery(PatientQueryParams queryParams)
        {
            QueryParams = queryParams;
        }
    }

    //Handler
    public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, PagedResult<PatientDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPatientsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Patients.GetPatientsAsync(request.QueryParams);
        }
    }

}
