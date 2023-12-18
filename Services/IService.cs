namespace OtterGui.Services;

public interface IService
{ }

public interface IDataContainer : IService
{
    public long   Time       { get; }
    public long   Memory     { get; }
    public string Name       { get; }
    public int    TotalCount { get; }
}

public interface IAsyncService : IService
{
    public Task Awaiter { get; }
}

public interface IAsyncDataContainer : IDataContainer, IAsyncService
{ }
