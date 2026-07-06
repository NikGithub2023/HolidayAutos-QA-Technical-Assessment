using Microsoft.Extensions.Configuration;

namespace HolidayAutosAutomation.Utilities;

public static class TestConfiguration
{
    private static readonly IConfiguration _configuration;

    static TestConfiguration()
    {
        // Single appsettings.json for now. Additional environment-specific files
        // (e.g. appsettings.uat.json) can be layered on top as environments are added.
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }

    public static string BaseUrl =>
        _configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");

    public static string Browser => _configuration["Browser"] ?? "Chromium";

    public static bool Headless => bool.Parse(_configuration["Headless"] ?? "false");

    public static float SlowMo => float.Parse(_configuration["SlowMo"] ?? "0");

    public static int DefaultTimeoutMs => int.Parse(_configuration["DefaultTimeoutMs"] ?? "30000");
}
