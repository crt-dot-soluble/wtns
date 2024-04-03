using System.Reflection.Metadata;
using Wtns.Me.Lib;
using Wtns.Me.Lib.Cli;
using Wtns.Me.Lib.Commands;
using Wtns.Me.Lib.Net;

namespace Wtns.Me.Cli;

/// <summary>
/// The CLI class (cli, Cli) reprents a base-level implmentation of the WTNS library accessible from a simple console/terminal/shell.
/// The CLI itself has the sole purpose of hooking into the WTNS library and contains minimal meaningful code.
/// This class and by extension the project serve as an example of the features within the WTNS library.
/// </summary>
public sealed class Wtns
{
    /// <summary>
    /// Asynchronous application entry.
    /// </summary>
    /// <param name="args">The user specified runtime arguments.</param>
    /// <returns></returns>
    public static async Task Main(string[] args)
    {
        // var wtnsCommand = new WtnsCommand();
        // var user = AuthorizationProvider.CreateUser("wtns", "wtns");
        // //Console.Write(user.UserName);
        // Console.ReadLine();

        // --- ACCOUNT LOGIN TEST
        // var user = AuthorizationProvider.Login("wtns", "wtns");
        // Console.WriteLine(user.userName);



        // var parser = new CommandLineBuilder(wtnsCommand).UseDefaults().Build();

        // parser.Invoke(args);

        // foreach (Option o in wtnsCommand.Options)
        // {
        //     Debug.Print("Option : " + o.Name);
        // }

        //Console.WriteLine(string.Join(' ', args));

        // --- COMMENT OUT THIS TO ESCAPE THE REPL
        // CommandRegistry.Instance.ExecuteCommand(args);

        // --- ADD THESE FOR SIMULATED HISTORY
        // CommandRegistry.Instance.ExecuteCommand(args);
        // CommandRegistry.Instance.ExecuteCommand(args);
        // CommandRegistry.Instance.ExecuteCommand(args);
        // CommandRegistry.Instance.ExecuteCommand(args);

        ConsoleLogger cl = new ConsoleLogger();
        FileLogger fl = new FileLogger();
        fl.Log("Hello");
        cl.Log(ConsoleLogger.ColorTestString);

        var profiler = ProfilerFactory.CreateProfiler();
        var cpuinfo = profiler.GetProcessorInfo();

        cl.Log($"Address Width: {cpuinfo.AddressWidth?[0]}");
        cl.Log($"Architecture: {cpuinfo.Architecture?[0]}");
        cl.Log($"Architecture Friendly Name: {cpuinfo.ArchitectureFriendlyValue?[0]}");
        cl.Log($"Asset Tag: {cpuinfo.AssetTag?[0]}");
        cl.Log($"Availability: {cpuinfo.Availability?[0]}");
        cl.Log($"Avilability Friendly Name: {cpuinfo.AvailabilityFriendlyValue?[0]}");

        cl.Log("");

        cl.Log($"Characteristics: {cpuinfo.Characteristics?[0]}");
        cl.Log($"Characteristics Length: {cpuinfo.CharacteristicsFriendlyValue?[0].Count}");
        if (cpuinfo.CharacteristicsFriendlyValue?[0] != null)
        {
            foreach (string s in cpuinfo.CharacteristicsFriendlyValue?[0])
            {
                cl.Log($"Characteristics Friendly Name: {s}");
            }
        }

        cl.Log("");

        cl.Log($"ConfigManagerErrorCode: {cpuinfo.ConfigManagerErrorCode?[0]}");
        cl.Log($"ConfigManagerErrorCode Count: {cpuinfo.ConfigManagerErrorCode?.Count}");
        if (cpuinfo.ConfigManagerErrorCodeFriendlyValue?[0] != null)
        {
            foreach (string? s in cpuinfo.ConfigManagerErrorCodeFriendlyValue?[0])
            {
                cl.Log($"Config Manager Error Code Friendly Name: {s}");
            }
        }

        cl.Log("");

        cl.Log($"ConfigManagerUserConfig: {cpuinfo.ConfigManagerUserConfig?[0]}");
        cl.Log($"CpuStatus: {cpuinfo.CpuStatus?[0]}");
        cl.Log($"CpuStatus Friendly Name: {cpuinfo.CpuStatusFriendlyValue?[0]}");
        cl.Log($"Creation ClassName: {cpuinfo.CreationClassName?[0]}");
        cl.Log($"CurrentClockSpeed: {cpuinfo.CurrentClockSpeed?[0]}");
        cl.Log($"Current Voltage: {cpuinfo.CurrentVoltage?[0]}");
        cl.Log($"Current Voltage Friendly Value: {cpuinfo.CurrentVoltageFriendlyValue?[0]}");
        cl.Log($"DataWidth: {cpuinfo.DataWidth?[0]}");
        cl.Log($"ErrorCleared: {cpuinfo.ErrorCleared?[0]}");
        cl.Log($"ErrorDescription: {cpuinfo.ErrorDescription?[0]}");

        cl.Log("");

        cl.Log($"ExtClock: {cpuinfo.ExtClock?[0]}Mhz");
        cl.Log($"Family: {cpuinfo.Family?[0]}");
        cl.Log($"Family Friendly Name: {cpuinfo.FamilyFriendlyValue?[0]}");
        cl.Log($"InstallDate: {cpuinfo.InstallDate?[0]}");

        Console.ReadLine();
        // --- COMMENT THIS OUT TO STOP DISPLAYING THE ARGUMENTS AS OUTPUT
        Repl.Log($"Args: {string.Join(' ', args)}", false, StructuredOutput.OutputMode.DEBUG);
        CommandRegistry.Instance.SaveHistory("./history.json");
    }
}
