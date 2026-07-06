using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Pages;

public class HomePage
{
    private readonly IPage _page;
    private readonly CalendarComponent _calendar;
    public HomePage(IPage page)
    {
        _page = page;
        _calendar = new CalendarComponent(page);
    }

    // Locators
    private ILocator AcceptCookiesButton => _page.Locator("#onetrust-accept-btn-handler");
    private ILocator PickupLocation => _page.GetByRole(AriaRole.Textbox, new() { Name = "Pick-up location" });
    private ILocator AirportOptions => _page.Locator("li.ct-drop-down-option");
    private ILocator PickupDate => _page.GetByLabel("Start date");
    private ILocator EndDate => _page.GetByLabel("End date");
    private ILocator PickupTimeInput => _page.Locator("#pickupTime");
    private ILocator PickupTimeListbox => _page.Locator("#pickupTime-listbox");
    private ILocator SearchButton => _page.GetByRole(AriaRole.Button, new() { Name = "search" });

    public async Task AcceptCookies()
    {
        try
        {
            await AcceptCookiesButton.WaitForAsync(new() { Timeout = 5000 });
            await AcceptCookiesButton.ClickAsync();
        }
        catch (PlaywrightException) { }
    }

    public async Task SelectPickupLocation(string location)
    {
        await PickupLocation.PressSequentiallyAsync(location, new() { Delay = 50 });
        await Expect(AirportOptions.First).ToBeVisibleAsync();
        await AirportOptions.First.ClickAsync();
    }

    public async Task SelectPickupDate(DateTime date)
    {
        await _calendar.SelectDate(PickupDate, date);
    }

    public async Task SelectEndDate(DateTime date)
    {
        await _calendar.SelectDate(EndDate, date);
    }

    public async Task ClickSearchAsync()
    {
        await SearchButton.ClickAsync();
    }

    public async Task SearchAsync(string location, DateTime pickupDate, DateTime dropOffDate)
    {
        await SelectPickupLocation(location);
        await SelectPickupDate(pickupDate);
        await SelectEndDate(dropOffDate);
        await ClickSearchAsync();
    }
}