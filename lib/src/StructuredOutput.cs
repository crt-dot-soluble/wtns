using System.Text;

namespace WTNS;

/// <summary>
/// Represents a structured output with sections that can be added, removed, and reordered.
/// By default the output has a single section called "Prefix".
/// The section order can be modified to change the order of the sections.
/// </summary>
public class StructuredOutput
{
    /// <summary>
    /// The output modes available to use within <see cref="StructuredOutput"/>
    /// </summary>
    public enum OutputMode
    {
        /// <summary>
        /// DEBUG prints a message exclusively to the debugger.
        /// </summary>
        DEBUG = 0,

        /// <summary>
        /// INFO prints an info message.
        /// </summary>
        INFO = 1,

        /// <summary>
        /// WARNING prints a warning message.
        /// </summary>
        WARNING = 2,

        /// <summary>
        /// ERROR prints an error message.
        /// </summary>
        ERROR = 3,

        /// <summary>
        /// AI prints AI chat-completions.
        /// </summary>
        ///
        AI = 4,

        /// <summary>
        /// Prints to the attached debugger and to the console.
        /// </summary>
        DEBUG_MIRRORED = 5,
    }

    private List<string> sectionOrder;
    private Dictionary<string, string> sections;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredOutput"/> class.
    /// </summary>
    public StructuredOutput()
    {
        sectionOrder = new List<string>();
        sections = new Dictionary<string, string>();

        AddSection("Prefix", "</>");
        // AddSection("InputPrompt", "<?>");
    }

    /// <summary>
    /// Adds a new section with the specified name and content.
    /// If a section with the same name exists, updates its content.
    /// </summary>
    /// <param name="sectionName">The name of the section to add or update.</param>
    /// <param name="sectionContent">The content of the section.</param>
    public void AddSection(string sectionName, string sectionContent)
    {
        if (!sections.ContainsKey(sectionName))
        {
            sections.Add(sectionName, sectionContent);
            sectionOrder.Add(sectionName);
        }
        else
        {
            Console.WriteLine($"Section '{sectionName}' already exists. Updating content...");
            sections[sectionName] = sectionContent;
        }
    }

    /// <summary>
    /// Removes the section with the specified name.
    /// </summary>
    /// <param name="sectionName">The name of the section to remove.</param>
    public void RemoveSection(string sectionName)
    {
        if (sections.ContainsKey(sectionName))
        {
            sections.Remove(sectionName);
            sectionOrder.Remove(sectionName);
        }
        else
        {
            Console.WriteLine($"Section '{sectionName}' does not exist.");
        }
    }

    /// <summary>
    /// Sets the content of the section with the specified name.
    /// If the section does not exist, does nothing.
    /// </summary>
    /// <param name="sectionName">The name of the section to update.</param>
    /// <param name="sectionContent">The new content for the section.</param>
    public void SetSectionContent(string sectionName, string sectionContent)
    {
        if (sections.ContainsKey(sectionName))
        {
            sections[sectionName] = sectionContent;
        }
        else
        {
            Console.WriteLine($"Section '{sectionName}' does not exist.");
        }
    }

