using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.ValueObjects
{
    public sealed class Language : ValueObject
    {
        public string Value { get; private set; }

        public static readonly Language English = new("en-US");
        public static readonly Language Arabic = new("ar-EG");

        public static IReadOnlyCollection<Language> SupportedLanguages => new[] { English, Arabic };

        public Language() { } // Needed for EF Core
        public Language(string value) => Value = value;


        public static Language From(string value)
        {
            var match = SupportedLanguages.FirstOrDefault(l => l.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (match is null)
                throw new ArgumentException($"Unsupported language: {value}");

            return match;
        }

        public static bool IsSupported(string value)
        {
            return SupportedLanguages.Any(l => l.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static bool AreAllSupported(IEnumerable<string> inputLanguages)
        {
            var normalized = inputLanguages.Select(v =>
            {
                try { return From(v).Value; } catch { return null; }
            }).Where(v => v != null).ToHashSet();

            var required = SupportedLanguages.Select(l => l.Value).ToHashSet();
            return required.All(normalized.Contains);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant(); // Ensures case-insensitive comparison
        }

        public override string ToString() => Value;
    }


}