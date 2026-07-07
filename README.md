# HolidayAutos Automation – Technical Assignment

## 1. Project Overview

This repository is a submission for the QA Automation technical assignment. The goal is to demonstrate a structured testing approach against [www.holidayautos.com](https://www.holidayautos.com), covering the full flow of searching for a rental car, identifying the cheapest option, and verifying that the vehicle details page reflects the correct price and date range.

This repository contains both the **manual test cases** and the **Playwright automation** completed for the technical assignment, built with a clean, maintainable framework using Playwright and NUnit in C#.

---

## 2. Technology Stack

| Tool / Library | Version | Purpose |
|---|---|---|
| .NET | 10.0.3 | Target framework |
| C# | Latest | Language |
| NUnit | 4.3.2 | Test framework – attributes, lifecycle hooks, and assertions |
| NUnit3TestAdapter | 6.2.0 | Bridges NUnit with `dotnet test` for test discovery |
| Microsoft.NET.Test.Sdk | 18.7.0 | Required MSBuild targets and test host that `dotnet test` needs to build and execute tests |
| Microsoft.Playwright | 1.61.0 | Browser automation |
| NUnit.Analyzers | 4.7.0 | Static analysis / code quality |
| coverlet.collector | 6.0.4 | Code coverage collection |
| Microsoft.Extensions.Configuration.Json | 10.0.9 | Loads `appsettings.json` at runtime |
| Microsoft.Extensions.Configuration.Binder | 10.0.9 | Typed access to configuration values |

---

## 3. Framework Design

The framework follows the **Page Object Model (POM)** pattern, keeping all page interactions encapsulated in dedicated page classes. Test classes contain only test logic and assertions, with no raw locators or browser interactions directly in the test body.

### Project Structure

```
HolidayAutosTestPack/
├── README.md
├── .gitignore
└── HolidayAutosAutomation/
    ├── HolidayAutosAutomation.csproj
    ├── appsettings.json               # Configuration (browser, URL, timeouts)
    ├── Components/
    │   └── CalendarComponent.cs       # Reusable date picker interaction
    ├── Documentation/
    │   └── Manual_Test_Cases.md       # Three written manual test cases
    ├── Helpers/
    │   └── PriceHelper.cs             # Parses price strings to decimal
    ├── Pages/
    │   ├── HomePage.cs                # Search form interactions
    │   ├── SearchResultsPage.cs       # Results list, sort, price extraction
    │   └── VehicleDetailsPage.cs      # Booking details page assertions
    ├── Tests/
    │   ├── SearchTests.cs             # Verifies search returns results
    │   ├── CheapestCarTests.cs        # Identifies cheapest car on results page
    │   └── BookingTests.cs            # Validates price and dates on details page
    └── Utilities/
        ├── TestConfiguration.cs       # Reads and exposes typed config values
        ├── TestSetUp.cs               # Browser/context/tracing initialisation
        └── TestTearDown.cs            # Trace export and browser cleanup
```

### Folder Purposes

- **Components** – Reusable UI component wrappers (e.g. the date picker calendar) that are shared across multiple page objects.
- **Documentation** – Written manual test cases in Markdown format.
- **Helpers** – Static utility methods for common operations such as parsing a formatted price string into a comparable decimal.
- **Pages** – One class per page, each encapsulating all locators and interactions for that page. Tests never contain raw selectors.
- **Tests** – NUnit test classes, each focused on a single test scenario. All browser setup is delegated to the `Utilities` classes.
- **Utilities** – Shared browser lifecycle management, including Playwright tracing start/stop and browser teardown.

---

## 4. Testing Approach

Before writing any automation, I conducted exploratory testing against the live site to understand how the application behaves, identify stable selectors, and note any quirks in the UI (such as the behaviour of the date picker, dropdown suggestions for locations, and how prices are displayed across pages).

Once the application behaviour was understood, test data was picked to keep the tests stable across repeated runs:

- **London Heathrow Airport** was selected as a stable location that consistently returns suggestions to select in the dropdown.
- Dates are calculated dynamically at runtime (relative to `DateTime.Today`) rather than being hardcoded, so tests do not fail due to past dates.
- The pickup date is set one day ahead to avoid inventory availability issues on same-day searches.
- The drop off date is set to five days after today to match common real world scenarios.

---

## 5. Assumptions

A few decisions made during Exploratory Testing phase:

- **London Heathrow Airport** was chosen as the pickup location because it reliably returns a consistent volume of results, making the tests stable across runs.
- The pickup location is selected from the **autocomplete dropdown** rather than typed in full, matching real user behaviour and ensuring the correct location code is submitted.
- **Pickup date** is dynamically set to `DateTime.Today.AddDays(1)` to avoid same-day inventory restrictions.
- **Drop-off date** is dynamically set to `DateTime.Today.AddDays(5)` in all the tests to ensure a valid rental window.
- **Pickup and drop-off times** are not explicitly selected. The application automatically populates sensible default times and the assignment did not require validating time selection.
- The tests assume that clicking **"Sort by Price (Low to High)"** correctly identifies the cheapest available vehicle, and the sort order is programmatically verified against the first three results before proceeding.

---

## 6. Manual Test Coverage

Three manual test cases were written prior to automation to validate the core user journey. They are documented in [HolidayAutosAutomation/Documentation/Manual_Test_Cases.md](HolidayAutosAutomation/Documentation/Manual_Test_Cases.md).

| ID | Description | Result |
|---|---|---|
| Test Case 1 | Search for available rental cars using valid criteria | PASS |
| Test Case 2 | Identify the cheapest available rental car from results | PASS |
| Test Case 3 | Select cheapest car and verify price and date range on details page | FAIL – decimal price variation observed |

---

## 7. Automated Test Coverage

### `SearchTests.cs` – Search returns results

Navigates to the home page, accepts cookies, searches for **London Heathrow Airport** with a pickup date of `today + 1` and drop-off of `today + 5`, then asserts that at least one car block with a visible price is present on the results page.

### `CheapestCarTests.cs` – Cheapest car can be identified

Runs the same search with a pickup date of `today + 1` and drop-off of `today + 5`, sorts the results by **Price (Low to High)**, and asserts that the first result's price element is not empty. The identified price is also written to the test output for visibility.

### `BookingTests.cs` – Vehicle details match search criteria

Performs the full end-to-end flow: searches, sorts by price, captures the cheapest price from the results page, clicks through to the vehicle details page in the newly opened tab, and asserts that:
- The pickup date matches the expected value (formatted as `d MMM yyyy`).
- The drop-off date matches the expected value.
- The total price displayed on the details page contains the price captured from the results page.

---

## 8. Framework Highlights

### Page Object Model
All page interactions are encapsulated in `Pages/`. Locators are private where possible, and only the properties or methods that tests need are exposed publicly.

### Reusable Calendar Component
The date picker is abstracted into `CalendarComponent`, which accepts any `ILocator` input and a target `DateTime`. Both `HomePage` pick-up and drop-off date fields use this single component.

```csharp
// Components/CalendarComponent.cs
public async Task SelectDate(ILocator input, DateTime date)
{
    await input.ClickAsync();
    await Expect(CalendarDate(date)).ToBeVisibleAsync();
    await CalendarDate(date).ClickAsync();
}

private ILocator CalendarDate(DateTime date) =>
    _page.Locator($"[data-date-formatted='{date:yyyy-MM-dd}']");
```

### Price Helper
`PriceHelper.ParsePrice` strips currency symbols and non-numeric characters from a price string and returns a `decimal`, used in `SearchResultsPage` to validate sort order across the first three results.

```csharp
// Helpers/PriceHelper.cs
public static decimal ParsePrice(string priceText)
{
    var cleaned = Regex.Replace(priceText, @"[^\d\.]", string.Empty);
    return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
        ? result : 0m;
}
```

### Sort Order Validation
After clicking "Sort by Price", the framework validates that the first three results are genuinely in ascending order before proceeding, catching any sort failures before the test continues.

```csharp
// Pages/SearchResultsPage.cs
if (prices[0] >= prices[1] || prices[1] >= prices[2])
    throw new InvalidOperationException(
        $"Results are not sorted by price ascending: {prices[0]} → {prices[1]} → {prices[2]}");
```

### Centralised Configuration
Settings that would otherwise be hardcoded are kept in `appsettings.json`. `TestConfiguration` loads this file at startup and exposes typed properties — no raw strings in test or setup code. Additional environment files (e.g. `appsettings.uat.json`) can be layered on top as more environments are introduced.

```json
// appsettings.json
{
  "BaseUrl": "https://www.holidayautos.com",
  "Browser": "Chromium",
  "Headless": false,
  "SlowMo": 0,
  "DefaultTimeoutMs": 30000
}
```

```csharp
// Utilities/TestConfiguration.cs
public static string BaseUrl =>
    _configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
public static string Browser => _configuration["Browser"] ?? "Chromium";
public static bool Headless => bool.Parse(_configuration["Headless"] ?? "false");
public static int DefaultTimeoutMs => int.Parse(_configuration["DefaultTimeoutMs"] ?? "30000");
```

### Shared Browser Setup with Playwright Tracing
`TestSetup` initialises the browser, context, and Playwright trace capture (with screenshots, snapshots, and sources) before every test.

```csharp
// Utilities/TestSetUp.cs
await Context.Tracing.StartAsync(new()
{
    Screenshots = true,
    Snapshots = true,
    Sources = true
});
```

### NUnit Lifecycle Hooks (SetUp / TearDown)
Every test class uses `[SetUp]` to spin up a fresh browser context and `[TearDown]` to export the trace and close the browser, whether the test passes or fails.

```csharp
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
```

### Assertion Style
Playwright's built-in `Expect` API is used throughout rather than NUnit's `Assert`, giving auto-retry behaviour and descriptive failure messages.

```csharp
await Expect(detailsPage.PickupDate).ToContainTextAsync(_pickupDate.ToString("d MMM yyyy"));
await Expect(detailsPage.TotalPrice).ToContainTextAsync(cheapestPrice);
```

---

## 9. Prerequisites
- Git
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PowerShell or a compatible terminal
- Internet access to reach `www.holidayautos.com`

---

## 10. How to Run the Project

### Clone the repository

```bash
git clone <repository-url>
cd HolidayAutosTestPack
```

### Restore NuGet packages

```bash
dotnet restore HolidayAutosAutomation/HolidayAutosAutomation.csproj
dotnet build HolidayAutosAutomation/HolidayAutosAutomation.csproj
```

### Install Playwright browsers

```bash
cd HolidayAutosAutomation
powershell -ExecutionPolicy Bypass -File .\HolidayAutosAutomation\bin\Debug\net10.0\playwright.ps1 install
```

### Run all tests

```bash
dotnet test HolidayAutosAutomation/HolidayAutosAutomation.csproj
```

### Run a specific test class

```bash
dotnet test --filter "ClassName=SearchTests"
dotnet test --filter "ClassName=CheapestCarTests"
dotnet test --filter "ClassName=BookingTests"
```

### Run a specific test method

```bash
dotnet test --filter "FullyQualifiedName~SelectCheapestCar_VerifyPriceAndDateRange"
```

> **Note:** Tests run in headed (visible) browser mode by default. To switch to headless, set `"Headless": true` in `appsettings.json`.

### Switch browser

Set `"Browser"` in `appsettings.json` to `"Chromium"`, `"Firefox"`, or `"WebKit"` — no code changes needed.

---

## 11. Test Reports

After each test run, Playwright traces are saved to:

```
HolidayAutosAutomation/Reports/<TestName>.zip
```

Traces are generated for every test, pass or fail.

To view a trace:

```bash
playwright show-trace HolidayAutosAutomation/Reports/<TestName>.zip
```

The Playwright Trace Viewer provides a full timeline of the test, including screenshots at each step, DOM snapshots, network activity, and the source code that triggered each action.

---

## 12. Observations

A few things noted during exploratory and automated testing:

- **Pickup and drop-off dates** matched correctly between the search form, the results page, and the vehicle details page across all test runs.
- A **small decimal variation** was observed between the total price displayed on the search results page and the price shown on the vehicle details page (for example, `Euro 35` vs `Euro 34.84`). This appears to be a rounding or presentation difference applied at the details stage.
- The automation **does not suppress or tolerate this difference**. The assertion performs a strict string match, meaning the test will surface this discrepancy rather than silently pass over it. This was intentional – hiding the difference would reduce the value of the test.
- Changing Pick-up location while enabling drop off location on **search results page** will reset drop off location same as Pick-up location while the behaviour is not the same on Home page

---

## 13. Future Improvements

A few things I would look to add with more time:

- **CI/CD integration** - A GitHub Actions workflow to trigger the test suite on every push or pull request, with trace artifacts published as pipeline attachments.
- **Parallel execution** - `[Parallelizable(ParallelScope.All)]` applying this to all test classes with isolated contexts would cut overall suite runtime.
- **Structured reporting** - Integrating a reporter such as Allure to produce HTML test summaries alongside the Playwright traces.
- **Negative and boundary scenarios** - Testing invalid locations, past dates, past time, same pickup and drop-off dates, and very short or very long rental windows to validate the application's error handling.
- **Retry mechanism** - Adding `[Retry(2)]` on specific tests that might fail due to network failures without marking the test as a genuine failure.
- **Test categories** - Tagging tests with `[Category("Smoke")]` or `[Category("Regression")]` to allow targeted runs from the CLI or CI pipeline rather than always running the full suite.
- **Ticket references** - Linking each test to a requirement or bug via `[Description]` to provide traceability back to Jira or Azure DevOps tickets.
