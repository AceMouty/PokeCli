namespace PokeCli.App;

public static class ConsoleMessages
{
  public static void Help()
  {
    Console.Write("\n\n");
    Console.WriteLine("Welcome to the Pokidex!");
    Console.WriteLine("Usage:");
    Console.Write("\n\n");

    Console.WriteLine("command: help\ndescriptoin: Displays a help message\n");
    Console.WriteLine("command: exit\ndescriptoin: closes the app\n");
    Console.WriteLine("command: map\ndescription: show a list of areas\n");
    Console.WriteLine("command: mapb\ndescription: show previous list of areas\n");
  }
}
