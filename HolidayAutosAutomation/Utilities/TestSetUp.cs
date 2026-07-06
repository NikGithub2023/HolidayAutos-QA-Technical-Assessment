using Microsoft.Playwright;

namespace HolidayAutosAutomation.Utilities;

public class TestSetup
{
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;

    public async Task SetupAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = TestConfiguration.Headless,
            SlowMo = TestConfiguration.SlowMo
        };

        Browser = TestConfiguration.Browser.ToLowerInvariant() switch
        {
            "firefox" => await Playwright.Firefox.LaunchAsync(launchOptions),
            "webkit" => await Playwright.Webkit.LaunchAsync(launchOptions),
            _ => await Playwright.Chromium.LaunchAsync(launchOptions)
        };

        Context = await Browser.NewContextAsync();

        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        Page = await Context.NewPageAsync();
        Page.SetDefaultTimeout(TestConfiguration.DefaultTimeoutMs);
        await Page.GotoAsync(TestConfiguration.BaseUrl);
    }
}
