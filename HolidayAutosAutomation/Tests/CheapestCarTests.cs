using HolidayAutosAutomation.Pages;
using HolidayAutosAutomation.Utilities;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Tests;

[TestFixture]
public class CheapestCarTests
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
    public async Task SearchResults_FindCheapestCarPrice()
    {
        var homePage = new HomePage(_setup.Page);
        await homePage.AcceptCookies();
        await homePage.SearchAsync("London Heathrow Airport", DateTime.Today.AddDays(1), DateTime.Today.AddDays(5));

        var resultsPage = new SearchResultsPage(_setup.Page);
        await resultsPage.WaitForResultsAsync();
        await resultsPage.SortByPriceAsync();

        await Expect(resultsPage.CheapestPriceLocator).Not.ToBeEmptyAsync();
        Console.WriteLine($"Cheapest car price: {await resultsPage.GetCheapestCarPriceAsync()}");
    }
}
