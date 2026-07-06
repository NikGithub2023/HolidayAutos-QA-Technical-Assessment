using HolidayAutosAutomation.Pages;
using HolidayAutosAutomation.Utilities;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Tests;

[TestFixture]
public class BookingTests
{
    private TestSetup _setup = null!;
    private string _testName = null!;
    private DateTime _pickupDate;
    private DateTime _dropOffDate;

    [SetUp]
    public async Task SetUp()
    {
        _pickupDate = DateTime.Today.AddDays(1);
        _dropOffDate = DateTime.Today.AddDays(5);
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
    public async Task SelectCheapestCar_VerifyPriceAndDateRange()
    {
        var homePage = new HomePage(_setup.Page);
        await homePage.AcceptCookies();
        await homePage.SearchAsync("London Heathrow Airport", _pickupDate, _dropOffDate);

        var resultsPage = new SearchResultsPage(_setup.Page);
        await resultsPage.WaitForResultsAsync();
        await resultsPage.SortByPriceAsync();

        var cheapestPrice = await resultsPage.GetCheapestCarPriceAsync();
        var vehiclePage = await resultsPage.SelectCheapestCarAsync();

        var detailsPage = new VehicleDetailsPage(vehiclePage);
        await detailsPage.WaitForPageLoadAsync();

        await Expect(detailsPage.PickupDate).ToContainTextAsync(_pickupDate.ToString("d MMM yyyy"));
        await Expect(detailsPage.ReturnDate).ToContainTextAsync(_dropOffDate.ToString("d MMM yyyy"));
        await Expect(detailsPage.TotalPrice).ToContainTextAsync(cheapestPrice);
    }
}
