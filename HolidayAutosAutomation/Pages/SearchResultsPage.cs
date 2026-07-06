using System.Text.RegularExpressions;
using HolidayAutosAutomation.Helpers;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace HolidayAutosAutomation.Pages;

public class SearchResultsPage
{
    private readonly IPage _page;

    private ILocator CarBlocks => _page.Locator("[data-auto-id^='divCarBlock']");
    private ILocator SortByPriceButton => _page.Locator("[data-auto-id='ct-sort-bar']").GetByRole(AriaRole.Button, new() { Name = "Sort by Price" });
    public ILocator ResultsCount => _page.Locator("strong.availabilitySummaryTotal").First;
    public ILocator CheapestPriceLocator => TotalPriceIn(CarBlocks.First);

    private static ILocator TotalPriceIn(ILocator carBlock) => carBlock.Locator("[data-auto-id='totalPrice']");

    public SearchResultsPage(IPage page)
    {
        _page = page;
    }

    public async Task WaitForResultsAsync()
    {
        await Expect(CarBlocks.First).ToBeVisibleAsync(new() { Timeout = 15000 });
    }

    public async Task<int> GetResultsCountAsync()
    {
        var text = await ResultsCount.InnerTextAsync();
        var match = Regex.Match(text.Trim(), @"\d+");
        return match.Success ? int.Parse(match.Value) : 0;
    }

    public async Task SortByPriceAsync()
    {
        await SortByPriceButton.ClickAsync();
        await Expect(CarBlocks.Nth(2)).ToBeVisibleAsync(new() { Timeout = 10000 });

        var blocks = await CarBlocks.AllAsync();
        var prices = await Task.WhenAll(
            blocks.Take(3).Select(async b => PriceHelper.ParsePrice(await TotalPriceIn(b).InnerTextAsync()))
        );

        if (prices[0] >= prices[1] || prices[1] >= prices[2])
            throw new InvalidOperationException(
                $"Results are not sorted by price ascending: {prices[0]} → {prices[1]} → {prices[2]}");
    }

    public async Task<string> GetCheapestCarPriceAsync()
    {
        return await TotalPriceIn(CarBlocks.First).InnerTextAsync();
    }

    public async Task<IPage> SelectCheapestCarAsync()
    {
        var vehiclePage = await _page.Context.RunAndWaitForPageAsync(async () =>
            await CarBlocks.First.GetByRole(AriaRole.Button, new() { Name = "Select" }).ClickAsync());
        await vehiclePage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        return vehiclePage;
    }
}
