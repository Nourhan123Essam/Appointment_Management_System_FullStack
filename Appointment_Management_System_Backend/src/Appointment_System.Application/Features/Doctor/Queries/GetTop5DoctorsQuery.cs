using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Helpers.Doctors;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    // Query
    public record GetTop5DoctorsQuery(string Language) : IRequest<Result<List<DoctorBasicDto>>>;

    // Handler
    public class GetTop5DoctorsHandler : IRequestHandler<GetTop5DoctorsQuery, Result<List<DoctorBasicDto>>>
    {
        private readonly IRedisCacheService _cache;
        private readonly IDoctorRepository _doctorRepository;

        public GetTop5DoctorsHandler(IRedisCacheService cache, IDoctorRepository doctorRepository)
        {
            _cache = cache;
            _doctorRepository = doctorRepository;
        }

        public async Task<Result<List<DoctorBasicDto>>> Handle(GetTop5DoctorsQuery request, CancellationToken cancellationToken)
        {
            // 1. Try get top 5 doctors from cache
            var top5 = await _cache.GetTop5DoctorsAsync();
            if (top5 is not null)
                return Result<List<DoctorBasicDto>>.Success(top5);

            // 2. Try get all doctors from cache
            var allDoctors = await _cache.GetAllDoctorsAsync();
            if (allDoctors is null)
            {
                // 3. Fetch all doctors from DB and cache them
                allDoctors = await _doctorRepository.GetAllForCacheAsync();
                await _cache.SetAllDoctorsAsync(allDoctors);
            }

            // 4. Filter for top 5 by rating (language-aware)
            var filterOptions = new DoctorFilterOptions
            {
                Language = request.Language,
                SortBy = "rating",
                SortDescending = true,
                PageNumber = 1,
                PageSize = 5
            };

            var top5Filtered = DoctorFilteringHelper.ApplyFilters(allDoctors, filterOptions, "").ToList();

            // 5. Cache the top 5 doctors for 30 minutes
            await _cache.SetTop5DoctorsAsync(top5Filtered);

            return Result<List<DoctorBasicDto>>.Success(top5Filtered);
        }
    }


}
