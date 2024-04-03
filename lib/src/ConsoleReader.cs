namespace WTNS;

public class ConsoleReader : IReader
{
    private static int previousWindowHeight;
    private static int previousWindowWidth;

    public string Prompt { get; set; } = "</>";

    public ConsoleReader()
    {
        previousWindowHeight = Console.WindowHeight;
        previousWindowWidth = Console.WindowWidth;
    }

    public string Read()
    {
        string? input = null;
        while (input == string.Empty || input == null)
        {
            input = Console.ReadLine();
        }
        return input;
    }

    static void HandleConsoleResize()
    {
        if (
            Console.WindowHeight != previousWindowHeight
            || Console.WindowWidth != previousWindowWidth
        )
        {
            // Clear to avoid artifacts due to resize
            Console.Clear();

            // Adjust for new size
            previousWindowHeight = Console.WindowHeight;
            previousWindowWidth = Console.WindowWidth;

            // You might re-display any persistent information or UI components here
        }
    }
}
