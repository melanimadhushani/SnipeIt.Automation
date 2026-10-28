using Microsoft.Playwright;

namespace SnipeIt.Automation.Tests.Pages;

public sealed class LoginPage(IPage page)
{
    private const string Username = "admin";
    private const string Password = "password";

    public async Task NavigateAsync(string baseUrl)
    {
        await page.GotoAsync(
            $"{baseUrl.TrimEnd('/')}/login");
    }

    public async Task LoginAsync()
    {
        await page.Locator("#username")
            .FillAsync(Username);

        await page.Locator("#password-field")
            .FillAsync(Password);

        await page.Locator("#submit")
            .ClickAsync();
    }
}