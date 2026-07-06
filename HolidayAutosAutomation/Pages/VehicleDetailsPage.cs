using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Pages;

public class VehicleDetailsPage
{
    private readonly IPage _page;

    private ILocator CarName => _page.Locator("[data-auto-id='ct-vehicle-block-title'] h3");
    public ILocator TotalPrice => _page.Locator("[data-auto-id='totalPrice']");
    public ILocator PickupDate => _page.Locator("strong[data-auto-id='searchFormPickupDateReadOnly']");
    public ILocator ReturnDate => _page.Locator("strong[data-auto-id='searchFormReturnDateReadOnly']");

    public VehicleDetailsPage(IPage page)
    {
        _page = page;
    }

    public async Task WaitForPageLoadAsync()
    {
        await Expect(TotalPrice).ToBeVisibleAsync();
    }

    public async Task<string> GetCarNameAsync() => await CarName.InnerTextAsync();
    public async Task<string> GetTotalPriceAsync() => await TotalPrice.InnerTextAsync();
    public async Task<string> GetPickupDateAsync() => await PickupDate.InnerTextAsync();
    public async Task<string> GetReturnDateAsync() => await ReturnDate.InnerTextAsync();
}
