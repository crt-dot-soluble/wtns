namespace Wtns.Me.Lib;

/// <summary>
/// Contains all application preferences.
/// </summary>
public struct Configuration
{
    public Configuration() { }

    /// <summary>
    /// The name of the Configuration.
    /// </summary>
    public string Name { get; set; } = "default";

    /// <summary>
    /// The secret phrase used for certain functions such as hashing.
    /// </summary>
    public string Pepper { get; set; } = "wtns";

    /// <summary>
    /// The color of the console foreground.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

    /// <summary>
    /// The color of the console background.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

    /// <summary>
    /// The color of DEBUG output.
    /// </summary>
    public ConsoleColor DebugColor { get; set; } = ConsoleColor.Cyan;

    /// <summary>
    /// The color of INFO output.
    /// </summary>
    public ConsoleColor InfoColor { get; set; } = ConsoleColor.White;

    /// <summary>
    /// The color of WARNING output.
    /// </summary>
    public ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;

    /// <summary>
    /// The color of ERROR output.
    /// </summary>
    public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;

    /// <summary>
    /// The color of AI output.
    /// </summary>
    public ConsoleColor AiColor { get; set; } = ConsoleColor.Green;

    /// <summary>
    /// The icon to use for DEBUG output. (typically an emoji or single character)
    /// </summary>
    public string DebugIcon { get; set; } = "🟦";

    /// <summary>
    /// The icon to use for INFO output. (typically an emoji or single character)
    /// </summary>
    public string InfoIcon { get; set; } = "⬜";

    /// <summary>
    /// The icon to use for WARNING output. (typically an emoji or single character)
    /// </summary>
    public string WarningIcon { get; set; } = "🟨";

    /// <summary>
    /// The icon to use for ERROR output. (typically an emoji or single character)
    /// </summary>
    public string ErrorIcon { get; set; } = "🟥";

    /// <summary>
    /// The icon to use for AI output. (Typically an emoji or single character)
    /// </summary>
    public string AiIcon { get; set; } = "🟩";

    /// <summary>
    /// The OpenAI API key that will be used for any service that requires one.
    /// </summary>
    public string OpenAiApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Defines the structure (format) to use when outputting data.
    /// Allows for a custom appearance.
    /// </summary>
    public StructuredOutput? OutputFormat { get; set; } = new StructuredOutput();
}
