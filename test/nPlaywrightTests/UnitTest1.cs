using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace nPlaywrightTests;

[TestFixture]
public class Tests
{
    private TestChirpWebFactory _factory = null!;

    [OneTimeSetUp]
    public void SetupFactory()
    {
        _factory = new TestChirpWebFactory();
        _factory.CreateDefaultClient();
    }

    [Test]
    public async Task ChirpWebsiteExists()
    {
        using var playwright = await Playwright.CreateAsync();
        
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        string url = "http://localhost:" + _factory.Server.BaseAddress.Port.ToString() + "/";

        await page.GotoAsync(url);

        var title = await page.TitleAsync();

        Assert.Equals("Chirp!", title);


    }

    [OneTimeTearDown]
    public void TearDownFactory()
    {
        _factory.Dispose();
    }

}
