namespace Wtns.Me.Lib;

/// <summary>
/// Data-To-File implementation of the ILogger interface.
/// </summary>
public class FileLogger : ILogger
{
    /// <summary>
    /// The directory to output the log files to.
    /// </summary>
    public string OutputDirectory { get; set; } = new("./logs/");

    /// <summary>
    /// Logs the message to a file.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        Directory.CreateDirectory(OutputDirectory);
        File.AppendAllText($"./logs/{timestamp}.txt", message + "\n");
    }
}
