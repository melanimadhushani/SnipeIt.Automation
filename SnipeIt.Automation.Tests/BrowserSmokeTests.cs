using Microsoft.Playwright;
using NUnit.Framework;

namespace SnipeIt.Automation.Tests;

[TestFixture]
public class BrowserSmokeTests
{
    [Test]
    public async Task SnipeItLoginPage_ShouldLoadSuccessfully()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
            });

        var page = await browser.NewPageAsync();

        await page.GotoAsync(
            "https://demo.snipeitapp.com/login",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        Assert.That(
            page.Url,
            Does.Contain("demo.snipeitapp.com"));

        Assert.That(
            await page.TitleAsync(),
            Is.Not.Empty,
            "The login page should have a non-empty title.");

        await page.CloseAsync();
    }
}