    /// <summary>
    /// Reorders the section with the specified name to the new index in the output.
    /// If the section does not exist or the index is out of range, does nothing.
    /// </summary>
    /// <param name="sectionName">The name of the section to reorder.</param>
    /// <param name="newIndex">The new index for the section.</param>
    public void ReorderSection(string sectionName, int newIndex)
    {
        if (sections.ContainsKey(sectionName))
        {
            if (newIndex >= 0 && newIndex < sectionOrder.Count)
            {
                sectionOrder.Remove(sectionName);
                sectionOrder.Insert(newIndex, sectionName);
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
        }
        else
        {
            Console.WriteLine($"Section '{sectionName}' does not exist.");
        }
    }

    /// <summary>
    /// Displays the structured output string.
    /// The sections are concatenated in the order specified by <see cref="sectionOrder"/>.
    /// </summary>
    /// <param name="text">Additional text to append to the output string.</param>
    /// <param name="input">Whether or not to capture input.</param>
    /// <param name="mode">The <see cref="OutputMode"/> to use. (Color and format)</param>
    /// <returns>The captured input if <paramref name="input"/> is true, otherwise null.</returns>
    public string? DisplayOutputString(
        string text = "",
        bool input = false,
        OutputMode mode = OutputMode.INFO
    )
    {
        StringBuilder output = new StringBuilder();
        foreach (var sectionName in sectionOrder)
        {
            if (sections.ContainsKey(sectionName))
            {
                output.Append(sections[sectionName]);
            }
        }

        // Set appropriate color based on the mode
        ConsoleColor color = GetColorForMode(mode);

        // Prepare output string
        string outputString = output.ToString();

        if (!string.IsNullOrWhiteSpace(text))
        {
            outputString += $" {text}";
        }

        outputString += input ? " → " : " ";

        // Display output
        Console.ForegroundColor = color;
        Console.Write(outputString);
        Console.ForegroundColor = ConsoleColor.White;

        // If input is true, prompt the user for input and return the input
        if (input)
        {
            return Console.ReadLine();
        }

        Console.WriteLine();
        return null;
    }

    /// <summary>
    /// Displays the structured output string.
    /// The sections are concatenated in the order specified by <see cref="sectionOrder"/>.
    /// </summary>
    /// <param name="lines">Additional text to append to the output string.</param>
    /// <param name="input">Whether or not to capture input.</param>
    /// <param name="mode">The <see cref="OutputMode"/> to use. (Color and format)</param>
    /// <returns>The captured input if <paramref name="input"/> is true, otherwise null.</returns>
    public string? DisplayOutputString(
        string[] lines,
        bool input = false,
        OutputMode mode = OutputMode.INFO
    )
    {
        // Set appropriate color based on the mode
        ConsoleColor color = GetColorForMode(mode);

        // If lines is null or empty, return early (display nothing)
        if (lines == null || lines.Length == 0)
        {
            return DisplayOutputString();
        }
        // If lines has only one item, call the single line version of DisplayOutputString
        else if (lines.Length == 1)
        {
            return DisplayOutputString(lines[0], input, mode);
        }
        else
        {
            StringBuilder output = new StringBuilder();
            foreach (var sectionName in sectionOrder)
            {
                if (sections.ContainsKey(sectionName))
                {
                    output.Append(sections[sectionName]);
                }
            }

            // Prepare output string
            string outputString = $"{output.ToString()}";

            // Calculate the length of the prefix to align subsequent lines
            int prefixLength = CalculatePrefixLength(outputString);

            // Display output
            Console.ForegroundColor = color;
            for (int i = 0; i < lines.Length; i++)
            {
                // For the first line, include the full output string
                // For subsequent lines, calculate the padding to align with the first line
                string line =
                    i == 0
                        ? $"{outputString} {lines[i]}"
                        : $"{new string(' ', prefixLength)}{lines[i]}";

                // If it's the last line and input is true, add the input prompt
                if (i == lines.Length - 1 && input)
                {
                    Console.Write($"{line} → ");
                    return Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(line);
                }
            }

            Console.ForegroundColor = ConsoleColor.White;

            // If input is true but we did not enter the loop (no lines to display), prompt for input
            if (input)
            {
                return Console.ReadLine();
            }

            return null;
        }
    }

    // Helper method to calculate the length of the prefix
    private int CalculatePrefixLength(string outputString)
    {
        // Default prefix length for </> tag
        int prefixLength = 4;

        // Adjust prefix length based on the actual length of outputString
        foreach (char c in outputString)
        {
            if (c == ' ')
            {
                prefixLength++;
            }
            else
            {
                break;
            }
        }

        return prefixLength;
    }

    /// <summary>
    /// Gets the console color for the given output mode.
    /// </summary>
    private ConsoleColor GetColorForMode(OutputMode mode)
    {
        return mode switch
        {
            OutputMode.DEBUG
            or OutputMode.DEBUG_MIRRORED
                => ConfigurationRegistry.Instance.CurrentConfiguration.DebugColor,
            OutputMode.INFO => ConfigurationRegistry.Instance.CurrentConfiguration.InfoColor,
            OutputMode.WARNING => ConfigurationRegistry.Instance.CurrentConfiguration.WarningColor,
            OutputMode.ERROR => ConfigurationRegistry.Instance.CurrentConfiguration.ErrorColor,
            _ => ConsoleColor.White,
        };
    }
}
