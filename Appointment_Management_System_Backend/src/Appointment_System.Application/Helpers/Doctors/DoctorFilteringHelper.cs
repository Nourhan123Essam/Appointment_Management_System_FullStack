using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Application.Helpers.Doctors
{
    public static class DoctorFilteringHelper
    {
        public static List<DoctorBasicDto> ApplyFilters(
      List<DoctorBasicDto> doctors,
      DoctorFilterOptions options,
      string? specializationNameFilter)
        {
            var query = doctors.AsQueryable();

            // Ensure language is supported, otherwise fallback to "en-US"
            var language = Language.IsSupported(options.Language ?? "")
                ? options.Language!
                : Language.English.Value;

            // Filter by language existence
            query = query.Where(d =>
                d.FirstNames.ContainsKey(language) &&
                d.LastNames.ContainsKey(language) &&
                d.Bio.ContainsKey(language) &&
                d.SpecializationNames.ContainsKey(language));

            // Filter by name (partial match on any first/last name)
            if (!string.IsNullOrWhiteSpace(options.Name))
            {
                var name = options.Name.ToLower();
                query = query.Where(d =>
                    d.FirstNames.Values.Any(f => f.ToLower().Contains(name)) ||
                    d.LastNames.Values.Any(l => l.ToLower().Contains(name)));
            }

            // Filter by specialization name (already localized in handler)
            if (!string.IsNullOrWhiteSpace(specializationNameFilter))
            {
                query = query.Where(d =>
                    d.SpecializationNames.ContainsKey(language) &&
                    d.SpecializationNames[language].Any(n =>
                        n.Equals(specializationNameFilter, StringComparison.OrdinalIgnoreCase)));
            }


            // Sorting
            query = options.SortBy?.ToLower() switch
            {
                "rating" => options.SortDescending
                    ? query.OrderByDescending(d =>
                        d.RatingPoints.HasValue && d.NumberOfRatings.HasValue && d.NumberOfRatings.Value > 0
                            ? (double)d.RatingPoints.Value / d.NumberOfRatings.Value
                            : 0)
                    : query.OrderBy(d =>
                        d.RatingPoints.HasValue && d.NumberOfRatings.HasValue && d.NumberOfRatings.Value > 0
                            ? (double)d.RatingPoints.Value / d.NumberOfRatings.Value
                            : 0),

                "experience" => options.SortDescending
                    ? query.OrderByDescending(d => d.YearsOfExperience)
                    : query.OrderBy(d => d.YearsOfExperience),

                "initialfee" => options.SortDescending
                    ? query.OrderByDescending(d => d.InitialFee)
                    : query.OrderBy(d => d.InitialFee),

                "name" => options.SortDescending
                    ? query.OrderByDescending(d =>
                        d.FirstNames.ContainsKey(language)
                            ? d.FirstNames[language]
                            : "")
                    : query.OrderBy(d =>
                        d.FirstNames.ContainsKey(language)
                            ? d.FirstNames[language]
                            : ""),

                _ => query
            };

            return query.ToList();
        }



        public static PaginatedResult<DoctorBasicDto> ApplyPagination(
            List<DoctorBasicDto> filteredDoctors,
            DoctorFilterOptions options)
        {
            var totalCount = filteredDoctors.Count();

            var items = filteredDoctors
                .Skip((options.PageNumber - 1) * options.PageSize)
                .Take(options.PageSize)
                .ToList();

            return PaginatedResult<DoctorBasicDto>.Create(items, totalCount, options.PageNumber, options.PageSize);
        }

    }

}
