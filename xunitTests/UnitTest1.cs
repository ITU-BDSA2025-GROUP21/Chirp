using ChirpProject.MainApp.CheepClass;
namespace xunitTests;

public class CheepTests
{
    [Fact]
    public void Test1()
    {
        //Note at what time the cheep is created and remove Ms as to not cause any issues while still being precise.
        DateTimeOffset time = DateTimeOffset.Now;
        DateTimeOffset withoutMs = time.AddTicks(-(time.Ticks % TimeSpan.TicksPerSecond));

        //Create new cheep with an overloaded constructor to force the noted timestamp.
        Cheep testCheep = new("testing", time);

        DateTimeOffset cheepTime = DateTimeOffset.FromUnixTimeSeconds(testCheep.Timestamp);
        DateTimeOffset noMs = cheepTime.AddTicks(-(cheepTime.Ticks % TimeSpan.TicksPerSecond));

        //Assertain that all sent information is still the same
        Assert.True(
            testCheep.Message.Equals("testing") &&
            testCheep.Author.Equals(Environment.UserName) &&
            (noMs == withoutMs)
            );
    }

    public void Test2()
    {
        Assert.True(true);
    }
}
