using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Pages;

public class CalendarComponent
{
    private readonly IPage _page;

    public CalendarComponent(IPage page)
    {
        _page = page;
    }

    public async Task SelectDate(ILocator input, DateTime date)
    {
        await input.ClickAsync();
        await Expect(CalendarDate(date)).ToBeVisibleAsync();
        await CalendarDate(date).ClickAsync();
    }

    private ILocator CalendarDate(DateTime date) =>
        _page.Locator($"[data-date-formatted='{date:yyyy-MM-dd}']");
}