
using Microsoft.Playwright;
using NUnit.Framework;
using SnipeIt.Automation.Tests.Pages;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SnipeIt.Automation.Tests.Tests;

[TestFixture]
public class AssetTests
{
    [Test]
    public async Task CreateAsset_SearchAndVerifyDetailsAndHistory()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

        var page = await browser.NewPageAsync();

        var loginPage = new LoginPage(page);
        var assetPage = new AssetPage(page);

        // 1. Login.
        await loginPage.NavigateAsync(
            "https://demo.snipeitapp.com");

        await loginPage.LoginAsync();

        await Assertions.Expect(page)
            .ToHaveURLAsync("https://demo.snipeitapp.com/");

        // 2. Open Assets.
        await assetPage.OpenAssetsAsync();

        await Assertions.Expect(page)
            .ToHaveURLAsync(new Regex(".*/hardware.*"));

        // 3. Open Create Asset form.
        await assetPage.OpenCreateAssetAsync();

        // 4. Generate unique test data.
        var serial = $"SERIAL-{Guid.NewGuid():N}"[..20];
        var notes = $"Created by Playwright assessment - {Guid.NewGuid():N}";

        // 5. Create asset and check it out to a user.
        await assetPage.CreateAssetAsync(serial, notes);

        // 6. Verify successful creation.
        var successAlert = page.Locator(
            ".alert.alert-success.fade.in");

        await Assertions.Expect(successAlert).ToBeVisibleAsync();

        await Assertions.Expect(successAlert)
            .ToContainTextAsync("Asset with tag");

        await Assertions.Expect(successAlert)
            .ToContainTextAsync(assetPage.CreatedAssetTag);

        await Assertions.Expect(successAlert)
            .ToContainTextAsync("was created successfully");

        await Assertions.Expect(page)
            .ToHaveURLAsync("https://demo.snipeitapp.com/hardware");


        // 7. Search for the newly created asset.
        await assetPage.SearchAssetAsync();

        //  Verify the asset navigation heading details.
        // 8.1 Verify the asset tag in the heading (breadcrumb.)
                var breadcrumb = page.Locator("li.breadcrumb-item").Last;
                await Assertions.Expect(breadcrumb) .ToContainTextAsync(assetPage.CreatedAssetTag);

        // 8.2. Verify the asset tag and Model in the navigation breadcrumb.
             var assetBreadcrumbLink = page.Locator(
             "li.breadcrumb-item a[href^='/hardware/']");

             await Assertions.Expect(assetBreadcrumbLink)
             .ToContainTextAsync( $"{assetPage.CreatedAssetTag} - Macbook Pro 13\"");    

        // 9 Verify the navigation breadcrumb contains Details.
            await Assertions.Expect(page.Locator("li.breadcrumb-item").Last).ToContainTextAsync("Details");

        //  Verify the asset  details in the content.
        // 10. Verify Asset Tag.
        await Assertions.Expect( page.Locator("span.js-copy-asset_tag")).ToContainTextAsync(assetPage.CreatedAssetTag);

        // 11. Verify Model.
        await Assertions.Expect(
            page.GetByText(
                "Macbook Pro 13\"",
                new() { Exact = true }))
            .ToBeVisibleAsync();

        // 12. Verify Serial Number.
        await Assertions.Expect(
            page.GetByText(serial, new() { Exact = true }))
            .ToBeVisibleAsync();

        // 13. Verify Notes.
        await Assertions.Expect(
            page.GetByText(notes, new() { Exact = true }))
            .ToBeVisibleAsync();

        // 14. Verify Category.
        await Assertions.Expect(
            page.Locator("a:has-text('Laptops')").First)
            .ToBeVisibleAsync();

         // 15. Verify the asset status and checkout status.
        // 15.1 Verify the asset status is "Ready to Deploy".
        var statusContainer = page.Locator("div.col-md-12.well") .Filter(new()
        {
             HasText = "Ready to Deploy"
        }).First;
        await Assertions.Expect(statusContainer) .ToContainTextAsync("Ready to Deploy");

