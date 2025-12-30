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

    [Test]
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

    [Test]
    public async Task likeButton()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync("philip");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("phqu@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("jeg gider ikke mere");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToMatchAriaSnapshotAsync("- button \"(0)\":\n  - img\n  - text: \"\"");
        await Page.GetByRole(AriaRole.Button, new() { Name = "(0)" }).First.ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToMatchAriaSnapshotAsync("- button \"(1)\":\n  - img\n  - text: \"\"");
    }

    [Test]
    public async Task timelineCHange()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("phqu@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync("- heading \"Public Timeline\" [level=2]\n- heading \"What's on your mind philip?\" [level=3]\n- textbox\n- button \"Share\"\n- list:\n  - listitem:\n    - img \"philip\"\n    - link \"philip\":\n      - /url: /9e0bcf33-f867-475a-a1cc-fd10440d9d7b\n    - text: /⭐ \\d+ jeg gider ikke mere — \\d+-\\d+-\\d+ \\d+:\\d+/\n    - button \"(1)\":\n      - img\n      - text: \"\"\n    - button \"(0)\":\n      - img\n      - text: \"\"");
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync("- heading \"philip's Timeline\" [level=2]\n- list:\n  - listitem:\n    - img \"philip\"\n    - link \"philip\":\n      - /url: /9e0bcf33-f867-475a-a1cc-fd10440d9d7b\n    - text: /⭐ \\d+ jeg gider ikke mere — \\d+-\\d+-\\d+ \\d+:\\d+/\n    - button \"(1)\":\n      - img\n      - text: \"\"\n    - button \"(0)\":\n      - img\n      - text: \"\"");
    }

    [Test]
    public async Task dislikeButton()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("phqu@itu.dk");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Dinmor2610!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "(0)" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToMatchAriaSnapshotAsync("- button \"(1)\":\n  - img\n  - text: \"\"");
    }

    [OneTimeTearDown]
    public async Task TearDownFactory()
    {
        await _factory.DisposeAsync();
    }
}
