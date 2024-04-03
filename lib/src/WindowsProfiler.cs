using System.ComponentModel;
using System.Formats.Asn1;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Wtns.Me.Lib;

/*  Required Documentation & Resources
*
*       Mappings for Most Win32_Processor Properties:
*           https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor?redirectedfrom=MSDN
*
*       Processor Characteristics Mappings:
*           https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.6.0.pdf
*
*/


/*  Windows Management Instrumentation (WMI) Note
*
*   Avoid pattern matching - instead, use explicit conversion.
*
*   The pattern matching construct `is int` checks if the given object is of type int
*   and directly casts it to an integer if true. This approach can be problematic
*   with COM objects and properties fetched via WMI, due to the way they
*   handle types.
*
*   When accessing a property like MaxClockSpeed from a ManagementObject,
*   the value you get is not a pure .NET int type directly. Under the hood, WMI provides access to
*   data that comes from the COM world, where types are often wrapped in a VARIANT or similar,
*   making them more akin to an `object` in .NET. Even when the actual value is an integer by nature, it's
*   not directly exposed as such, which can lead pattern matching to fail because the type isn't
*   a native int but possibly object wrapping an int.
*
*   The solution is to explicitly convert the value to the desired .NET datatype using something
*   like System.Convert
*
*   Consider the following snippet of code:
*
*       using (
*           var searcher = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor")
*       )
*       {
*           foreach (var item in searcher.Get())
*           {
*               // Assuming the result always has the MaxClockSpeed property
*               // MaxClockSpeed is in MHz, convert to GHz
*               if (item["MaxClockSpeed"] is int maxClockSpeed)
*               {
*                   return Convert.ToInt32(maxClockSpeed) / 1000.0;
*               }
*               else
*               {
*                   return -1;
*               }
*           }
*       }
*
*   The above code will always return -1. However, we can see using the powershell script:
*
*       Get-WmiObject -Class Win32_Processor | Select-Object Name, MaxClockSpeed
*
*   that the MaxClockSpeed property is indeed a positive integer which accurately reflects the clock speed.
*
*/

/// <summary>
/// <para>
/// Windows implementation of the <see cref="IProfiler"/> interface.
/// </para>
/// <para>
/// Note: This class uses Windows specific code and supports only the operating system
/// versions that are directly supported by the framework it depends on. (in most cases, .NET)
/// </para>
/// <list type="bullet">
/// <item><description>Legacy hardware and operating systems may not be supported.</description></item>
/// <item><description>Obscure hardware and operating systems may not be supported.</description></item>
/// </list>
/// </summary>
public class WindowsProfiler : IProfiler
{
    /// <inheritdoc/>
    public CpuInfo GetProcessorInfo()
    {
        return new CpuInfo
        {
            AddressWidth = GetProcessorAddressWidth(),
            Architecture = GetProcessorArchitecture(),
            ArchitectureFriendlyValue = GetProcessorArchitectureFriendlyValue(),
            AssetTag = GetProcessorAssetTag(),
            Availability = GetProcessorAvailability(),
            AvailabilityFriendlyValue = GetProcessorAvailabilityFriendlyValue(),
            Characteristics = GetProcessorCharacteristics(),
            CharacteristicsFriendlyValue = GetProcessorCharacteristicsFriendlyValue(),
            ConfigManagerErrorCode = GetProcessorConfigManagerErrorCode(),
            ConfigManagerErrorCodeFriendlyValue = GetProcessorConfigManagerErrorCodeFriendlyValue(),
            ConfigManagerUserConfig = GetProcessorConfigManagerUserConfig(),
            CpuStatus = GetProcessorCpuStatus(),
            CpuStatusFriendlyValue = GetProcessorCpuStatusFriendlyValue(),
            CreationClassName = GetProcessorCreationClassName(),
            CurrentClockSpeed = GetProcessorCurrentClockSpeed(),
            CurrentVoltage = GetProcessorCurrentVoltage(),
            CurrentVoltageFriendlyValue = GetProcessorCurrentVoltageFriendlyValue(),
            DataWidth = GetProcessorDataWidth(),
            Description = GetProcessorDescription(),
            DeviceId = GetProcessorDeviceID(),
            ErrorCleared = GetProcessorErrorCleared(),
            ErrorDescription = GetProcessorErrorDescription(),
            ExtClock = GetProcessorExtClock(),
            Family = GetProcessorFamily(),
            FamilyFriendlyValue = GetProcessorFamilyFriendlyValue(),
            InstallDate = GetProcessorInstallDate(),
            L2CacheSize = GetProcessorL2CacheSize(),
            L2CacheSpeed = GetProcessorL2CacheSpeed(),
            L3CacheSize = GetProcessorL3CacheSize(),
            L3CacheSpeed = GetProcessorL3CacheSpeed()
            //LastErrorCode = GetProcessorLastErrorCode();
        };
    }

    /// <summary>
    /// Retrieves a property of the processor using WMI as a List where each element in the List corresponds to an individual piece of hardware.
    /// </summary>
    /// <typeparam name="T">The type of the property to retrieve.</typeparam>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <param name="converter">A function to convert the retrieved object to the desired type.</param>
    /// <returns>A list of values of the specified property, or null if an error occurs or the platform is not Windows.</returns>
    private List<T>? GetWmiObject<T>(string propertyName, Func<object?, T> converter)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var values = new List<T>();

        try
        {
            var query = $"SELECT {propertyName} FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var value = queryObj[propertyName];
                    var convertedValue = converter(value);
                    if (convertedValue != null)
                        values.Add(convertedValue);
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return values;
    }

