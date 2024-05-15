using WTNS.Cli;
using WTNS.Telemetry;

namespace WTNS.Me.Cli;

/// <summary>
/// The CLI class (cli, Cli) reprents a base-level implmentation of the WTNS library accessible from a simple console/terminal/shell.
/// The CLI itself has the sole purpose of hooking into the WTNS library and contains minimal meaningful code.
/// This class and by extension the project serve as an example of the features within the WTNS library.
/// </summary>
public sealed class WTNS
{
    /// <summary>
    /// Asynchronous application entry.
    /// </summary>
    /// <param name="args">The user specified runtime arguments.</param>
    /// <returns></returns>
    public static void Main(string[] args)
    {
        // var WTNSCommand = new WTNSCommand();
        // var user = AuthorizationProvider.CreateUser("WTNS", "WTNS");
        // //Console.Write(user.UserName);
        // Console.ReadLine();

        // --- ACCOUNT LOGIN TEST
        // var user = AuthorizationProvider.Login("WTNS", "WTNS");
        // Console.WriteLine(user.userName);



        // var parser = new CommandLineBuilder(WTNSCommand).UseDefaults().Build();

        // parser.Invoke(args);

        // foreach (Option o in WTNSCommand.Options)
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
        var memoryinfo = profiler.GetMemoryInfo();
        var storageinfo = profiler.GetStorageInfo();

        Console.ForegroundColor = ConsoleColor.White;

        cl.Log($"Address Width: {cpuinfo.AddressWidth?[0]}");
        cl.Log($"Architecture: {cpuinfo.Architecture?[0]}");
        cl.Log($"Architecture Friendly Name: {cpuinfo.ArchitectureFriendlyValue?[0]}");
        cl.Log($"Asset Tag: {cpuinfo.AssetTag?[0]}");
        cl.Log($"Availability: {cpuinfo.Availability?[0]}");
        cl.Log($"Avilability Friendly Name: {cpuinfo.AvailabilityFriendlyValue?[0]}");
        cl.Log($"Characteristics: {cpuinfo.Characteristics?[0]}");
        cl.Log(
            $"Characteristics Length: {(cpuinfo.CharacteristicsFriendlyValue?[0] != null ? cpuinfo.CharacteristicsFriendlyValue?[0] : null)}"
        );

        if (cpuinfo.CharacteristicsFriendlyValue?[0] != null)
        {
            foreach (string s in cpuinfo.CharacteristicsFriendlyValue[0]!)
            {
                cl.Log($"Characteristics Friendly Value:: {s}");
            }
        }

        cl.Log($"ConfigManagerErrorCode: {cpuinfo.ConfigManagerErrorCode?[0]}");
        cl.Log($"ConfigManagerErrorCode Count: {cpuinfo.ConfigManagerErrorCode?.Count}");
        cl.Log(
            $"ConfigManagerErrorCodeFriendlyValue: {cpuinfo.ConfigManagerErrorCodeFriendlyValue?[0]}"
        );

        cl.Log($"ConfigManagerUserConfig: {cpuinfo.ConfigManagerUserConfig?[0]}");
        cl.Log($"CpuStatus: {cpuinfo.CpuStatus?[0]}");
        cl.Log($"CpuStatus Friendly Name: {cpuinfo.CpuStatusFriendlyValue?[0]}");
        cl.Log($"Creation ClassName: {cpuinfo.CreationClassName?[0]}");
        cl.Log($"CurrentClockSpeed: {cpuinfo.CurrentClockSpeed?[0]}");
        cl.Log($"Current Voltage: {cpuinfo.CurrentVoltage?[0]}");
        cl.Log($"Current Voltage Friendly Value: {cpuinfo.CurrentVoltageFriendlyValue?[0]}");
        cl.Log($"DataWidth: {cpuinfo.DataWidth?[0]}");
        cl.Log($"Description: {cpuinfo.Description?[0]}");
        cl.Log($"ErrorCleared: {cpuinfo.ErrorCleared?[0]}");
        cl.Log($"ErrorDescription: {cpuinfo.ErrorDescription?[0]}");
        cl.Log($"ExtClock: {cpuinfo.ExtClock?[0]}Mhz");
        cl.Log($"Family: {cpuinfo.Family?[0]}");
        cl.Log($"Family Friendly Name: {cpuinfo.FamilyFriendlyValue?[0]}");
        cl.Log($"InstallDate: {cpuinfo.InstallDate?[0]}");
        cl.Log($"Name: {cpuinfo.Name?[0]}");
        cl.Log($"NumberOfCores: {cpuinfo.NumberOfCores?[0]}");
        cl.Log($"NumberOfEnabledCore: {cpuinfo.NumberOfEnabledCore?[0]}");
        cl.Log($"NumberOfLogicalProcessors: {cpuinfo.NumberOfLogicalProcessors?[0]}");
        cl.Log($"PartNumber: {cpuinfo.PartNumber?[0]}");
        cl.Log($"PowerManagementCapabilities: {cpuinfo.PowerManagementCapabilities?[0]}");
        cl.Log($"PowerManagementCapabilities Count: {cpuinfo.PowerManagementCapabilities?.Count}");

        if (
            cpuinfo.PowerManagementCapabilities?[0] != null
            && cpuinfo.PowerManagementCapabilitiesFriendlyValue?[0] != null
        )
        {
            foreach (string? s in cpuinfo.PowerManagementCapabilitiesFriendlyValue[0]!)
            {
                cl.Log($"PowerManagementCapabilities Friendly Value: {s}");
            }
        }

        cl.Log($"PowerManagementSupported: {cpuinfo.PowerManagementSupported?[0]}");
        cl.Log($"ProcessorId: {cpuinfo.ProcessorID?[0]}");
        cl.Log($"ProcessorType: {cpuinfo.ProcessorType?[0]}");
        cl.Log($"ProcessorTypeFriendlyValue: {cpuinfo.ProcessorTypeFriendlyValue?[0]}");
        cl.Log($"Revision: {cpuinfo.Revision?[0]}");
        cl.Log($"Role: {cpuinfo.Role?[0]}");
        cl.Log($"SocketDesignation: {cpuinfo.SocketDesignation?[0]}");
        cl.Log($"Status: {cpuinfo.Status?[0]}");
        cl.Log($"StatusInfo: {cpuinfo.StatusInfo?[0]}");
        cl.Log($"StatusInfoFriendlyValue: {cpuinfo.StatusInfoFriendlyValue?[0]}");
        cl.Log($"SystemCreationClassName: {cpuinfo.SystemCreationClassName?[0]}");
        cl.Log($"SystemName: {cpuinfo.SystemName?[0]}");
        cl.Log($"ThreadCount: {cpuinfo.ThreadCount?[0]}");
        cl.Log($"UniqueId: {cpuinfo.UniqueID?[0]}");

        Console.ForegroundColor = ConsoleColor.Cyan;
        cl.Log("");
        cl.Log($"Memory Attributes: {memoryinfo.Attributes?[0]}");
        cl.Log($"Memory BankLabel: {memoryinfo.BankLabel?[0]}");
        cl.Log($"Memory Capacity: {memoryinfo.Capacity?[0]}");
        cl.Log($"Memory ConfiguredClockSpeed: {memoryinfo.ConfiguredClockSpeed?[0]}");
        cl.Log($"Memory ConfiguredVoltage: {memoryinfo.ConfiguredVoltage?[0]}");
        cl.Log($"Memory CreationClassName: {memoryinfo.CreationClassName?[0]}");
        cl.Log($"Memory DataWidth: {memoryinfo.DataWidth?[0]}");
        cl.Log($"Memory Description: {memoryinfo.Description?[0]}");
        cl.Log($"Memory DeviceLocator: {memoryinfo.DeviceLocator?[0]}");
        cl.Log($"Memory FormFactor: {memoryinfo.FormFactor?[0]}");
        cl.Log($"Memory HotSwappable: {memoryinfo.HotSwappable?[0]}");
        cl.Log($"Memory InstallDate: {memoryinfo.InstallDate?[0]}");
        cl.Log($"Memory InterleaveDepth: {memoryinfo.InterleaveDataDepth?[0]}");
        cl.Log($"Memory InterleaveDataDepth: {memoryinfo.InterleaveDataDepth?[0]}");

        cl.Log("");

        cl.Log($"Storage Availability: {storageinfo.Availability?[0]}");
        cl.Log($"Storage BytesPerSector: {storageinfo.BytesPerSector?[0]}");

        Console.ReadLine();

        // --- COMMENT THIS OUT TO STOP DISPLAYING THE ARGUMENTS AS OUTPUT
        Repl.Log($"Args: {string.Join(' ', args)}", false, StructuredOutput.OutputMode.DEBUG);

        // CommandRegistry.Instance.SaveHistory("./history.json");
    }
}
