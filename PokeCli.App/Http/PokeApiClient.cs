using System.Text.Json;

namespace PokeCli.App.Http;

public class PokeApiClient
{
  private readonly HttpClient _client;
  private readonly IApiCache _cache;

  public PokeApiClient(IApiCache cache)
  {
    _client = new HttpClient();
    _cache = cache;
  }

  public async Task<string> MakeGet(string url)
  {
    PokeAreaResponse serializedAreaResponse;
    
    // Try cache first
    var responseIsCached = _cache.TryGet(url, out string cachedResult);
    if (responseIsCached)
    {
      Console.WriteLine("Using cached value");
      // silenced_warning:: if we are here, we know we have a cached result
      return cachedResult;
    }
    
    var response = await _client.GetAsync(url);
    var requestFailed = !response.IsSuccessStatusCode;
    
    if (requestFailed)
    {
      throw new Exception("Unable to make request to " + url);
    }

    var stringResponse = await response.Content.ReadAsStringAsync();
    
    _cache.Add(url, stringResponse);
    return stringResponse;
  }
}

