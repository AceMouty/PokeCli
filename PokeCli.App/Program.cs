using PokeCli.App;
using PokeCli.App.Http;

var shouldExit = false;
var paginationConfig = new PaginationConfig
{
  Next = "https://pokeapi.co/api/v2/location-area?offset=0&limit=20"
};

var pokeCache = new ApiCache();
var pokeClient = new PokeApiClient(pokeCache);

do
{
  Console.Write("pokidex > ");
  string userInput = Console.ReadLine() ?? "";
  userInput = userInput.ToLower();

  if (userInput == Commands.Help)
  {
    ConsoleMessages.Help();
  }

  if (userInput == Commands.Exit)
  {
    shouldExit = true;
  }

  if (userInput == Commands.Map)
  {
    if (string.IsNullOrEmpty(paginationConfig.Next))
    {
      Console.WriteLine("Nothing to show, try using 'mapb' instead");
    }
    
    var response = await pokeClient.MakeGet(paginationConfig.Next);
    paginationConfig.Next = response.next;
    paginationConfig.Prev = response.previous ?? string.Empty;
    
    foreach (var location in response.results)
    {
      Console.WriteLine(location.name);
    }
  }

  if (userInput == Commands.Mapb)
  {
    // will be null if we are on the first page of results
    if (string.IsNullOrEmpty(paginationConfig.Prev))
    {
      Console.WriteLine("Nothing to show, try using 'map' instead");
      continue;
    }
    
    
    var response = await pokeClient.MakeGet(paginationConfig.Prev);
    paginationConfig.Next = response.next;
    paginationConfig.Prev = response.previous ?? String.Empty;

    foreach (var location in response.results)
    {
      Console.WriteLine(location.name);
    }
  }

} while (shouldExit == false);

public readonly struct Commands
{
  public const string Help = "help";
  public const string Exit = "exit";
  public const string Map = "map";
  public const string Mapb = "mapb";
}

// has to be a class, when its a struct
// values do not update between method calls
public class PaginationConfig
{
  public string Next { get; set; } = string.Empty;
  public string Prev { get; set; } = string.Empty;

}