using System.Text.Json;
using PokeCli.App;
using PokeCli.App.Http;

var shouldExit = false;
var paginationConfig = new PaginationConfig
{
  Next = "https://pokeapi.co/api/v2/location-area?offset=0&limit=20"
};

var pokeCache = new ApiCache();
var pokeClient = new PokeApiClient(pokeCache);

string command = string.Empty;
string searchArea = String.Empty;
string lastMappedArea = string.Empty;

// TODO: Its a mess I know...but it works
do
{
  Console.Write("pokidex > ");
  string userInput = Console.ReadLine() ?? "";
  string[] splitInput = userInput.ToLower().Split(" ");
  command = splitInput[0];
  
  if (splitInput.Length > 1)
  {
    searchArea = splitInput[1];
  }

  if (command == Commands.Help)
  {
    ConsoleMessages.Help();
  }

  if (command == Commands.Exit)
  {
    shouldExit = true;
  }

  if (command == Commands.Map)
  {
    if (string.IsNullOrEmpty(paginationConfig.Next))
    {
      Console.WriteLine("Nothing to show, try using 'mapb' instead");
    }
    
    var response = await pokeClient.MakeGet(paginationConfig.Next);
    var parsedResponse = JsonSerializer.Deserialize<PokeAreaResponse>(response);
    
    lastMappedArea = paginationConfig.Next;
    paginationConfig.Next = parsedResponse.next;
    paginationConfig.Prev = parsedResponse.previous ?? string.Empty;
    
    foreach (var location in parsedResponse.results)
    {
      Console.WriteLine($"{location.name} {location.url}");
    }
  }

  if (command == Commands.Mapb)
  {
    // will be null if we are on the first page of results
    if (string.IsNullOrEmpty(paginationConfig.Prev))
    {
      Console.WriteLine("Nothing to show, try using 'map' instead");
      continue;
    }
    
    
    var response = await pokeClient.MakeGet(paginationConfig.Prev);
    var parsedResponse = JsonSerializer.Deserialize<PokeAreaResponse>(response);
    
    paginationConfig.Next = parsedResponse.next;
    paginationConfig.Prev = parsedResponse.previous ?? String.Empty;

    foreach (var location in parsedResponse.results)
    {
      Console.WriteLine(location.name);
    }
  }

  if (command == Commands.Explore)
  {
    // Prev will be empty string after first *map* run
    var url = lastMappedArea;
    
    var baseUrlIsNotCached = !pokeCache.TryGet(url, out string cachedResult);
    if (baseUrlIsNotCached)
    {
      throw new Exception($"Command.Explore:: url not chaced: {url}");
    }
    
    
    var serializedResponse = JsonSerializer.Deserialize<PokeAreaResponse>(cachedResult)!;
    var foundArea = serializedResponse.results.FirstOrDefault(areaObj => areaObj.name == searchArea);

    var areaNotCached = !pokeCache.TryGet(foundArea.name, out string cachedArea);
    
    if (areaNotCached)
    {
      url = foundArea.url;
      var searchResult = await pokeClient.MakeGet(url);
      pokeCache.Add(searchArea, searchResult);
    }
    
    // I hate this...but less than a if / else block XD
    pokeCache.TryGet(foundArea.name, out cachedArea);
    var areaDetails = JsonSerializer.Deserialize<PokeAreaDetails>(cachedArea);

    foreach (var details in areaDetails.pokemon_encounters)
    {
      Console.WriteLine(details.pokemon.name);
    }
  }

} while (shouldExit == false);

public readonly struct Commands
{
  public const string Help = "help";
  public const string Exit = "exit";
  public const string Map = "map";
  public const string Mapb = "mapb";
  public const string Explore = "explore";
}

// has to be a class, when its a struct
// values do not update between method calls
public class PaginationConfig
{
  public string Next { get; set; } = string.Empty;
  public string Prev { get; set; } = string.Empty;

}