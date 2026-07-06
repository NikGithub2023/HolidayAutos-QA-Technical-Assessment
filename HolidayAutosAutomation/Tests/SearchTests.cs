using HolidayAutosAutomation.Pages;
using HolidayAutosAutomation.Utilities;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SearchTests
{
    private TestSetup _setup = null!;
    private string _testName = null!;

    [SetUp]
    public async Task SetUp()
    {
        _testName = TestContext.CurrentContext.Test.Name;
        _setup = new TestSetup();
        await _setup.SetupAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        var tearDown = new TestTearDown();
        await tearDown.CloseContextAndStopTracing(_setup.Context, _setup.Browser, _setup.Playwright, _testName);
    }

    [Test]
    public async Task Search_Should_DisplayAvailableCars()
    {
        var homePage = new HomePage(_setup.Page);
        await homePage.AcceptCookies();
        await homePage.SearchAsync("London Heathrow Airport", DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));

        var resultsPage = new SearchResultsPage(_setup.Page);
        await resultsPage.WaitForResultsAsync();

        await Expect(resultsPage.CheapestPriceLocator).ToBeVisibleAsync();
    }
}