    /// <inheritdoc/>
    public StorageInfo GetStorageInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public GpuInfo GetGpuInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public MotherboardInfo GetMotherboardInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public NetInfo GetNetInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public OsInfo GetOsInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public MemoryInfo GetRamInfo()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the address width of all processors in the system.
    /// </summary>s
    /// <returns>A list of ushort values representing the address widths of all processors, or null if an error occurs or the platform is not Windows.</returns>
    public List<ushort>? GetProcessorAddressWidth()
    {
        var list = GetWmiObject(
            "AddressWidth",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Gets the architecture of all processors in the system.
    /// </summary>
    /// <returns>A list of ushort values representing the architectures of all processors, or null if an error occurs or the platform is not Windows.</returns>
    public List<ushort>? GetProcessorArchitecture()
    {
        var list = GetWmiObject(
            "Architecture",
            arch => arch != null ? (ushort?)Convert.ToUInt16(arch) : null
        )
            ?.Where(arch => arch != null)
            .Select(arch => arch!.Value)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Gets the friendly names of the processor architectures for each processor.
    /// </summary>
    /// <returns>
    /// The list of friendly names of the processor architectures for each processor,
    /// or null if the method is not supported on the current platform or if no architectures are available.
    /// </returns>
    public List<string>? GetProcessorArchitectureFriendlyValue()
    {
        var list = GetProcessorArchitecture()
            ?.Select(MapProcessorArchitectureToFriendlyValue)
            .Where(arch => arch != null) // Filter out null values
            .Select(arch => arch!) // Assert non-null using null-forgiving operator
            .ToList(); // Convert to list

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps the
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string? MapProcessorArchitectureToFriendlyValue(ushort value)
    {
        switch (value)
        {
            case 0:
                return "x86";
            case 1:
                return "MIPS";
            case 2:
                return "Alpha";
            case 3:
                return "PowerPC";
            case 5:
                return "ARM";
            case 6:
                return "Itanium (ia64)";
            case 9:
                return "x64";
            case 12:
                return "ARM64";
            default:
                return null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public List<string>? GetProcessorAssetTag()
    {
        var list = GetWmiObject("AssetTag", value => Convert.ToString(value) ?? null)
            ?.Select(Convert.ToString) // Ensure non-nullable strings
            .Where(tag => tag != null)
            .Select(tag => tag!)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the availability of the processor.
    /// Availability indicates the availability and status of the processor.
    /// </summary>
    /// <returns>A list of ushort values representing the availability of each processor.</returns>
    public List<ushort>? GetProcessorAvailability()
    {
        var list = GetWmiObject(
            "Availability",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list != null & list.Count > 0 ? list : null;
    }

    public List<string>? GetProcessorAvailabilityFriendlyValue()
    {
        var list = GetProcessorAvailability()
            ?.Select(MapProcessorAvailabilityToFriendlyValue)
            .Where(value => value != null)
            .Select(value => value!)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps the availability code to its corresponding friendly name.
    /// </summary>
    /// <param name="value">The ushort value representing the availability code.</param>
    /// <returns>The friendly name of the availability, or null if the value is not recognized.</returns>
    private string? MapProcessorAvailabilityToFriendlyValue(ushort value)
    {
        switch (value)
        {
            case 1:
                return "Other";
            case 2:
                return "Unknown";
            case 3:
                return "Running/Full Power";
            case 4:
                return "Warning";
            case 5:
                return "In Test";
            case 6:
                return "Not Applicable";
            case 7:
                return "Power Off";
            case 8:
                return "Off Line";
            case 9:
                return "Off Duty";
            case 10:
                return "Degraded";
            case 11:
                return "Not Installed";
            case 12:
                return "Install Error";
            case 13:
                return "Power Save - Unknown";
            case 14:
                return "Power Save - Low Power Mode";
            case 15:
                return "Power Save - Standby";
            case 16:
                return "Power Cycle";
            case 17:
                return "Power Save - Warning";
            case 18:
                return "Paused";
            case 19:
                return "Not Ready";
            case 20:
                return "Not Configured";
            case 21:
                return "Quiesced";
            default:
                return null;
        }
    }

    /// <summary>
    /// Retrieves the characteristics of each processor.
    /// </summary>
    /// <returns>A list of uint32 values representing the characteristics of each processor, or null if the operation fails.</returns>
    public List<uint>? GetProcessorCharacteristics()
    {
        var list = GetWmiObject(
            "Characteristics",
            characteristics => (uint?)Convert.ToUInt32(characteristics)
        )
            ?.Where(characteristics => characteristics != null)
            .Select(characteristics => characteristics!.Value)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the characteristics of each processor and maps them to their corresponding friendly names.
    /// </summary>
    /// <returns>A list of lists of strings representing the friendly names of the characteristics for each processor.</returns>
    public List<List<string>?>? GetProcessorCharacteristicsFriendlyValue()
    {
        var list = GetProcessorCharacteristics()
            ?.Select(MapProcessorCharacteristicsToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// <para>
    /// Maps the binary representation of processor characteristics to their corresponding friendly names.
    /// </para>
    /// <para>
    /// Use <see href="https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.6.0.pdf"/> as Microsofts documentation does not contain the mappings for these values. />
    /// </para>
    /// </summary>
    /// <param name="characteristicValue">The uint32 representation of processor characteristics.</param>
    /// <returns>A list of strings representing the friendly names of the characteristics.</returns>
    private List<string>? MapProcessorCharacteristicsToFriendlyValue(uint characteristicValue)
    {
        var binaryString = Convert.ToString(characteristicValue, 2).PadLeft(32, '0');
        var characteristicNames = new List<string>();

        var mapping = new Dictionary<int, string>
        {
            { 0, "Reserved" },
            { 1, "Unknown" },
            { 2, "64-bit Capable" },
            { 3, "Multi-Core" },
            { 4, "Hardware Thread" },
            { 5, "Execute Protection" },
            { 6, "Enhanced Virtualization" },
            { 7, "Power/Performance Control" },
            { 8, "128-bit Capable" },
            { 9, "Arm64 SoC ID" },
            { 10, "Reserved" },
            { 11, "Reserved" },
            { 12, "Reserved" },
            { 13, "Reserved" },
            { 14, "Reserved" },
            { 15, "Reserved" }
        };

        for (int i = binaryString.Length - 1; i >= 0; i--)
        {
            if (binaryString[i] == '1' && mapping.ContainsKey(binaryString.Length - 1 - i))
            {
                characteristicNames.Add(mapping[binaryString.Length - 1 - i]);
            }
        }

        return characteristicNames.Count > 0 ? characteristicNames : null;
    }

    /// <summary>
    /// Retrieves the ConfigManagerErrorCode for each processor.
    /// </summary>
    /// <remarks>
    /// The ConfigManagerErrorCode represents the error code reported by the Configuration Manager.
    /// This method queries WMI to fetch the ConfigManagerErrorCode for every processor present in the system.
    /// </remarks>
    /// <returns>A list of uint values representing the ConfigManagerErrorCode of each processor,
    /// or null if unable to retrieve.</returns>
    public List<uint>? GetProcessorConfigManagerErrorCode()
    {
        var list = GetWmiObject(
            "ConfigManagerErrorCode",
            value =>
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    return (uint?)Convert.ToUInt32(value);
                }
            }
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves friendly names corresponding to the ConfigManagerErrorCode of each processor.
    /// </summary>
    /// <remarks>
    /// Utilizes the <see cref="GetProcessorConfigManagerErrorCode"/> method to fetch raw ConfigManagerErrorCode values
    /// and then maps them to user-friendly descriptions via <see cref="MapProcessorConfigManagerErrorCodeToFriendlyValue"/>.
    /// </remarks>
    /// <returns>A list of lists of strings representing the friendly names for the ConfigManagerErrorCode of each processor,
    /// or null if unable to retrieve.</returns>
    public List<List<string>>? GetProcessorConfigManagerErrorCodeFriendlyValue()
    {
        var list = GetProcessorConfigManagerErrorCode()
            ?.Select(MapProcessorConfigManagerErrorCodeToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list != null && list.Count > 0 ? list : null;

        // var errorCodes = GetProcessorConfigManagerErrorCode();
        // var FriendlyValuesList = new List<List<string>>();

        // if (errorCodes == null)
        // {
        //     return null;
        // }

        // foreach (var errorCode in errorCodes)
        // {
        //     var FriendlyValues = MapProcessorConfigManagerErrorCodeToFriendlyValue(errorCode);
        //     FriendlyValuesList.Add(FriendlyValues);
        // }

        // return FriendlyValuesList.Count > 0 ? FriendlyValuesList : null;
    }

    /// <summary>
    /// Maps a ConfigManagerErrorCode to its corresponding friendly name(s).
    /// </summary>
    /// <param name="errorCode">The uint value representing the ConfigManagerErrorCode.</param>
    /// <returns>A list of strings representing the friendly name(s) of the ConfigManagerErrorCode.</returns>
    private List<string> MapProcessorConfigManagerErrorCodeToFriendlyValue(uint errorCode)
    {
        var description = errorCode switch
        {
            0 => new List<string> { "This device is working properly." },
            1 => new List<string> { "Device is not configured correctly." },
            2 => new List<string> { "Windows cannot load the driver for this device." },
            3
                => new List<string>
                {
                    "The driver for this device might be corrupted, or your system may be running low on memory or other resources."
                },
            4
                => new List<string>
                {
                    "This device is not working properly. One of its drivers or your registry might be corrupted."
                },
            // Include all other case numbers according to the specified descriptions...
            31
                => new List<string>
                {
                    "Device is not working properly. Windows cannot load the required device drivers."
                },
            _ => new List<string> { "Unknown Error Code" }
        };

        return description;
    }

    /// <summary>
    /// Retrieves the ConfigManagerUserConfig value for each processor.
    /// </summary>
    /// <returns>A list of boolean values representing whether the device is using a user-defined configuration for each processor, or null if the operation fails. True indicates a user-defined configuration is in use.</returns>
    public List<bool>? GetProcessorConfigManagerUserConfig()
    {
        // Check if the current OS platform supports this method
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var userConfigs = new List<bool>();

        try
        {
            // WMI query to fetch the ConfigManagerUserConfig property
            var query = "SELECT ConfigManagerUserConfig FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                // Execute the query and process results
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var userConfig = queryObj["ConfigManagerUserConfig"];

                    if (userConfig != null)
                    {
                        // Convert and add the result to the list
                        userConfigs.Add(Convert.ToBoolean(userConfig));
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return userConfigs.Count > 0 ? userConfigs : null;
    }

    /// <summary>
    /// Retrieves the raw CpuStatus values for each processor on the system.
    /// </summary>
    /// <remarks>
    /// This method queries WMI to fetch the CpuStatus property for each processor, indicating its operational state.
    /// </remarks>
    /// <returns>A list containing the CpuStatus for each processor, or null if unable to retrieve.</returns>
    public List<ushort>? GetProcessorCpuStatus()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var cpuStatusList = new List<ushort>();

        try
        {
            var query = "SELECT CpuStatus FROM Win32_Processor";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var cpuStatus = queryObj["CpuStatus"];
                    if (
                        cpuStatus != null
                        && ushort.TryParse(cpuStatus.ToString(), out ushort cpuStatusCode)
                    )
                    {
                        cpuStatusList.Add(cpuStatusCode);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching CPU status: {ex.Message}");
        }

        return cpuStatusList.Count > 0 ? cpuStatusList : null;
    }

    /// <summary>
    /// Retrieves friendly names corresponding to the CpuStatus code of each processor.
    /// </summary>
    /// <remarks>
    /// Utilizes the <see cref="GetProcessorCpuStatus"/> method to fetch raw CpuStatus values and then
    /// maps them to user-friendly descriptions via <see cref="MapProcessorCpuStatusToFriendlyValue"/>.
    /// </remarks>
    /// <returns>A list of friendly names representing each processor's CpuStatus, or null if unable to retrieve.</returns>
    public List<string>? GetProcessorCpuStatusFriendlyValue()
    {
        var FriendlyValuesList = new List<string>();
        var cpuStatuses = GetProcessorCpuStatus();

        if (cpuStatuses == null)
        {
            return null;
        }

        foreach (var status in cpuStatuses)
        {
            var FriendlyValue = MapProcessorCpuStatusToFriendlyValue(status);
            if (FriendlyValue != null)
            {
                FriendlyValuesList.Add(FriendlyValue);
            }
        }

        return FriendlyValuesList.Count > 0 ? FriendlyValuesList : null;
    }

    /// <summary>
    /// Maps a numeric CpuStatus code to a human-readable description or friendly name.
    /// </summary>
    /// <param name="status">A ushort value representing the CpuStatus code.</param>
    /// <returns>
    /// The friendly name describing the CpuStatus. Returns null if the value is not recognized.
    /// </returns>
    private string? MapProcessorCpuStatusToFriendlyValue(ushort status)
    {
        var mapping = new Dictionary<ushort, string>
        {
            { 0, "Unknown" },
            { 1, "CPU Enabled" },
            { 2, "CPU Disabled by User through BIOS Setup" },
            { 3, "CPU Disabled By BIOS (POST Error)" },
            { 4, "CPU is Idle" },
            { 5, "Reserved" },
            { 6, "Reserved" },
            { 7, "Other" }
        };

        return mapping.TryGetValue(status, out string? FriendlyValue) ? FriendlyValue : null;
    }

    /// <summary>
    /// Retrieves the CreationClassName for each processor on the system.
    /// </summary>
    /// <remarks>
    /// CreationClassName provides the name of the first concrete class in the inheritance
    /// chain used to create an instance. This method fetches the CreationClassName for each
    /// processor to uniquely identify the processor class type within WMI.
    /// </remarks>
    /// <returns>A list of strings each representing the CreationClassName of a processor,
    /// or null if the operation fails.</returns>
    public List<string>? GetProcessorCreationClassName()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var creationClassNames = new List<string>();

        try
        {
            var query = "SELECT CreationClassName FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var creationClassName = queryObj["CreationClassName"].ToString();
                    if (!string.IsNullOrEmpty(creationClassName))
                    {
                        creationClassNames.Add(creationClassName);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching CreationClassNames: {ex.Message}");
        }

        return creationClassNames.Count > 0 ? creationClassNames : null;
    }

    /// <summary>
    /// Retrieves the CurrentClockSpeed for each processor.
    /// </summary>
    /// <remarks>
    /// The CurrentClockSpeed represents the current speed of the processor in MHz,
    /// as reported by the System Management BIOS (SMBIOS) Processor Information structure.
    /// This method queries WMI to fetch the CurrentClockSpeed for every processor present in the system.
    /// </remarks>
    /// <returns>A list of uint values representing the CurrentClockSpeed of each processor,
    /// or null if unable to retrieve.</returns>
    public List<uint>? GetProcessorCurrentClockSpeed()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var clockSpeeds = new List<uint>();

        try
        {
            var query = "SELECT CurrentClockSpeed FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var currentClockSpeed = queryObj["CurrentClockSpeed"];

                    if (
                        currentClockSpeed != null
                        && uint.TryParse(currentClockSpeed.ToString(), out uint clockSpeed)
                    )
                    {
                        clockSpeeds.Add(clockSpeed);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching CurrentClockSpeed: {ex.Message}");
        }

        return clockSpeeds.Count > 0 ? clockSpeeds : null;
    }

    /// <summary>
    /// Retrieves the CurrentVoltage for each processor, expressed in tenth-volts.
    /// </summary>
    /// <remarks>
    /// The CurrentVoltage represents the raw voltage of the processor in tenth-volts, directly fetched from WMI.
    /// If the eighth bit is set, bits 0-6 contain the voltage multiplied by 10.
    /// </remarks>
    /// <returns>A list of ushort values representing the CurrentVoltage of each processor in tenth-volts, or null if unable to retrieve.</returns>
    public List<ushort>? GetProcessorCurrentVoltage()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var voltages = new List<ushort>();

        try
        {
            var query = "SELECT CurrentVoltage FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var currentVoltage = queryObj["CurrentVoltage"];

                    if (
                        currentVoltage != null
                        && ushort.TryParse(currentVoltage.ToString(), out ushort voltageValue)
                    )
                    {
                        voltages.Add(voltageValue);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching CurrentVoltage: {ex.Message}");
        }

        return voltages.Count > 0 ? voltages : null;
    }

    /// <summary>
    /// Retrieves and converts the CurrentVoltage for each processor to a more user-friendly decimal format (volts).
    /// </summary>
    /// <remarks>
    /// This function takes the raw voltage values in tenth-volts, processes them, and presents them in volts.
    /// It considers the SMBIOS documentation for accurate interpretation, especially regarding the eighth bit logic.
    /// </remarks>
    /// <returns>A list of decimal values representing each processor's voltage in volts, or null if unable to process.</returns>
    public List<decimal>? GetProcessorCurrentVoltageFriendlyValue()
    {
        var rawVoltages = GetProcessorCurrentVoltage();
        var friendlyVoltages = new List<decimal>();

        if (rawVoltages == null)
        {
            return null;
        }

        foreach (var voltage in rawVoltages)
        {
            // Convert every value directly from tenth-volts to volts.
            decimal adjustedVoltage = voltage / 10M;
            friendlyVoltages.Add(adjustedVoltage);
        }

        return friendlyVoltages.Count > 0 ? friendlyVoltages : null;
    }

    /// <summary>
    /// Retrieves the DataWidth for each processor.
    /// </summary>
    /// <remarks>
    /// The DataWidth indicates the architecture width of the processor in bits. For a 32-bit processor,
    /// the value is 32, and for a 64-bit processor, it is 64. This method queries WMI to fetch the
    /// DataWidth for every processor present in the system.
    /// </remarks>
    /// <returns>A list of ushort values representing the DataWidth of each processor, or null if unable to retrieve.</returns>
    public List<ushort>? GetProcessorDataWidth()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var dataWidthValues = new List<ushort>();

        try
        {
            var query = "SELECT DataWidth FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var dataWidth = queryObj["DataWidth"];

                    if (
                        dataWidth != null
                        && ushort.TryParse(dataWidth.ToString(), out ushort dataWidthValue)
                    )
                    {
                        dataWidthValues.Add(dataWidthValue);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching DataWidth: {ex.Message}");
        }

        return dataWidthValues.Count > 0 ? dataWidthValues : null;
    }

    /// <summary>
    /// Retrieves the Description for each processor.
    /// </summary>
    /// <remarks>
    /// The Description property provides a textual description of the processor. This method queries WMI to fetch the
    /// Description for every processor present in the system.
    /// </remarks>
    /// <returns>A list of strings each representing the Description of a processor, or null if unable to retrieve.</returns>
    public List<string>? GetProcessorDescription()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var descriptions = new List<string>();

        try
        {
            var query = "SELECT Description FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var description = queryObj["Description"].ToString();
                    if (!string.IsNullOrEmpty(description))
                    {
                        descriptions.Add(description);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(
                $"An error occurred while fetching processor Descriptions: {ex.Message}"
            );
        }

        return descriptions.Count > 0 ? descriptions : null;
    }

    /// <summary>
    /// Retrieves the DeviceID for each processor.
    /// </summary>
    /// <remarks>
    /// The DeviceID property provides a unique identifier for each processor on the system. This method queries WMI to fetch the
    /// DeviceID for every processor present in the system, facilitating identification and cataloging of processor units.
    /// This property is crucial for distinguishing among multiple processors within a single system.
    /// </remarks>
    /// <returns>A list of strings each representing the DeviceID of a processor, or null if unable to retrieve.</returns>
    public List<string>? GetProcessorDeviceID()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var deviceIds = new List<string>();

        try
        {
            var query = "SELECT DeviceID FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var deviceId = queryObj["DeviceID"].ToString();
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        deviceIds.Add(deviceId);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(
                $"An error occurred while fetching processor DeviceIDs: {ex.Message}"
            );
        }

        return deviceIds.Count > 0 ? deviceIds : null;
    }

    /// <summary>
    /// Retrieves the ErrorCleared status for each processor.
    /// </summary>
    /// <remarks>
    /// The ErrorCleared property indicates whether the processor's last reported error (noted in LastErrorCode) has been resolved.
    /// A True value signifies that no errors are currently flagged for the processor. This method queries WMI to fetch the
    /// ErrorCleared status for every processor present in the system.
    /// </remarks>
    /// <returns>A list of boolean values each representing the ErrorCleared status of a processor,
    /// or null if unable to retrieve.</returns>
    public List<bool>? GetProcessorErrorCleared()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var errorClearedStatuses = new List<bool>();

        try
        {
            var query = "SELECT ErrorCleared FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var errorCleared = queryObj["ErrorCleared"];

                    if (
                        errorCleared != null
                        && bool.TryParse(errorCleared.ToString(), out bool errorClearedValue)
                    )
                    {
                        errorClearedStatuses.Add(errorClearedValue);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(
                $"An error occurred while fetching processor error cleared statuses: {ex.Message}"
            );
        }

        return errorClearedStatuses.Count > 0 ? errorClearedStatuses : null;
    }

    /// <summary>
    /// Retrieves the ErrorDescription for each processor.
    /// </summary>
    /// <remarks>
    /// The ErrorDescription property provides a textual description of the last error reported by the processor,
    /// if any. This method queries WMI to fetch the ErrorDescription for every processor present in the system,
    /// offering insights into processor-specific problems that have occurred.
    /// </remarks>
    /// <returns>A list of strings each representing the ErrorDescription of a processor, or null if unable to retrieve.</returns>
    public List<string>? GetProcessorErrorDescription()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return null;
        }

        var errorDescriptionsList = new List<string>();

        try
        {
            var query = "SELECT ErrorDescription FROM Win32_Processor";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (var item in searcher.Get())
                {
                    var errorDescription = item["ErrorDescription"]?.ToString();
                    if (!string.IsNullOrEmpty(errorDescription))
                    {
                        errorDescriptionsList.Add(errorDescription);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(
                $"An error occurred while fetching processor error descriptions: {ex.Message}"
            );
        }

        return errorDescriptionsList.Count > 0 ? errorDescriptionsList : null;
    }

    /// <summary>
    /// Retrieves the ExtClock (External Clock Frequency) for each processor, expressed in MHz.
    /// </summary>
    /// <remarks>
    /// The ExtClock property indicates the external clock frequency of the processor in megahertz (MHz).
    /// If the frequency is unknown or not applicable, the property is set to NULL.
    /// This method queries the WMI for the ExtClock property for every processor in the system,
    /// based on SMBIOS Processor Information.
    /// </remarks>
    /// <returns>A list of uint values representing the ExtClock of each processor, or null if unable to retrieve.</returns>
    public List<uint>? GetProcessorExtClock()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var extClockFrequencies = new List<uint>();

        try
        {
            var query = "SELECT ExtClock FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    // Checking for null since the property may not be set if unknown
                    if (
                        queryObj["ExtClock"] != null
                        && uint.TryParse(queryObj["ExtClock"].ToString(), out uint extClock)
                    )
                    {
                        extClockFrequencies.Add(extClock);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(
                $"An error occurred while fetching ExtClock frequencies: {ex.Message}"
            );
        }

        // Notice, we return null if we were unable to fetch any frequencies,
        // adhering to the consistency requirement.
        return extClockFrequencies.Count > 0 ? extClockFrequencies : null;
    }

    /// <summary>
    /// Retrieves the processor family identifier for each processor.
    /// </summary>
    /// <returns>A list of ushort values representing the processor family identifier of each processor,
    /// or null if the information is unable to be retrieved.</returns>
    public List<ushort>? GetProcessorFamily()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var familyValues = new List<ushort>();

        try
        {
            var query = "SELECT Family FROM Win32_Processor";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Family"] is ushort family)
                    {
                        familyValues.Add(family);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine(
                $"An error occurred while fetching processor family values: {ex.Message}"
            );
        }

        return familyValues.Count > 0 ? familyValues : null;
    }

    /// <summary>
    /// Converts a list of processor family identifiers into a list of corresponding friendly names.
    /// </summary>
    /// <returns>A list of friendly names corresponding to the processor family identifiers,
    /// or null if the input list is empty or null.</returns>
    public List<string>? GetProcessorFamilyFriendlyValue()
    {
        var familyList = GetProcessorFamily();
        var friendlyList = new List<string>();

        if (familyList == null || familyList.Count == 0)
        {
            return null;
        }

        foreach (var value in familyList)
        {
            var friendlyName = MapProcessorFamilyToFriendlyName(value);
            if (friendlyName != null)
            {
                friendlyList.Add(friendlyName);
            }
        }

        return friendlyList.Count > 0 ? friendlyList : null;
    }

    /// <summary>
    /// Maps a single processor family identifier to its friendly name.
    /// </summary>
    /// <param name="familyValue">The family identifier of the processor.</param>
    /// <returns>The friendly name corresponding to the processor family identifier, or null if there is no defined mapping.</returns>
    public string? MapProcessorFamilyToFriendlyName(ushort familyValue)
    {
        var mapping = new Dictionary<ushort, string>
        {
            { 1, "Other" },
            { 2, "Unknown" },
            { 3, "8086" },
            { 4, "80286" },
            { 5, "80386" },
            { 6, "80486" },
            { 7, "8087" },
            { 8, "80287" },
            { 9, "80387" },
            { 10, "80487" },
            { 11, "Pentium(R) brand" },
            { 12, "Pentium(R) Pro" },
            { 13, "Pentium(R) II" },
            { 14, "Pentium(R) processor with MMX(TM) technology" },
            { 15, "Celeron(TM)" },
            { 16, "Pentium(R) II Xeon(TM)" },
            { 17, "Pentium(R) III" },
            { 18, "M1 Family" },
            { 19, "M2 Family" },
            { 20, "Intel(R) Celeron(R) M processor" },
            { 21, "Intel(R) Pentium(R) 4 HT processor" },
            { 24, "K5 Family" },
            { 25, "K6 Family" },
            { 26, "K6-2" },
            { 27, "K6-3" },
            { 28, "AMD Athlon(TM) Processor Family" },
            { 29, "AMD(R) Duron(TM) Processor" },
            { 30, "AMD29000 Family" },
            { 31, "K6-2+" },
            { 32, "Power PC Family" },
            { 33, "Power PC 601" },
            { 34, "Power PC 603" },
            { 35, "Power PC 603+" },
            { 36, "Power PC 604" },
            { 37, "Power PC 620" },
            { 38, "Power PC X704" },
            { 39, "Power PC 750" },
            { 40, "Intel(R) Core(TM) Duo processor" },
            { 41, "Intel(R) Core(TM) Duo mobile processor" },
            { 42, "Intel(R) Core(TM) Solo mobile processor" },
            { 43, "Intel(R) Atom(TM) processor" },
            { 48, "Alpha Family" },
            { 49, "Alpha 21064" },
            { 50, "Alpha 21066" },
            { 51, "Alpha 21164" },
            { 52, "Alpha 21164PC" },
            { 53, "Alpha 21164a" },
            { 54, "Alpha 21264" },
            { 55, "Alpha 21364" },
            { 56, "AMD Turion(TM) II Ultra Dual-Core Mobile M Processor Family" },
            { 57, "AMD Turion(TM) II Dual-Core Mobile M Processor Family" },
            { 58, "AMD Athlon(TM) II Dual-Core Mobile M Processor Family" },
            { 59, "AMD Opteron(TM) 6100 Series Processor" },
            { 60, "AMD Opteron(TM) 4100 Series Processor" },
            { 64, "MIPS Family" },
            { 65, "MIPS R4000" },
            { 66, "MIPS R4200" },
            { 67, "MIPS R4400" },
            { 68, "MIPS R4600" },
            { 69, "MIPS R10000" },
            { 80, "SPARC Family" },
            { 81, "SuperSPARC" },
            { 82, "microSPARC II" },
            { 83, "microSPARC IIep" },
            { 84, "UltraSPARC" },
            { 85, "UltraSPARC II" },
            { 86, "UltraSPARC IIi" },
            { 87, "UltraSPARC III" },
            { 88, "UltraSPARC IIIi" },
            { 96, "68040" },
            { 97, "68xxx Family" },
            { 98, "68000" },
            { 99, "68010" },
            { 100, "68020" },
            { 101, "68030" },
            { 112, "Hobbit Family" },
            { 120, "Crusoe(TM) TM5000 Family" },
            { 121, "Crusoe(TM) TM3000 Family" },
            { 122, "Efficeon(TM) TM8000 Family" },
            { 128, "Weitek" },
            { 130, "Itanium(TM) Processor" },
            { 131, "AMD Athlon(TM) 64 Processor Family" },
            { 132, "AMD Opteron(TM) Processor Family" },
            { 133, "AMD Sempron(TM) Processor Family" },
            { 134, "AMD Turion(TM) 64 Mobile Technology" },
            { 135, "Dual-Core AMD Opteron(TM) Processor Family" },
            { 136, "AMD Athlon(TM) 64 X2 Dual-Core Processor Family" },
            { 137, "AMD Turion(TM) 64 X2 Mobile Technology" },
            { 138, "Quad-Core AMD Opteron(TM) Processor Family" },
            { 139, "Third-Generation AMD Opteron(TM) Processor Family" },
            { 140, "AMD Phenom(TM) FX Quad-Core Processor Family" },
            { 141, "AMD Phenom(TM) X4 Quad-Core Processor Family" },
            { 142, "AMD Phenom(TM) X2 Dual-Core Processor Family" },
            { 143, "AMD Athlon(TM) X2 Dual-Core Processor Family" },
            { 144, "PA-RISC Family" },
            { 145, "PA-RISC 8500" },
            { 146, "PA-RISC 8000" },
            { 147, "PA-RISC 7300LC" },
            { 148, "PA-RISC 7200" },
            { 149, "PA-RISC 7100LC" },
            { 150, "PA-RISC 7100" },
            { 160, "V30 Family" },
            { 161, "Quad-Core Intel(R) Xeon(R) processor 3200 Series" },
            { 162, "Dual-Core Intel(R) Xeon(R) processor 3000 Series" },
            { 163, "Quad-Core Intel(R) Xeon(R) processor 5300 Series" },
            { 164, "Dual-Core Intel(R) Xeon(R) processor 5100 Series" },
            { 165, "Dual-Core Intel(R) Xeon(R) processor 5000 Series" },
            { 166, "Dual-Core Intel(R) Xeon(R) processor LV" },
            { 167, "Dual-Core Intel(R) Xeon(R) processor ULV" },
            { 168, "Dual-Core Intel(R) Xeon(R) processor 7100 Series" },
            { 169, "Quad-Core Intel(R) Xeon(R) processor 5400 Series" },
            { 170, "Quad-Core Intel(R) Xeon(R) processor" },
            { 171, "Dual-Core Intel(R) Xeon(R) processor 5200 Series" },
            { 172, "Dual-Core Intel(R) Xeon(R) processor 7200 Series" },
            { 173, "Quad-Core Intel(R) Xeon(R) processor 7300 Series" },
            { 174, "Quad-Core Intel(R) Xeon(R) processor 7400 Series" },
            { 175, "Multi-Core Intel(R) Xeon(R) processor 7400 Series" },
            { 176, "Pentium(R) III Xeon(TM)" },
            { 177, "Pentium(R) III Processor with Intel(R) SpeedStep(TM) Technology" },
            { 178, "Pentium(R) 4" },
            { 179, "Intel(R) Xeon(TM)" },
            { 180, "AS400 Family" },
            { 181, "Intel(R) Xeon(TM) processor MP" },
            { 182, "AMD Athlon(TM) XP Family" },
            { 183, "AMD Athlon(TM) MP Family" },
            { 184, "Intel(R) Itanium(R) 2" },
            { 185, "Intel(R) Pentium(R) M processor" },
            { 186, "Intel(R) Celeron(R) D processor" },
            { 187, "Intel(R) Pentium(R) D processor" },
            { 188, "Intel(R) Pentium(R) Processor Extreme Edition" },
            { 189, "Intel(R) Core(TM) Solo Processor" },
            { 190, "K7" },
            { 191, "Intel(R) Core(TM)2 Duo Processor" },
            { 192, "Intel(R) Core(TM)2 Solo processor" },
            { 193, "Intel(R) Core(TM)2 Extreme processor" },
            { 194, "Intel(R) Core(TM)2 Quad processor" },
            { 195, "Intel(R) Core(TM)2 Extreme mobile processor" },
            { 196, "Intel(R) Core(TM)2 Duo mobile processor" },
            { 197, "Intel(R) Core(TM)2 Solo mobile processor" },
            { 198, "Intel(R) Core(TM) i7 processor" },
            { 199, "Dual-Core Intel(R) Celeron(R) Processor" },
            { 200, "S/390 and zSeries Family" },
            { 201, "ESA/390 G4" },
            { 202, "ESA/390 G5" },
            { 203, "ESA/390 G6" },
            { 204, "z/Architectur base" },
            { 205, "Intel(R) Core(TM) i5 processor" },
            { 206, "Intel(R) Core(TM) i3 processor" },
            { 207, "Intel(R) Core(TM) i9 processor" },
            { 210, "VIA C7(TM)-M Processor Family" },
            { 211, "VIA C7(TM)-D Processor Family" },
            { 212, "VIA C7(TM) Processor Family" },
            { 213, "VIA Eden(TM) Processor Family" },
            { 214, "Multi-Core Intel(R) Xeon(R) processor" },
            { 215, "Dual-Core Intel(R) Xeon(R) processor 3xxx Series" },
            { 216, "Quad-Core Intel(R) Xeon(R) processor 3xxx Series" },
            { 217, "VIA Nano(TM) Processor Family" },
            { 218, "Dual-Core Intel(R) Xeon(R) processor 5xxx Series" },
            { 219, "Quad-Core Intel(R) Xeon(R) processor 5xxx Series" },
            { 221, "Dual-Core Intel(R) Xeon(R) processor 7xxx Series" },
            { 222, "Quad-Core Intel(R) Xeon(R) processor 7xxx Series" },
            { 223, "Multi-Core Intel(R) Xeon(R) processor 7xxx Series" },
            { 224, "Multi-Core Intel(R) Xeon(R) processor 3400 Series" },
            { 230, "Embedded AMD Opteron(TM) Quad-Core Processor Family" },
            { 231, "AMD Phenom(TM) Triple-Core Processor Family" },
            { 232, "AMD Turion(TM) Ultra Dual-Core Mobile Processor Family" },
            { 233, "AMD Turion(TM) Dual-Core Mobile Processor Family" },
            { 234, "AMD Athlon(TM) Dual-Core Processor Family" },
            { 235, "AMD Sempron(TM) SI Processor Family" },
            { 236, "AMD Phenom(TM) II Processor Family" },
            { 237, "AMD Athlon(TM) II Processor Family" },
            { 238, "Six-Core AMD Opteron(TM) Processor Family" },
            { 239, "AMD Sempron(TM) M Processor Family" },
            { 250, "i860" },
            { 251, "i960" },
            { 254, "Reserved (SMBIOS Extension)" },
            { 255, "Reserved (Un-initialized Flash Content - Lo)" },
            { 260, "SH-3" },
            { 261, "SH-4" },
            { 280, "ARM" },
            { 281, "StrongARM" },
            { 300, "6x86" },
            { 301, "MediaGX" },
            { 302, "MII" },
            { 320, "WinChip" },
            { 350, "DSP" },
            { 500, "Video Processor" },
            { 65534, "Reserved (For Future Special Purpose Assignment)" },
            { 65535, "Reserved (Un-initialized Flash Content - Hi)" }
        };

        if (mapping.TryGetValue(familyValue, out string? friendlyName))
        {
            return friendlyName;
        }

        return null; // Return null for unmapped values
    }

    /// <summary>
    /// Retrieves the InstallDate for each processor.
    /// </summary>
    /// <returns>A list of DateTime values representing the InstallDate of each processor, or null if unable to retrieve.</returns>
    public List<DateTime>? GetProcessorInstallDate()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var installDates = new List<DateTime>();

        try
        {
            var query = "SELECT InstallDate FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var installDate = queryObj["InstallDate"];

                    if (
                        installDate != null
                        && ManagementDateTimeConverter.ToDateTime(installDate.ToString())
                            != DateTime.MinValue
                    )
                    {
                        installDates.Add(
                            ManagementDateTimeConverter.ToDateTime(installDate.ToString())
                        );
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching InstallDate: {ex.Message}");
        }

        return installDates.Count > 0 ? installDates : null;
    }

    /// <summary>
    /// Retrieves the L2CacheSize for each processor in kilobytes (KiB).
    /// </summary>
    /// <returns>A list of uint values representing the L2CacheSize in kilobytes (KiB) of each processor, or null if unable to retrieve.</returns>
    public List<uint>? GetProcessorL2CacheSize()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var l2CacheSizes = new List<uint>();

        try
        {
            var query = "SELECT L2CacheSize FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var l2CacheSize = queryObj["L2CacheSize"];

                    if (
                        l2CacheSize != null
                        && uint.TryParse(l2CacheSize.ToString(), out uint cacheSize)
                    )
                    {
                        l2CacheSizes.Add(cacheSize);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching L2CacheSize: {ex.Message}");
        }

        return l2CacheSizes.Count > 0 ? l2CacheSizes : null;
    }

    /// <summary>
    /// Retrieves the clock speed of the Level 2 processor cache in megahertz (MHz).
    /// </summary>
    /// <returns>
    /// A list of uint values representing the clock speed of the Level 2 processor cache in megahertz (MHz),
    /// or null if unable to retrieve the information.
    /// </returns>
    public List<uint>? GetProcessorL2CacheSpeed()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var l2CacheSpeeds = new List<uint>();

        try
        {
            var query = "SELECT L2CacheSpeed FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var l2CacheSpeed = queryObj["L2CacheSpeed"];

                    if (
                        l2CacheSpeed != null
                        && uint.TryParse(l2CacheSpeed.ToString(), out uint speed)
                    )
                    {
                        l2CacheSpeeds.Add(speed);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching L2 cache speeds: {ex.Message}");
        }

        return l2CacheSpeeds.Count > 0 ? l2CacheSpeeds : null;
    }

    /// <summary>
    /// Retrieves the size of the Level 3 processor cache in kilobytes (KiB).
    /// </summary>
    /// <returns>
    /// A list of uint values representing the size of the Level 3 processor cache in kilobytes (KiB),
    /// or null if unable to retrieve the information.
    /// </returns>
    public List<uint>? GetProcessorL3CacheSize()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var l3CacheSizes = new List<uint>();

        try
        {
            var query = "SELECT L3CacheSize FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var l3CacheSize = queryObj["L3CacheSize"];

                    if (l3CacheSize != null && uint.TryParse(l3CacheSize.ToString(), out uint size))
                    {
                        l3CacheSizes.Add(size);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching L3 cache sizes: {ex.Message}");
        }

        return l3CacheSizes.Count > 0 ? l3CacheSizes : null;
    }

    /// <summary>
    /// Retrieves the clock speed of the Level 3 processor cache in megahertz (MHz).
    /// </summary>
    /// <returns>
    /// A list of uint values representing the clock speed of the Level 3 processor cache in megahertz (MHz),
    /// or null if unable to retrieve the information.
    /// </returns>
    public List<uint>? GetProcessorL3CacheSpeed()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var l3CacheSpeeds = new List<uint>();

        try
        {
            var query = "SELECT L3CacheSpeed FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var l3CacheSpeed = queryObj["L3CacheSpeed"];

                    if (
                        l3CacheSpeed != null
                        && uint.TryParse(l3CacheSpeed.ToString(), out uint speed)
                    )
                    {
                        l3CacheSpeeds.Add(speed);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching L3 cache speeds: {ex.Message}");
        }

        return l3CacheSpeeds.Count > 0 ? l3CacheSpeeds : null;
    }

    /// <summary>
    /// Retrieves the last error code reported by the logical device.
    /// </summary>
    /// <returns>
    /// The last error code reported by the logical device as a uint value,
    /// or null if unable to retrieve the information.
    /// </returns>
    public uint? GetProcessorLastErrorCode()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        try
        {
            var query = "SELECT LastErrorCode FROM CIM_LogicalDevice";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var lastErrorCode = queryObj["LastErrorCode"];
                    if (lastErrorCode != null && lastErrorCode is uint errorCode)
                    {
                        return errorCode;
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"An error occurred while fetching last error code: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Retrieves the definition of the processor type.
    /// </summary>
    /// <returns>A list of ushort values representing the processor type levels, or null if an error occurs or the platform is not Windows.</returns>
    public List<ushort>? GetProcessorLevel()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var processorLevels = new List<ushort>();

        try
        {
            var query = "SELECT Level FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var level = queryObj["Level"];
                    if (level != null && ushort.TryParse(level.ToString(), out var levelValue))
                    {
                        processorLevels.Add(levelValue);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return processorLevels.Count > 0 ? processorLevels : null;
    }

    /// <summary>
    /// Retrieves the load capacity of each processor, averaged to the last second.
    /// </summary>
    /// <returns>A list of ushort values representing the load percentages of each processor, or null if an error occurs or the platform is not Windows.</returns>
    public List<ushort>? GetProcessorLoadPercentage()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var loadPercentages = new List<ushort>();

        try
        {
            var query = "SELECT LoadPercentage FROM Win32_PerfFormattedData_PerfOS_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var loadPercentage = queryObj["LoadPercentage"];
                    if (
                        loadPercentage != null
                        && ushort.TryParse(loadPercentage.ToString(), out var loadPercentageValue)
                    )
                    {
                        loadPercentages.Add(loadPercentageValue);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return loadPercentages.Count > 0 ? loadPercentages : null;
    }

    /// <summary>
    /// Retrieves the name of the processor manufacturer.
    /// </summary>
    /// <returns>A list of strings representing the processor manufacturers, or null if an error occurs or the platform is not Windows.</returns>
    public List<string>? GetProcessorManufacturer()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var manufacturers = new List<string>();

        try
        {
            var query = "SELECT Manufacturer FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var manufacturer = queryObj["Manufacturer"].ToString();
                    if (!string.IsNullOrEmpty(manufacturer))
                    {
                        manufacturers.Add(manufacturer);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return manufacturers.Count > 0 ? manufacturers : null;
    }

    /// <summary>
    /// Retrieves the maximum speed of the processor in megahertz (MHz).
    /// </summary>
    /// <returns>A list of uint values representing the maximum speeds of the processors in megahertz (MHz), or null if an error occurs or the platform is not Windows.</returns>
    public List<uint>? GetProcessorMaxClockSpeed()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var maxClockSpeeds = new List<uint>();

        try
        {
            var query = "SELECT MaxClockSpeed FROM Win32_Processor";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var maxClockSpeed = queryObj["MaxClockSpeed"];
                    if (
                        maxClockSpeed != null
                        && uint.TryParse(maxClockSpeed.ToString(), out var maxClockSpeedValue)
                    )
                    {
                        maxClockSpeeds.Add(maxClockSpeedValue);
                    }
                }
            }
        }
        catch (ManagementException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return maxClockSpeeds.Count > 0 ? maxClockSpeeds : null;
    }

    // /// <summary>
    // /// Gets the count of physcial and logical CPU cores.
    // /// </summary>
    // /// <returns>A List of Tuples containing both the physical and logical core counts.</returns>
    // private List<(int PhysicalCores, int LogicalCores)>? GetCpuCoreCounts()
    // {
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //         return null;

    //     var coreCountsList = new List<(int PhysicalCores, int LogicalCores)>();

    //     try
    //     {
    //         // Use ManagementObjectSearcher to query information about the CPU
    //         using (
    //             var searcher = new ManagementObjectSearcher(
    //                 "select NumberOfCores, NumberOfLogicalProcessors from Win32_Processor"
    //             )
    //         )
    //         {
    //             foreach (var item in searcher.Get())
    //             {
    //                 var physcialCoresObject = item["NumberOfCores"];
    //                 var logicalCoresObject = item["NumberOfLogicalProcessors"];

    //                 if (
    //                     physcialCoresObject != null
    //                     && int.TryParse(physcialCoresObject.ToString(), out int physicalCores)
    //                     && logicalCoresObject != null
    //                     && int.TryParse(logicalCoresObject.ToString(), out int logicalCores)
    //                 )
    //                     coreCountsList.Add((physicalCores, logicalCores));
    //             }
    //         }
    //     }
    //     catch
    //     {
    //         // Log or handle the exception
    //         Console.WriteLine($"Error fetching CPU core counts.");
    //     }
    //     return coreCountsList.Count > 0 ? coreCountsList : null;
    // }

    // /// <summary>
    // /// Adds each CPU's brand string (Name) to a list and returns the list.
    // /// </summary>
    // /// <returns>A list of all CPU brand strings (Names)</returns>
    // public List<string>? GetCpuNames()
    // {
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //         return null;

    //     var processorNames = new List<string>();

    //     try
    //     {
    //         using (var searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
    //         {
    //             foreach (var item in searcher.Get())
    //             {
    //                 var name = item["Name"].ToString();
    //                 if (!string.IsNullOrEmpty(name))
    //                 {
    //                     processorNames.Add(name);
    //                 }
    //             }
    //         }
    //     }
    //     catch
    //     {
    //         Console.WriteLine("Error fetching processor name.");
    //     }

    //     return processorNames.Count > 0 ? processorNames : null;
    // }

    // private List<string>? GetCpuManufacturer()
    // {
    //     // Prepare to return null if not running on Windows
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //     {
    //         throw new Exception(
    //             "A Windows Profiler is attempting to profile an operating system which is not identifying as Windows."
    //         );
    //     }

    //     var manufacturers = new List<string>();

    //     try
    //     {
    //         using (
    //             var searcher = new ManagementObjectSearcher(
    //                 "select Manufacturer from Win32_Processor"
    //             )
    //         )
    //         {
    //             foreach (var item in searcher.Get())
    //             {
    //                 var manufacturer = item["Manufacturer"].ToString();
    //                 if (!string.IsNullOrEmpty(manufacturer))
    //                 {
    //                     manufacturers.Add(manufacturer); // Add manufacturer to the list
    //                 }
    //             }
    //         }
    //     }
    //     catch
    //     {
    //         Console.WriteLine($"Error fetching CPU manufacturer.");
    //     }

    //     return manufacturers.Count > 0 ? manufacturers : null; // Return list or null
    // }

    // /// <summary>
    // /// Retrieves the caption of each processor.
    // /// </summary>
    // /// <returns>A list of strings representing the caption of each processor, or null if the operation fails.</returns>
    // public List<string>? GetProcessorCaption()
    // {
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //     {
    //         Console.WriteLine("Error: This method is only supported on Windows.");
    //         return null;
    //     }

    //     var captions = new List<string>();

    //     try
    //     {
    //         var query = "SELECT Caption FROM Win32_Processor";

    //         using (var searcher = new ManagementObjectSearcher(query))
    //         {
    //             foreach (ManagementObject queryObj in searcher.Get())
    //             {
    //                 var caption = queryObj["Caption"] as string;

    //                 if (!string.IsNullOrEmpty(caption))
    //                 {
    //                     captions.Add(caption);
    //                 }
    //             }
    //         }
    //     }
    //     catch (ManagementException ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }

    //     return captions.Count > 0 ? captions : null;
    // }

    // private List<(int L2, int L3)>? GetCpuCacheSize()
    // {
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //     {
    //         Console.WriteLine("Error: This method is only supported on Windows.");
    //         return null;
    //     }

    //     var cacheSizes = new List<(int L2, int L3)>();

    //     try
    //     {
    //         var query = "SELECT L2CacheSize, L3CacheSize FROM Win32_Processor";

    //         using (var searcher = new ManagementObjectSearcher(query))
    //         {
    //             foreach (ManagementObject queryObj in searcher.Get())
    //             {
    //                 var l2CacheSize =
    //                     queryObj["L2CacheSize"] != null
    //                         ? Convert.ToInt32(queryObj["L2CacheSize"])
    //                         : 0;
    //                 var l3CacheSize =
    //                     queryObj["L3CacheSize"] != null
    //                         ? Convert.ToInt32(queryObj["L3CacheSize"])
    //                         : 0;

    //                 cacheSizes.Add((l2CacheSize / 1000, l3CacheSize / 1000));
    //             }
    //         }
    //     }
    //     catch (ManagementException ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }

    //     return cacheSizes.Count > 0 ? cacheSizes : null;
    // }

    // private List<string>? GetCpuId()
    // {
    //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //     {
    //         Console.WriteLine("Error: This method is only supported on Windows.");
    //         return null;
    //     }

    //     var cpuIds = new List<string>();

    //     try
    //     {
    //         var query = "SELECT ProcessorId FROM Win32_Processor";

    //         using (var searcher = new ManagementObjectSearcher(query))
    //         {
    //             foreach (ManagementObject queryObj in searcher.Get())
    //             {
    //                 var cpuIdString = queryObj["ProcessorId"]?.ToString();

    //                 if (!string.IsNullOrEmpty(cpuIdString))
    //                 {
    //                     cpuIds.Add(cpuIdString);
    //                 }
    //             }
    //         }
    //     }
    //     catch (ManagementException ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }

    //     return cpuIds.Count > 0 ? cpuIds : null;
    // }
}