         // 15.2 Verify the Deployed indicator after checkout.
         var deployedStatus = page.Locator( "label:has-text('Deployed')");

        await Assertions.Expect(deployedStatus) .ToBeVisibleAsync();
        await Assertions.Expect(deployedStatus) .ToHaveTextAsync("Deployed");

      // 16. Verify the assigned user on the asset Details page.

    //Console.WriteLine( $"Username at Details assertion: '{assetPage.SelectedUserName}'");

    // 16.1 Locate the user link in the status container.
        var userLink = page.Locator(
        "div.col-md-12.well a[href*='/users/']")
        .First;

    // 16.2 Verify the link is visible.
        await Assertions.Expect(userLink)
         .ToBeVisibleAsync();

        // 16.3 Read the actual username displayed in the UI.
        var actualUserName = (await userLink.InnerTextAsync()).Trim();

        // 16.4 Compare the displayed username with the selected username.
            Assert.That(
            actualUserName,
            Is.EqualTo(assetPage.SelectedUserName),
            "The displayed assigned user should match the user selected during asset creation.");

         // 17. Verify checkout information is present.
        await Assertions.Expect(
            page.Locator("strong")
                .Filter(new() { HasText = "Last Checkout" })
                .First)
            .ToBeVisibleAsync();
 
       
        // 18. Open History.
        await assetPage.OpenHistoryAsync();

        // 19. Verify the History breadcrumb.
        await Assertions.Expect(
            page.Locator("li.breadcrumb-item.active").Last)
            .ToContainTextAsync("History");

        // 20. Verify the History heading.
        await Assertions.Expect(
            page.Locator("h3").Filter(new()
            {
                HasText = "History"
            }).First)
            .ToBeVisibleAsync();


        // 21. Verify the checkout history record.
        var checkoutRow = page.Locator("tr")
            .Filter(new() { HasText = "checkout" })
            .Filter(new() { HasText = assetPage.CreatedAssetTag })
            .First;

        await Assertions.Expect(checkoutRow).ToBeVisibleAsync();

        // 22. Verify the asset tag in the checkout record.
        await Assertions.Expect(
           checkoutRow.Locator("td").Nth(4))
           .ToContainTextAsync(
            $"{assetPage.CreatedAssetTag} - Macbook Pro 13\"");

        // 23. Verify the Created By user in the checkout record.
        await Assertions.Expect(
            checkoutRow.Locator("td").Nth(2))
            .ToContainTextAsync("User Admin");

//Console.WriteLine($"Username at History assertion: '{assetPage.SelectedUserName}'");
    
        // 24. Verify the Target User in the checkout record.
        await Assertions.Expect(
            checkoutRow.Locator("td").Nth(5))
            .ToContainTextAsync(assetPage.SelectedUserName);
        
        // 25. Verify quantity.
        await Assertions.Expect(
        checkoutRow.Locator("td").Nth(8))
        .ToContainTextAsync("1");

        
        // 26. Locate the "create new" history row.
        var createNewRow = page.Locator("tr")
            .Filter(new() { HasText = "create new" })
            .Filter(new() { HasText = assetPage.CreatedAssetTag })
            .First;

        await Assertions.Expect(createNewRow).ToBeVisibleAsync();

        
        // 26. Verify the asset tag and model in the "create new" record.
        await Assertions.Expect(
            createNewRow.Locator("td").Nth(4))
            .ToContainTextAsync(
                $"{assetPage.CreatedAssetTag} - Macbook Pro 13\"");


        // 27. Verify the Created By user.
        await Assertions.Expect(
            createNewRow.Locator("td").Nth(2))
            .ToContainTextAsync("User Admin");

        // 28. Verify the quantity.
        await Assertions.Expect(
            createNewRow.Locator("td").Nth(8))
            .ToContainTextAsync("1");

    }
}
