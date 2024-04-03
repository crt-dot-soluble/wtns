using WTNS.Commands;
using static WTNS.StructuredOutput;

namespace WTNS.Cli;

/// <summary>
/// REPL (Read, Evaluate, Print, and Loop) service for the WTNS CLI.
/// Allows for an interactive experience, rather than the default single execution style. (-e, --execute)
///
/// Follows singleton pattern to avoid multiple instantiations and other conflicts.
/// </summary>
public sealed class Repl
{
    private static readonly Lazy<Repl> lazy = new Lazy<Repl>(() => new Repl());

    /// <summary>
    /// The REPL instance, initialized only whe  called (lazy).
    /// </summary>
    public static Repl Instance => lazy.Value;

    /// <summary>
    /// The <see cref="Configuration"/> to use fo  the REPL.
    /// If a custom configuration object has not been supplied, the defaults are used.
    /// </summary>
    public Configuration Replconfiguration;

    /// <summary>
    /// The output structure to use. Allows for the manipulation and ordering/reording of the output segments.
    /// </summary>
    public StructuredOutput ReplStructuredOutput;

    /// <summary>
    /// Constructor for the REPL.
    /// </summary>
    /// <param name="replStructuredOutput">The output structure to use when outputting data.</param>
    private Repl(StructuredOutput? replStructuredOutput = null)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Replconfiguration = ConfigurationRegistry.Instance.CurrentConfiguration;
        ReplStructuredOutput =
            (replStructuredOutput == null)
                ? ReplStructuredOutput = new StructuredOutput()
                : replStructuredOutput;
    }

    /// <summary>
    /// Begins listening for the submission of a string of text (command) from the user.
    /// </summary>
    public void Listen()
    {
        while (true)
        {
            var input = ReplStructuredOutput.DisplayOutputString("", true);
            // _ = Log(input ?? string.Empty);
            CommandRegistry.Instance.ExecuteCommand(input);
        }
    }

    /// <summary>
    /// Provides a more convienient way to access <see cref="StructuredOutput.DisplayOutputString"/>.
    /// </summary>
    /// <param name="text">Additional text to append to the output string.</param>
    /// <param name="input">Whether or not to capture input.</param>
    /// <param name="mode">The <see cref="OutputMode"/> to use. (Color and format)</param>
    /// <returns></returns>
    public static string Log(string text, bool input = false, OutputMode mode = OutputMode.INFO)
    {
        return Instance.ReplStructuredOutput.DisplayOutputString(text, input, mode) ?? string.Empty;
    }

    /// <summary>
    /// Provides a more convienient way to access <see cref="StructuredOutput.DisplayOutputString"/>
    /// </summary>
    /// <param name="lines">Additional text to append to the output string.</param>
    /// <param name="input">Whether or not to capture input.</param>
    /// <param name="mode">The <see cref="OutputMode"/> to use. (Color and format)</param>
    /// <returns></returns>
    public static string Log(string[] lines, bool input = false, OutputMode mode = OutputMode.INFO)
    {
        return Instance.ReplStructuredOutput.DisplayOutputString(lines, input, mode)
            ?? string.Empty;
    }
}
