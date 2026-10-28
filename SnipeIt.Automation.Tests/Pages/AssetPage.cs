
using Microsoft.Playwright;

namespace SnipeIt.Automation.Tests.Pages;

public sealed class AssetPage(IPage page)
{
    public string CreatedAssetTag { get; private set; } = string.Empty;
    public string SelectedUserName { get; private set; } = string.Empty;

    public async Task OpenAssetsAsync()
    {
        await page.Locator("p")
            .Filter(new() { HasText = "Assets" })
            .ClickAsync();
    }

    public async Task OpenCreateAssetAsync()
    {
        await page.Locator("button[name='btnAdd']")
            .ClickAsync();
    }

    public async Task CreateAssetAsync(string serial, string notes)
    {
        // Select the first Company.
        await page.GetByText(
            "Select Company",
            new() { Exact = true })
            .ClickAsync();

        await page.Locator(
            ".select2-results__option[role='option']:visible")
            .First
            .ClickAsync();

        // Enter Serial Number.
        await page.Locator("#serial_1")
            .FillAsync(serial);

        // Select Model.
        await page.GetByText(
            "Select a Model",
            new() { Exact = true })
            .ClickAsync();

        var searchBox = page.Locator(
            "input.select2-search__field:visible").Last;

        await searchBox.FillAsync("macbook pro 13");

        await page.Locator(
            ".select2-results__option[role='option']:visible")
            .Filter(new() { HasText = "Macbook Pro 13" })
            .First
            .ClickAsync();

        // Select Ready to Deploy status.
        await page.Locator("#select2-status_select_id-container")
            .ClickAsync();

        searchBox = page.Locator(
            "input.select2-search__field:visible").Last;

        await searchBox.FillAsync("Ready to Deploy");

        await page.Locator(
            ".select2-results__option[role='option']:visible")
            .Filter(new() { HasText = "Ready to Deploy" })
            .First
            .ClickAsync();

        // Wait for checkout availability.
        await page.Locator("p")
            .Filter(new()
            {
                HasText = "This asset can be checked out."
            })
            .WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible
            });

        // Wait for Checkout to.
        await page.GetByText("Checkout to")
            .WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible
            });


        // Open the User dropdown.
            await page.Locator(
            "span.select2-selection__placeholder.needsclick:visible")
            .Filter(new()
            {
                HasText = "Select a User"
            })
            .ClickAsync();

        // Locate the actual user options.
        var userOptions = page.Locator(
            ".select2-results__option[role='option']:visible");

        await userOptions.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });

        // Find the first option containing a username and user ID.
        var actualUserOptions = userOptions.Filter(new()
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex(
                @"\(.+\)\s*-\s*#\d+")
        });

        await actualUserOptions.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });

        // Read the option value before clicking.
        var firstUser = actualUserOptions.First;

        var firstUserText = (
            await firstUser.InnerTextAsync()).Trim();

        //Console.WriteLine( $"LOG 1 - User option value: '{firstUserText}'");

        // Click the user option.
        await firstUser.ClickAsync();

        //Console.WriteLine("LOG 2 - User option clicked.");

        // Read the selected value from the dropdown after clicking.
        var selectedUserElement = page.Locator(
            "#select2-assigned_user_select-container");

        await selectedUserElement.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });

        var selectedUserValue = (
            await selectedUserElement.GetAttributeAsync("title")
            ?? string.Empty).Trim();

        //Console.WriteLine(
        //    $"LOG 3 - Selected user value after click: '{selectedUserValue}'");

        // Save only the person's name.
        SelectedUserName = selectedUserValue
            .Split(" (")[0]
            .Trim();

        //Console.WriteLine(
        //    $"LOG 4 - SelectedUserName saved: '{SelectedUserName}'");

        // Stop if the selected username could not be captured.
        if (string.IsNullOrWhiteSpace(SelectedUserName))
        {
            throw new InvalidOperationException(
                "SelectedUserName was empty after selecting the user.");
        }

        // Enter Notes.
        await page.Locator("#notes")
            .FillAsync(notes);

        // Capture the generated Asset Tag.
        CreatedAssetTag = (
            await page.Locator("#asset_tag").InputValueAsync()).Trim();

        if (string.IsNullOrWhiteSpace(CreatedAssetTag))
        {
            throw new System.InvalidOperationException(
                "The Asset Tag was empty before saving.");
        }

        // Save the asset.
        await page.Locator("#submit_button")
            .ClickAsync();
    }

    
public async Task SearchAssetAsync()
{
    // Locate the search field.
    var searchField = page.Locator(
        "input.form-control.search-input");

    await searchField.WaitForAsync(new()
    {
        State = WaitForSelectorState.Visible
    });

    // Enter the generated Asset Tag.
    await searchField.FillAsync(CreatedAssetTag);

    // Trigger the search.
    await searchField.PressAsync("Enter");

    // Wait for the matching asset row.
    var assetRow = page.Locator("tr")
        .Filter(new()
        {
            HasText = CreatedAssetTag
        });

    await assetRow.WaitForAsync(new()
    {
        State = WaitForSelectorState.Visible,
        Timeout = 15000
    });

        // Click the Asset Tag link in the second cell.
await assetRow.Locator("td")
    .Nth(1)
    .Locator("a")
    .ClickAsync();
}


public async Task OpenHistoryAsync()
{
    var historyTab = page.Locator("a[href='#history'][data-toggle='tab']");

    await historyTab.WaitForAsync(
        new() { State = WaitForSelectorState.Visible });

    await historyTab.ClickAsync();

}


}
