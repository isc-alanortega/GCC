namespace Nubetico.Shared.Static.Core
{
    public static class Constants
    {
        public static readonly List<LanguageOption> Languages = new()
        {
            new LanguageOption { Code = "en-US", Name = "English (United States)" },
            new LanguageOption { Code = "es-MX", Name = "Español (México)" }
        };

        public static readonly List<TimeZoneOption> TimeZones = TimeZoneInfo
            .GetSystemTimeZones()
            .Select(tz => new TimeZoneOption
            {
                Id = tz.Id,
                DisplayName = tz.DisplayName
            })
            .ToList();
    }

    public class LanguageOption
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class TimeZoneOption
    {
        public string Id { get; set; } = string.Empty; // America/Mexico_City
        public string DisplayName { get; set; } = string.Empty; // (UTC-06:00) Eastern Time (US & Canada)
    }
}
