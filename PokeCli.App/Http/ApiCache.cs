using System.Diagnostics.CodeAnalysis;

namespace PokeCli.App.Http;

public interface IApiCache
{
    public void Add(string key, string jsonString);
    public bool TryGet(string key, out string cachedJson);
    // TODO: Implement ReapLoop - ReapLoop should clear out the cache after a interval
}

public class ApiCache : IApiCache
{
    private readonly Dictionary<string, string> _cache = new(); // short hand for new Dictionary<string, string>();

    public void Add(string key, string jsonString)
    {
        _cache.Add(key, jsonString);
    }

    public bool TryGet(string key, out string cachedString)
    {
        var result = _cache.TryGetValue(key, out string? value);
        cachedString = value ?? string.Empty;
        
        return result;
    }
}