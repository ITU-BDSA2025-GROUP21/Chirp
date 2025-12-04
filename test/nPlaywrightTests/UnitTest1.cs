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
        _factory.CreateClient();
    }

    [Test]
    public async Task ChirpWebsiteExists()
    {

    }

    [OneTimeTearDown]
    public void TearDownFactory()
    {
        _factory.Dispose();
    }

}
