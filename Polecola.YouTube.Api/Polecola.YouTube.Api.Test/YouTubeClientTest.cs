using Polecola.Youtube.Api;

namespace Polecola.YouTube.Api.Test;

public class YouTubeClientTest
{
    private YouTubeClient _client;
    
    [SetUp]
    public void Setup()
    {
        _client = new YouTubeClient(new HttpClient());
    }

    [Test]
    public async Task VideoSearchTest()
    {
        var result = await _client.SearchVideoAsync("Te", retry: 6, page:3);
        Assert.That(result, Is.Not.Null);
    }
}