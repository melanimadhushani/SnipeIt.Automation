
using Microsoft.Playwright;
using NUnit.Framework;
using SnipeIt.Automation.Tests.Pages;

namespace SnipeIt.Automation.Tests.Tests;

[TestFixture]
public class LoginTests
{
    [Test]
    public async Task Login_WithValidCredentials_ShouldSucceed()
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

        // Navigate to the login page.
        await loginPage.NavigateAsync(
            "https://demo.snipeitapp.com");

        // Log in using the credentials defined in LoginPage.cs.
        await loginPage.LoginAsync();

        // Verify successful login.
        await Assertions.Expect(page)
            .ToHaveURLAsync(
                "https://demo.snipeitapp.com/");

        await browser.CloseAsync();
    }
}
