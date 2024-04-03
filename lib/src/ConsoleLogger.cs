namespace WTNS;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Data-To-Console implementation of the ILogger interface.
/// </summary>
public class ConsoleLogger : ILogger
{
    public static string ColorTestString =
        "[black:███][blue:███][cyan:███][darkblue:███][darkcyan:███][darkgray:███][darkgreen:███][DarkMagenta:███][darkred:███][darkyellow:███][gray:███][green:███][magenta:███][red:███][white:███][yellow:███]";

    /// <summary>
    /// Logs to the console using the color formatting syntax.
    /// </summary>
    /// <param name="message"></param>
    public void Log(string message)
    {
        // Pattern to match color segments like "[color:text]"
        Regex pattern = new Regex(@"\[([a-zA-Z]+):([^\]]+)\]");
        MatchCollection matches = pattern.Matches(message);
        int lastPosition = 0;

        foreach (Match match in matches)
        {
            // Output text before the color segment in default color
            Console.Write(message.Substring(lastPosition, match.Index - lastPosition));

            // Try to parse the color
            if (Enum.TryParse(match.Groups[1].Value, true, out ConsoleColor color))
            {
                // Set the color and output the text segment
                Console.ForegroundColor = color;
                Console.Write(match.Groups[2].Value);
                Console.ResetColor();
            }
            else
            {
                // If color parsing fails, output the original segment
                Console.Write(match.Value);
            }

            lastPosition = match.Index + match.Length;
        }

        // Output any remaining text after the last match in default color
        if (lastPosition < message.Length)
        {
            Console.Write(message.Substring(lastPosition));
        }

        Console.WriteLine(); // Move to the next line after logging the message
    }
}
