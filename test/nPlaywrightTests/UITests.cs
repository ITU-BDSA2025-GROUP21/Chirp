using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace nPlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UITests : PageTest
{
    private TestChirpWebFactory _factory = null!;
    private string _baseUrl = null!;

    [OneTimeSetUp]
    public async Task SetupFactory()
    {
        _factory = new TestChirpWebFactory();
        await _factory.StartAsync();
        _baseUrl = _factory.BaseAddress;
    }

    [Test, Order(1)]
    public async Task ChirpWebsiteExists()
    {
        var response = await Page.GotoAsync(_baseUrl);

        TestContext.Out.WriteLine($"Navigated to {response?.Url} with status {response?.Status}");
        if (response is { Status: not 200 })
        {
            TestContext.Out.WriteLine(await response.TextAsync());
        }

        Assert.That(response, Is.Not.Null, "No response returned from the web app.");
        Assert.That(response!.Status, Is.EqualTo(200), $"Home page returned status {response.Status}.");

        var title = await Page.TitleAsync();
        Assert.That(title, Is.EqualTo("Chirp!"));
    }

    [Test, Order(2)]
    public async Task likeButton()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync("noah");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("noah@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("(0)");
        await Page.GetByRole(AriaRole.Button, new() { Name = "(0)" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("(1)");
        await Page.GetByRole(AriaRole.Button, new() { Name = "(1)" }).First.ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("(0)");
    }

    [Test, Order(3)]
    public async Task timelineChange()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("noah@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.Locator("h2")).ToContainTextAsync("Public Timeline");
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.Locator("h2")).ToContainTextAsync("noah's Timeline");
    }

    [Test, Order(4)]
    public async Task dislikeButton()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("noah@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("(1)");
        await Page.GetByRole(AriaRole.Button, new() { Name = "(1)" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("(2)");
        await Page.GetByRole(AriaRole.Button, new() { Name = "(2)" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("(1)");
    }

    [Test, Order(5)]
    public async Task accountPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("noah@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.Locator("h2")).ToContainTextAsync("Public Timeline");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Account" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("Manage your account");
    }

    [OneTimeTearDown]
    public async Task TearDownFactory()
    {
        await _factory.DisposeAsync();
    }
}
