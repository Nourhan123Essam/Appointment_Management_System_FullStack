using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Infrastructure.Seeders;

namespace Appointment_System.Infrastructure.Data.Seed;

public class DoctorTestDataSeeder
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly ApplicationDbContext _context;

    public DoctorTestDataSeeder(IDoctorRepository doctorRepository, 
        ISpecializationRepository specializationRepository, 
        IOfficeRepository officeRepository,
        ApplicationDbContext context)
    {
        _doctorRepository = doctorRepository;
        _specializationRepository = specializationRepository;
        _officeRepository = officeRepository;
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Sedding Offices and Specialization
        var officeSeeding = new OfficeTestDataSeeder(_context);
        var specializationSeeding = new SpecializationTestDataSeeder(_context);
        await officeSeeding.SeedAsync();
        await specializationSeeding.SeedAsync();

        // Fetch Offices and Specialization from DB
        var specializations = await _specializationRepository.GetAllWithTranslationsAsync();
        var offices = await _officeRepository.GetAllAsync();
        var random = new Random();

        for (int i = 1; i <= 100; i++)
        {
            var specializationIds = specializations.OrderBy(x => random.Next()).Take(2).Select(s => s.Id).ToList();
            var officeId = offices[random.Next(offices.Count)].Id;

            var dto = new CreateDoctorDto
            {
                Email = $"doctor{i}@example.com",
                UserId = $"user-{i}",
                Phone = $"010000000{i:D3}",
                Password = "StrongP@ssword123!",
                ImageUrl = "https://example.com/images/doctor.jpg",
                InitialFee = 150,
                FollowUpFee = 75,
                MaxFollowUps = 3,
                YearsOfExperience = random.Next(5, 30),
                SpecializationIds = specializationIds,
                Translations = new List<DoctorTranslationDto>
                {
                    new DoctorTranslationDto
                    {
                        Language = "en-US",
                        FirstName = $"John{i}",
                        LastName = "Doe",
                        Bio = "Experienced specialist in test data generation"
                    },
                    new DoctorTranslationDto
                    {
                        Language = "ar-EG",
                        FirstName = $"جون{i}",
                        LastName = "دو",
                        Bio = "طبيب متخصص في توليد بيانات الاختبار"
                    }
                },
                Qualifications = new List<CreateQualificationDto>
                {
                    new CreateQualificationDto
                    {
                        Date = 2010 + (i % 10),
                        Translations = new List<QualificationTranslationDto>
                        {
                            new QualificationTranslationDto
                            {
                                Language = "en-US",
                                Title = "MD Test Specialty",
                                Institution = "Test University"
                            },
                            new QualificationTranslationDto
                            {
                                Language = "ar-EG",
                                Title = "دكتوراه في التخصص التجريبي",
                                Institution = "جامعة الاختبار"
                            }
                        }
                    }
                },
                Availabilities = new List<CreateDoctorAvailabilityDto>
                {
                    new CreateDoctorAvailabilityDto
                    {
                        DayOfWeek = (DayOfWeek)random.Next(0, 7),
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        OfficeId = officeId
                    },
                    new CreateDoctorAvailabilityDto
                    {
                        DayOfWeek = (DayOfWeek)random.Next(0, 7),
                        StartTime = new TimeSpan(13, 0, 0),
                        EndTime = new TimeSpan(20, 0, 0),
                        OfficeId = officeId
                    }
                }
            };

            await _doctorRepository.CreateDoctorWithUserAsync(dto, dto.Password);
        }
    }
}
