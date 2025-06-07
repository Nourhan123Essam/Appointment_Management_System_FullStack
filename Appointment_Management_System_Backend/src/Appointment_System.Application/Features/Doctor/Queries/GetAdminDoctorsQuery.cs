using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Helpers.Doctors;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;
using FluentValidation;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    // Query
    public class GetAdminDoctorsQuery : IRequest<Result<PaginatedResult<DoctorAdminDto>>>
    {
        public DoctorFilterOptions FilterOptions { get; set; }

        public GetAdminDoctorsQuery(DoctorFilterOptions filterOptions)
        {
            FilterOptions = filterOptions;
        }
    }

    // Handler
    public class GetAdminDoctorsQueryHandler : IRequestHandler<GetAdminDoctorsQuery, Result<PaginatedResult<DoctorAdminDto>>>
    {
        private readonly IRedisCacheService _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpecializationRepository _specializationRepo;
        private readonly ILocalizationService _localizer;

        public GetAdminDoctorsQueryHandler(IRedisCacheService cache, IUnitOfWork unitOfWork, 
            ISpecializationRepository specializationRepository, ILocalizationService localizationService)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _specializationRepo = specializationRepository;
            _localizer = localizationService;                                                                                                                                                                                   
        }

        public async Task<Result<PaginatedResult<DoctorAdminDto>>> Handle(GetAdminDoctorsQuery request, CancellationToken cancellationToken)
        {
            var filter = request.FilterOptions;
            string specializationNameFilter = null!;

            // STEP 1: Handle Specialization ID (optional)
            if (filter.SpecializationId != null)
            {
                int id = (int)filter.SpecializationId;

                // 1.1 Retrieve specialization from DB
                var specializationExist = await _specializationRepo.ExistsAsync(id);
                if (!specializationExist)
                {
                    // 1.2 Return failure if not found
                    return Result<PaginatedResult<DoctorAdminDto>>.Fail(_localizer["SpecializationNotFound"]);
                }

                // 1.3 Use requested language if available, fallback to default
                var requestedLang = Language.IsSupported(filter.Language?? "")
                    ? filter.Language
                    : Language.English.Value;

                var translation = await _specializationRepo.GetTranslationAsync(id, requestedLang);
                specializationNameFilter = translation?.Name ?? "";
            }

            // STEP 2: Get cached doctors
            var cachedDoctors = await _cache.GetAllDoctorsAsync();
            if (cachedDoctors == null)
            {
                cachedDoctors = await _unitOfWork.DoctorRepository.GetAllForCacheAsync();
                await _cache.SetAllDoctorsAsync(cachedDoctors);
            }

            // STEP 3: Apply filters (with resolved specialization name)
            var filteredWithoutPagination = DoctorFilteringHelper.ApplyFilters(
                cachedDoctors, request.FilterOptions, specializationNameFilter);

            var totalCount = filteredWithoutPagination.Count;

            // STEP 4: Validate page number is within range
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
            if (filter.PageNumber > totalPages) { 
                return Result<PaginatedResult<DoctorAdminDto>>.Fail(
                    $"Page number {filter.PageNumber} exceeds total pages ({totalPages}).");
            }

            // STEP 5: Apply pagination (should paginate the *filtered* list)
            var paginated = DoctorFilteringHelper.ApplyPagination(filteredWithoutPagination, filter);

            // STEP 6: Map to DTOs
            var mapped = paginated.Items.Select(DoctorAdminDto.FromCacheModel).ToList();

            // STEP 7: Return success
            return Result<PaginatedResult<DoctorAdminDto>>.Success(
                PaginatedResult<DoctorAdminDto>.Create(mapped, paginated.TotalCount, paginated.PageNumber, paginated.PageSize));
        }

    }

    // Validator
    public class GetAdminDoctorsQueryValidator : AbstractValidator<GetAdminDoctorsQuery>
    {
        public GetAdminDoctorsQueryValidator()
        {
            RuleFor(q => q.FilterOptions.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(q => q.FilterOptions.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
        }
    }

}
