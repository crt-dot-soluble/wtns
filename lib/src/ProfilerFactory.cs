using System.Runtime.InteropServices;

namespace Wtns.Me.Lib;

/// <summary>
/// Provides access to a factory method to create a profiler based on the current operating system.
/// </summary>
public static class ProfilerFactory
{
    /// <summary>
    /// <para>
    /// Creates and returns a <see cref="IProfiler"/> instance based on the current operating system.
    /// </para>
    /// </summary>
    /// <returns>The correct <see cref="IProfiler"/> based on the current operating system.</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IProfiler CreateProfiler()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsProfiler();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            //return new LinuxProfiler();
            return new WindowsProfiler();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            //return new MacProfiler();
            return new WindowsProfiler();
        }
        else
        {
            throw new NotSupportedException("Unsupported operating system.");
        }
    }
}
