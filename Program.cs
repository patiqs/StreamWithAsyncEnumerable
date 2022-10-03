using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (CancellationToken token) => new FooClass { Stream1 = DoWork(token), Stream2 = DoWork(token) });

app.Run();

static async IAsyncEnumerable<int> DoWork([EnumeratorCancellation] CancellationToken token)
{
    for (int i = 0; i < 30; i++)
    {
        yield return i;
        await Task.Delay(100, token);
    }

    SimulateSporadicFailure();

    void SimulateSporadicFailure()
    {
        if (DateTimeOffset.UtcNow.Second > 30)
            throw new Exception();
    }
}

class FooClass
{
    public string Before => "start";
    public IAsyncEnumerable<int> Stream1 { get; init; }
    public string Between => "middle";
    public IAsyncEnumerable<int> Stream2 { get; init; }
    public string After => "end";
}
