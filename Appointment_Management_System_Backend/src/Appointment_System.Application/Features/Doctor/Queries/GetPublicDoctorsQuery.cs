using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Helpers.Doctors;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    // Query
    public class GetPublicDoctorsQuery : IRequest<Result<PaginatedResult<DoctorPublicDto>>>
    {
        public DoctorFilterOptions FilterOptions { get; set; }

        public GetPublicDoctorsQuery(DoctorFilterOptions filterOptions)
        {
            FilterOptions = filterOptions;
        }
    }


    // Handler
    public class GetPublicDoctorsQueryHandler : IRequestHandler<GetPublicDoctorsQuery, Result<PaginatedResult<DoctorPublicDto>>>
    {
        private readonly IRedisCacheService _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpecializationRepository _specializationRepo;
        private readonly ILocalizationService _localizer;

        public GetPublicDoctorsQueryHandler(
            IRedisCacheService cache,
            IUnitOfWork unitOfWork,
            ISpecializationRepository specializationRepo,
            ILocalizationService localizer)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _specializationRepo = specializationRepo;
            _localizer = localizer;
        }

        public async Task<Result<PaginatedResult<DoctorPublicDto>>> Handle(GetPublicDoctorsQuery request, CancellationToken cancellationToken)
        {
            var filter = request.FilterOptions;
            string specializationNameFilter = null!;

            // STEP 1: Check specialization ID
            if (filter.SpecializationId != null)
            {
                int id = (int)filter.SpecializationId;

                var specializationExists = await _specializationRepo.ExistsAsync(id);
                if (!specializationExists)
                    return Result<PaginatedResult<DoctorPublicDto>>.Fail(_localizer["SpecializationNotFound"]);

                var requestedLang = Language.IsSupported(filter.Language ?? "")
                    ? filter.Language
                    : Language.English.Value;

                var translation = await _specializationRepo.GetTranslationAsync(id, requestedLang);
                specializationNameFilter = translation?.Name ?? "";
            }

            // STEP 2: Try to get all doctors from cache
            var cachedDoctors = await _cache.GetAllDoctorsAsync();

            // STEP 3: If not found in cache, fetch from DB and store in cache
            if (cachedDoctors == null)
            {
                cachedDoctors = await _unitOfWork.DoctorRepository.GetAllForCacheAsync();
                await _cache.SetAllDoctorsAsync(cachedDoctors);
            }

            // STEP 4: Apply filters ONLY to count
            var filteredWithoutPagination = DoctorFilteringHelper.ApplyFilters(cachedDoctors, filter, specializationNameFilter);
            var totalCount = filteredWithoutPagination.Count;

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
            if (filter.PageNumber > totalPages && totalPages > 0)
            {
                return Result<PaginatedResult<DoctorPublicDto>>.Fail($"Page number {filter.PageNumber} exceeds total pages ({totalPages}).");
            }

            // STEP 5: Paginate the *filtered* result
            var paginated = DoctorFilteringHelper.ApplyPagination(filteredWithoutPagination, filter);

            // STEP 6: Map to public DTO
            var language = filter.Language!;
            var mapped = paginated.Items.Select(doc => DoctorPublicDto.FromCacheModel(doc, language)).ToList();

            return Result<PaginatedResult<DoctorPublicDto>>.Success(
                PaginatedResult<DoctorPublicDto>.Create(mapped, paginated.TotalCount, paginated.PageNumber, paginated.PageSize)
            );
        }
    }


    // Validator
    public class GetPublicDoctorsQueryValidator : AbstractValidator<GetPublicDoctorsQuery>
    {
        public GetPublicDoctorsQueryValidator()
        {
            RuleFor(q => q.FilterOptions.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(q => q.FilterOptions.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(q => q.FilterOptions.Language)
                .NotEmpty().WithMessage("Language is required.")
                .Must(lang => Language.SupportedLanguages.Any(l => l.Value == lang))
                .WithMessage($"Supported languages are: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");
        }
    }
}
