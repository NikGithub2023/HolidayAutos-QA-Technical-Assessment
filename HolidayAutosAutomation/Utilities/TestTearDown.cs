using Microsoft.Playwright;

namespace HolidayAutosAutomation.Utilities;

public class TestTearDown
{
    public bool failed;

    public async Task CloseContextAndStopTracing(IBrowserContext Context, IBrowser browser, IPlaywright playwright, string testName)
    {
        //Check test failure or error status
        failed = TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Error
     || TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Failure;

        //Always stop tracing and save a report file for every test
        await Context.Tracing.StopAsync(new()
        {
            Path = $"../../../Reports/{testName}.zip"
        });

        //Always close the browser context
        await Context.CloseAsync();
        await browser.CloseAsync();
        playwright.Dispose();
    }
}
