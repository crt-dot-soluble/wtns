using System.Management;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace WTNS.Telemetry;

/*  Required Documentation & Resources
*
*       Mappings for Most Win32_Processor Properties:
*           https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor?redirectedfrom=MSDN
*
*       Processor Characteristics Mappings:
*           https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.6.0.pdf
*
*       TODO:
*           MapProcessorFamilyToFriendlyValue - Complete the mappings from documentation ^
*           MapProcessorConfigManagerErrorCodeToFriendlyValue - Complete the mappings from documentation
*
*           Complete ToFriendlyValue functions for all properties requiring them

            Fix warning on ManagementDateTimeConverter usage (anywhere DateTime is being used)
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
*   like System.Convert, with a null check included WITHIN GetWmiObject, as to avoid recieving back default
*   values.
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
    /*
    * -------------------------------------------------------------------------------------------------------
    * ---- SYSTEM INFORMATION STRUCTURES
    * -------------------------------------------------------------------------------------------------------
    */

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
            DeviceID = GetProcessorDeviceID(),
            ErrorCleared = GetProcessorErrorCleared(),
            ErrorDescription = GetProcessorErrorDescription(),
            ExtClock = GetProcessorExtClock(),
            Family = GetProcessorFamily(),
            FamilyFriendlyValue = GetProcessorFamilyFriendlyValue(),
            InstallDate = GetProcessorInstallDate(),
            L2CacheSize = GetProcessorL2CacheSize(),
            L2CacheSpeed = GetProcessorL2CacheSpeed(),
            L3CacheSize = GetProcessorL3CacheSize(),
            L3CacheSpeed = GetProcessorL3CacheSpeed(),
            LastErrorCode = GetProcessorLastErrorCode(),
            Level = GetProcessorLevel(),
            Manufacturer = GetProcessorManufacturer(),
            Name = GetProcessorName(),
            NumberOfCores = GetProcessorNumberOfCores(),
            NumberOfEnabledCore = GetProcessorNumberOfEnabledCore(),
            NumberOfLogicalProcessors = GetProcessorNumberOfLogicalProcessors(),
            PartNumber = GetProcessorPartNumber(),
            PNPDeviceID = GetProcessorPNPDeviceID(),
            PowerManagementCapabilities = GetProcessorPowerManagementCapabilities(),
            PowerManagementCapabilitiesFriendlyValue =
                GetProcessorPowerManagementCapabilitiesFriendlyValue(),
            PowerManagementSupported = GetProcessorPowerManagementSupported(),
            ProcessorID = GetProcessorProcessorId(),
            ProcessorType = GetProcessorProcessorType(),
            ProcessorTypeFriendlyValue = GetProcessorProcessorTypeFriendlyValue(),
            Revision = GetProcessorRevision(),
            Role = GetProcessorRole(),
            SecondLevelAddressTranslationExtensions =
                GetProcessorSecondLevelAddressTranslationExtensions(),
            SerialNumber = GetProcessorSerialNumber(),
            SocketDesignation = GetProcessorSocketDesignation(),
            Status = GetProcessorStatus(),
            StatusInfo = GetProcessorStatusInfo(),
            StatusInfoFriendlyValue = GetProcessorStatusInfoFriendlyValue(),
            Stepping = GetProcessorStepping(),
            SystemCreationClassName = GetProcessorSystemCreationClassName(),
            SystemName = GetProcessorSystemName(),
            ThreadCount = GetProcessorThreadCount(),
            UniqueID = GetProcessorUniqueID(),
        };
    }

    /// <inheritdoc/>
    public StorageInfo GetStorageInfo()
    {
        return new StorageInfo
        {
            Availability = GetStorageAvailability(),
            BytesPerSector = GetStorageBytesPerSector(),
            Capabilities = GetStorageCapabilities(),
            CapabilityDescriptions = GetStorageCapabilityDescriptions(),
            Caption = GetStorageCaption(),
            CompressionMethod = GetStorageCompressionMethod(),
            ConfigManagerErrorCode = GetStorageConfigManagerErrorCode(),
            ConfigManagerUserConfig = GetStorageConfigManagerUserConfig(),
            CreationClassName = GetStorageCreationClassName(),
            DefaultBlockSize = GetStorageDefaultBlockSize(),
            Description = GetStorageDescription(),
            DeviceID = GetStorageDeviceID(),
            ErrorCleared = GetStorageErrorCleared(),
            ErrorDescription = GetStorageErrorDescription(),
            ErrorMethodology = GetStorageErrorMethodology(),
            FirmwareRevision = GetStorageFirmwareRevision(),
            Index = GetStorageIndex(),
            InstallDate = GetStorageInstallDate(),
            InterfaceType = GetStorageInterfaceType(),
            LastErrorCode = GetStorageLastErrorCode(),
            Manufacturer = GetStorageManufacturer(),
            MaxBlockSize = GetStorageMaxBlockSize(),
            MaxMediaSize = GetStorageMaxMediaSize(),
            MediaLoaded = GetStorageMediaLoaded(),
            MediaType = GetStorageMediaType(),
            MinBlockSize = GetStorageMinBlockSize(),
            Model = GetStorageModel(),
            Name = GetStorageName(),
            NeedsCleaning = GetStorageNeedsCleaning(),
            NumberOfMediaSupported = GetStorageNumberOfMediaSupported(),
            Partitions = GetStoragePartitions(),
            PNPDeviceID = GetStoragePNPDeviceID(),
            PowerManagementCapabilities = GetStoragePowerManagementCapabilities(),
            PowerManagementSupported = GetStoragePowerManagementSupported(),
            SCSIBus = GetStorageSCSIBus(),
            SCSILogicalUnit = GetStorageSCSILogicalUnit(),
            SCSIPort = GetStorageSCSIPort(),
            SCSITargetId = GetStorageSCSITargetId(),
            SectorsPerTrack = GetStorageSectorsPerTrack(),
            SerialNumber = GetStorageSerialNumber(),
            Signature = GetStorageSignature(),
            Size = GetStorageSize(),
            Status = GetStorageStatus(),
            StatusInfo = GetStorageStatusInfo(),
            SystemCreationClassName = GetStorageSystemCreationClassName(),
            SystemName = GetStorageSystemName(),
            TotalCylinders = GetStorageTotalCylinders(),
            TotalHeads = GetStorageTotalHeads(),
            TotalSectors = GetStorageTotalSectors(),
            TotalTracks = GetStorageTotalTracks(),
            TracksPerCylinder = GetStorageTracksPerCylinder(),
        };
    }

    /// <inheritdoc/>
    public GpuInfo GetGpuInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public MotherboardInfo GetMotherboardInfo()
    {
        return new MotherboardInfo
        {
            Caption = GetMotherboardCaption(),
            ConfigOptions = GetMotherboardConfigOptions(),
            CreationClassName = GetMotherboardCreationClassName(),
            Depth = GetMotherboardDepth(),
            Description = GetMotherboardDescription(),
            Height = GetMotherboardHeight(),
            HostingBoard = GetMotherboardHostingBoard(),
            HotSwappable = GetMotherboardHotSwappable(),
            InstallDate = GetMotherboardInstallDate(),
            Manufacturer = GetMotherboardManufacturer(),
            Model = GetMotherboardModel(),
            Name = GetMotherboardName(),
            OtherIdentifyingInfo = GetMotherboardOtherIdentifyingInfo(),
            PartNumber = GetMotherboardPartNumber(),
            PoweredOn = GetMotherboardPoweredOn(),
            Product = GetMotherboardProduct(),
            Removable = GetMotherboardRemovable(),
            Replaceable = GetMotherboardReplaceable(),
            RequirementsDescription = GetMotherboardRequirementsDescription(),
            RequiresDaughterBoard = GetMotherboardRequiresDaughterBoard(),
            SerialNumber = GetMotherboardSerialNumber(),
            SKU = GetMotherboardSKU(),
            SlotLayout = GetMotherboardSlotLayout(),
            SpecialRequirements = GetMotherboardSpecialRequirements(),
            Status = GetMotherboardStatus(),
            Tag = GetMotherboardTag(),
            Version = GetMotherboardVersion(),
            Weight = GetMotherboardWeight(),
            Width = GetMotherboardWidth(),
        };
    }

    /// <inheritdoc/>
    public NetInfo GetNetInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public OsInfo GetOsInfo()
    {
        return new OsInfo
        {
            BootDevice = GetOsBootDevice(),
            BuildNumber = GetOsBuildNumber(),
            BuildType = GetOsBuildType(),
            Caption = GetOsCaption(),
            CodeSet = GetOsCodeSet(),
            CountryCode = GetOsCountryCode(),
            CreationClassName = GetOsCreationClassName(),
            CSDVersion = GetOsCSDVersion(),
            CSName = GetOsCSName(),
            CurrentTimeZone = GetOsCurrentTimeZone(),
            DataExecutionPrevention_Available = GetOsDataExecutionPrevention_Available(),
            DataExecutionPrevention_32BitApplications =
                GetOsDataExecutionPrevention_32BitApplications(),
            DataExecutionPrevention_Drivers = GetOsDataExecutionPrevention_Drivers(),
            DataExecutionPrevention_SupportPolicy = GetOsDataExecutionPrevention_SupportPolicy(),
            Debug = GetOsDebug(),
            Description = GetOsDescription(),
            Distributed = GetOsDistributed(),
            EncryptionLevel = GetOsEncryptionLevel(),
            ForegroundApplicationBoost = GetOsForegroundApplicationBoost(),
            FreePhysicalMemory = GetOsFreePhysicalMemory(),
            FreeSpaceInPagingFiles = GetOsFreeSpaceInPagingFiles(),
            FreeVirtualMemory = GetOsFreeVirtualMemory(),
            InstallDate = GetOsInstallDate(),
            LastBootUpTime = GetOsLastBootUpTime(),
            LocalDateTime = GetOsLocalDateTime(),
            Locale = GetOsLocale(),
            Manufacturer = GetOsManufacturer(),
            MaxNumberOfProcesses = GetOsMaxNumberOfProcesses(),
            MaxProcessMemorySize = GetOsMaxProcessMemorySize(),
            MUILanguages = GetOsMUILanguages(),
            Name = GetOsName(),
            NumberOfLicensedUsers = GetOsNumberOfLicensedUsers(),
            NumberOfProcesses = GetOsNumberOfProcesses(),
            NumberOfUsers = GetOsNumberOfUsers(),
            OperatingSystemSKU = GetOsOperatingSystemSKU(),
            Organization = GetOsOrganization(),
            OSArchitecture = GetOsOSArchitecture(),
            OSLanguage = GetOsOSLanguage(),
            OSProductSuite = GetOsOSProductSuite(),
            OSType = GetOsOSType(),
            OtherTypeDescription = GetOsOtherTypeDescription(),
            PAEEnabled = GetOsPAEEnabled(),
            PlusProductID = GetOsPlusProductID(),
            PlusVersionNumber = GetOsPlusVersionNumber(),
            PortableOperatingSystem = GetOsPortableOperatingSystem(),
            Primary = GetOsPrimary(),
            ProductType = GetOsProductType(),
            RegisteredUser = GetOsRegisteredUser(),
            SerialNumber = GetOsSerialNumber(),
            ServicePackMajorVersion = GetOsServicePackMajorVersion(),
            ServicePackMinorVersion = GetOsServicePackMinorVersion(),
            SizeStoredInPagingFiles = GetOsSizeStoredInPagingFiles(),
            Status = GetOsStatus(),
            SuiteMask = GetOsSuiteMask(),
            SystemDevice = GetOsSystemDevice(),
            SystemDirectory = GetOsSystemDirectory(),
            SystemDrive = GetOsSystemDrive(),
            TotalSwapSpaceSize = GetOsTotalSwapSpaceSize(),
            TotalVirtualMemorySize = GetOsTotalVirtualMemorySize(),
            TotalVisibleMemorySize = GetOsTotalVisibleMemorySize(),
            Version = GetOsVersion(),
            WindowsDirectory = GetOsWindowsDirectory(),
            QuantumLength = GetOsQuantumLength(),
            QuantumType = GetOsQuantumType(),
        };
    }

    /// <inheritdoc/>
    public MemoryInfo GetMemoryInfo()
    {
        return new MemoryInfo
        {
            Attributes = GetMemoryAttributes(),
            BankLabel = GetMemoryBankLabel(),
            Capacity = GetMemoryCapacity(),
            Caption = GetMemoryCaption(),
            ConfiguredClockSpeed = GetMemoryConfiguredClockSpeed(),
            ConfiguredVoltage = GetMemoryConfiguredVoltage(),
            CreationClassName = GetMemoryCreationClassName(),
            DataWidth = GetMemoryDataWidth(),
            Description = GetMemoryDescription(),
            DeviceLocator = GetMemoryDeviceLocator(),
            FormFactor = GetMemoryFormFactor(),
            HotSwappable = GetMemoryHotSwappable(),
            InstallDate = GetMemoryInstallDate(),
            InterleaveDataDepth = GetMemoryInterleaveDataDepth(),
            InterleavePosition = GetMemoryInterleavePosition(),
            Manufacturer = GetMemoryManufacturer(),
            MaxVoltage = GetMemoryMaxVoltage(),
            MemoryType = GetMemoryMemoryType(),
            MinVoltage = GetMemoryMinimumVoltage(),
            Model = GetMemoryModel(),
            Name = GetMemoryName(),
            PartNumber = GetMemoryPartNumber(),
            PositionInRow = GetMemoryPositionInRow(),
            PoweredOn = GetMemoryPoweredOn(),
            Removable = GetMemoryRemovable(),
            Replaceable = GetMemoryReplaceable(),
            SerialNumber = GetMemorySerialNumber(),
            SKU = GetMemorySKU(),
            SMBIOSMemoryType = GetMemorySMBIOSMemoryType(),
            Speed = GetMemorySpeed(),
            Status = GetMemoryStatus(),
            Tag = GetMemoryTag(),
            TotalWidth = GetMemoryTotalWidth(),
            TypeDetail = GetMemoryTypeDetail(),
            Version = GetMemoryVersion(),
        };
    }

    /// <summary>
    /// Retrieves a property of the processor using WMI as a List where each element in the List corresponds to an individual piece of hardware.
    /// </summary>
    /// <typeparam name="T">The type of the property to retrieve.</typeparam>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <param name="converter">A function to convert the retrieved object to the desired type.</param>
    /// <returns>A list of values of the specified property, or null if an error occurs or the platform is not Windows.</returns>
    private List<T>? GetWmiObject<T>(
        string propertyName,
        string className,
        Func<object?, T> converter
    )
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Error: This method is only supported on Windows.");
            return null;
        }

        var values = new List<T>();

        try
        {
            var query = $"SELECT {propertyName} FROM {className}";

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

    /*
    * -------------------------------------------------------------------------------------------------------
    * ---- PROCESSOR INFORMATION METHODS
    * -------------------------------------------------------------------------------------------------------
    *
    * Win32_Processor
    * https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor?redirectedfrom=MSDN
    *
    */

    /// <summary>
    /// Retrieves the address width of the processor.
    /// </summary>s
    /// <returns>A list of ushort values representing the address width, or null if none is found.</returns>
    public List<ushort>? GetProcessorAddressWidth()
    {
        var list = GetWmiObject(
            "AddressWidth",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the processor architecture.
    /// </summary>
    /// <returns>A list of ushort values representing the architectures of the processor, or null if none is found.</returns>
    public List<ushort>? GetProcessorArchitecture()
    {
        var list = GetWmiObject(
            "Architecture",
            "Win32_Processor",
            arch => arch != null ? (ushort?)Convert.ToUInt16(arch) : null
        )
            ?.Where(arch => arch != null)
            .Select(arch => arch!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the friendly (mapped) values of the processor architectures.
    /// </summary>
    /// <returns>
    /// A list of strings representing the human readable values of the processor architectures, or null if none is found.
    /// </returns>
    public List<string?>? GetProcessorArchitectureFriendlyValue()
    {
        var list = GetProcessorArchitecture()
            ?.Select(MapProcessorArchitectureToFriendlyValue)
            .Where(arch => arch != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps the processor architecture to a human readable value.
    /// </summary>
    /// <param name="architecture">The ushort value representing the achitecture.</param>
    /// <returns>A string value represening the human readable representation of the processor architecture.</returns>
    public string? MapProcessorArchitectureToFriendlyValue(ushort architecture)
    {
        switch (architecture)
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
    /// Retrieves the processor asset tag.
    /// </summary>
    /// <returns>A list of strings representing the asset tag, or null if none is found.</returns>
    public List<string?>? GetProcessorAssetTag()
    {
        var list = GetWmiObject(
            "AssetTag",
            "Win32_Processor",
            value =>
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    return value.ToString();
                }
            }
        )
            ?.ToList();

        return list?.Count > 0 ? list : null;
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
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the human readable (mapped) values of the processor availability.
    /// </summary>
    /// <returns>A list of strings representing the availability, or null if none is found.</returns>
    public List<string?>? GetProcessorAvailabilityFriendlyValue()
    {
        var list = GetProcessorAvailability()
            ?.Select(MapProcessorAvailabilityToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps the availability code to its corresponding friendly name.
    /// </summary>
    /// <param name="availability">The ushort value representing the availability code.</param>
    /// <returns>The friendly name of the availability, or null if the value is not recognized.</returns>
    public string? MapProcessorAvailabilityToFriendlyValue(ushort availability)
    {
        switch (availability)
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
    /// <returns>A list of uint32 values representing the characteristics of each processor, or null if none is found.</returns>
    public List<uint>? GetProcessorCharacteristics()
    {
        var list = GetWmiObject(
            "Characteristics",
            "Win32_Processor",
            characteristics => (uint?)Convert.ToUInt32(characteristics)
        )
            ?.Where(characteristics => characteristics != null)
            .Select(characteristics => characteristics!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the characteristics of each processor and maps them to their corresponding friendly names.
    /// </summary>
    /// <returns>A list of a list of strings representing the human readable values of the characteristics, or null if none is found.</returns>
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
    /// <returns>A list of uint values representing the ConfigManagerErrorCode,
    /// or null if none is found.</returns>
    public List<uint>? GetProcessorConfigManagerErrorCode()
    {
        var list = GetWmiObject(
            "ConfigManagerErrorCode",
            "Win32_Processor",
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
    public List<string?>? GetProcessorConfigManagerErrorCodeFriendlyValue()
    {
        var list = GetProcessorConfigManagerErrorCode()
            ?.Select(MapProcessorConfigManagerErrorCodeToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps a ConfigManagerErrorCode to its corresponding friendly name(s).
    /// </summary>
    /// <param name="errorCode">The uint value representing the ConfigManagerErrorCode.</param>
    /// <returns>A list of strings representing the friendly name(s) of the ConfigManagerErrorCode.</returns>
    private string? MapProcessorConfigManagerErrorCodeToFriendlyValue(uint errorCode)
    {
        return errorCode switch
        {
            0 => "This device is working properly.",
            1 => "Device is not configured correctly.",
            2 => "Windows cannot load the driver for this device.",
            3
                => "The driver for this device might be corrupted, or your system may be running low on memory or other resources.",
            4
                => "This device is not working properly. One of its drivers or your registry might be corrupted.",
            // Include all other case numbers according to the specified descriptions...
            31
                => "Device is not working properly. Windows cannot load the required device drivers.",
            _ => null,
        };
    }

    /// <summary>
    /// Retrieves the ConfigManagerUserConfig value for each processor.
    /// </summary>
    /// <returns>A list of boolean values representing whether the device is using a user-defined configuration for each processor, or null if the operation fails. True indicates a user-defined configuration is in use.</returns>
    public List<bool>? GetProcessorConfigManagerUserConfig()
    {
        var list = GetWmiObject(
            "ConfigManagerUserConfig",
            "Win32_Processor",
            value =>
            {
                if (value == null)
                {
                    return null;
                }
                else
                {
                    return (bool?)Convert.ToBoolean(value);
                }
            }
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();
        return list != null && list.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "CpuStatus",
            "Win32_Processor",
            value =>
            {
                if (value == null)
                { // Immediately return null, rather than allowing ToUint16 to convert it to a 0 (see below)
                    return null;
                }
                else
                { // special if-else check due to Convert.ToUInt16 returning 0 (a code with its own meaning) when the input is null
                    return (ushort?)Convert.ToUInt16(value);
                }
            }
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list != null && list.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves friendly names corresponding to the CpuStatus code of each processor.
    /// </summary>
    /// <remarks>
    /// Utilizes the <see cref="GetProcessorCpuStatus"/> method to fetch raw CpuStatus values and then
    /// maps them to user-friendly descriptions via <see cref="MapProcessorCpuStatusToFriendlyValue"/>.
    /// </remarks>
    /// <returns>A list of friendly names representing each processor's CpuStatus, or null if unable to retrieve.</returns>
    public List<string?>? GetProcessorCpuStatusFriendlyValue()
    {
        var list = GetProcessorCpuStatus()
            ?.Select(MapProcessorCpuStatusToFriendlyValue)
            .Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list != null && list.Count > 0 ? list : null;
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
    public List<string?>? GetProcessorCreationClassName()
    {
        var list = GetWmiObject(
            "CreationClassName",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "CurrentClockSpeed",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "CurrentVoltage",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "DataWidth",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the Description for each processor.
    /// </summary>
    /// <remarks>
    /// The Description property provides a textual description of the processor. This method queries WMI to fetch the
    /// Description for every processor present in the system.
    /// </remarks>
    /// <returns>A list of strings each representing the Description of a processor, or null if unable to retrieve.</returns>
    public List<string?>? GetProcessorDescription()
    {
        var list = GetWmiObject(
            "Description",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
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
    public List<string?>? GetProcessorDeviceID()
    {
        var list = GetWmiObject(
            "DeviceID",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "ErrorCleared",
            "Win32_Processor",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
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
    public List<string?>? GetProcessorErrorDescription()
    {
        var list = GetWmiObject(
            "ErrorDescription",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "ExtClock",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the processor family identifier for each processor.
    /// </summary>
    /// <returns>A list of ushort values representing the processor family identifier of each processor,
    /// or null if the information is unable to be retrieved.</returns>
    public List<ushort>? GetProcessorFamily()
    {
        var list = GetWmiObject(
            "Family",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Converts a list of processor family identifiers into a list of corresponding friendly names.
    /// </summary>
    /// <returns>A list of friendly names corresponding to the processor family identifiers,
    /// or null if the input list is empty or null.</returns>
    public List<string?>? GetProcessorFamilyFriendlyValue()
    {
        var list = GetProcessorFamily()
            ?.Select(MapProcessorFamilyToFriendlyValue)
            .Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps a single processor family identifier to its friendly name.
    /// </summary>
    /// <param name="familyValue">The family identifier of the processor.</param>
    /// <returns>The friendly name corresponding to the processor family identifier, or null if there is no defined mapping.</returns>
    public string? MapProcessorFamilyToFriendlyValue(ushort familyValue)
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
    /// <remarks>
    /// It is unclear what is returned from ManagementDateTimeConverter.ToDateTime when an incorrect/malformed value is passed as it never returns null.
    /// Due to this, the returned reference type DateTime is non-nullable.
    /// </remarks>
    /// <returns>A list of DateTime values representing the InstallDate of each processor, or null if unable to retrieve.</returns>
    public List<DateTime>? GetProcessorInstallDate()
    {
        // IProfiler should be responsible for providing access to the correct Profiler
        // Disable compile time warning.

        var list = GetWmiObject(
            "InstallDate",
            "Win32_Processor",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(Convert.ToString(value)))
            .ToList();
        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the L2CacheSize for each processor in kilobytes (KiB).
    /// </summary>
    /// <returns>A list of uint values representing the L2CacheSize in kilobytes (KiB) of each processor, or null if unable to retrieve.</returns>
    public List<uint>? GetProcessorL2CacheSize()
    {
        var list = GetWmiObject(
            "L2CacheSize",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "L2CacheSpeed",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "L3CacheSize",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
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
        var list = GetWmiObject(
            "L3CacheSpeed",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the last error code reported by the logical device.
    /// </summary>
    /// <returns>
    /// The last error code reported by the logical device as a uint value,
    /// or null if unable to retrieve the information.
    /// </returns>
    public List<uint>? GetProcessorLastErrorCode()
    {
        var list = GetWmiObject(
            "LastErrorCode",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the definition of the processor type.
    /// </summary>
    /// <returns>A list of ushort values representing the processor type levels, or null if an error occurs or the platform is not Windows.</returns>
    public List<ushort>? GetProcessorLevel()
    {
        var list = GetWmiObject(
            "Level",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the load capacity of each processor, averaged to the last second.
    /// </summary>
    /// <returns>A list of ushort values representing the load percentages of each processor, or null if an error occurs or the platform is not Windows.</returns>
    public List<ushort>? GetProcessorLoadPercentage()
    {
        var list = GetWmiObject(
            "LoadPercentage",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the name of the processor manufacturer.
    /// </summary>
    /// <returns>A list of strings representing the processor manufacturers, or null if an error occurs or the platform is not Windows.</returns>
    public List<string?>? GetProcessorManufacturer()
    {
        var list = GetWmiObject(
            "Manufacturer",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the maximum speed of the processor in megahertz (MHz).
    /// </summary>
    /// <returns>A list of uint values representing the maximum speeds of the processors in megahertz (MHz), or null if none are found.</returns>
    public List<uint>? GetProcessorMaxClockSpeed()
    {
        var list = GetWmiObject(
            "MaxClockSpeed",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the name of the processor model.
    /// </summary>
    /// <returns>A list of string values representing the processor model, or null if none is found.</returns>
    public List<string?>? GetProcessorName()
    {
        var list = GetWmiObject(
            "Name",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the number of physical processors cores within the processor.
    /// </summary>
    /// <returns>A list of integer values representing the phyisical core count of the processor, or null if none are found.</returns>
    public List<uint>? GetProcessorNumberOfCores()
    {
        var list = GetWmiObject(
            "NumberOfCores",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the number of enabled physical processing cores within the processor.
    /// </summary>
    /// <returns>A list of integer values which represent the amount of enabled physcial processor cores, or null if none are found.</returns>
    public List<uint>? GetProcessorNumberOfEnabledCore()
    {
        var list = GetWmiObject(
            "NumberOfEnabledCore",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the number of physical processing cores within the processor. (threads)
    /// </summary>
    /// <returns>A list of integer values which represent the amount of logical processing cores, or null if none are found.</returns>
    public List<uint>? GetProcessorNumberOfLogicalProcessors()
    {
        var list = GetWmiObject(
            "NumberOfLogicalProcessors",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the part number from the processor.
    /// </summary>
    /// <returns>A list of strings representing the part number of the processor, or null if none is found.</returns>
    public List<string?>? GetProcessorPartNumber()
    {
        var list = GetWmiObject(
            "PartNumber",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the PnP (Plug and Play) device ID of the processor.
    /// </summary>
    /// <returns>A list of string values which represent the PnP Device ID of the processor, or null if none is found.</returns>
    public List<string?>? GetProcessorPNPDeviceID()
    {
        var list = GetWmiObject(
            "PNPDeviceID",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the power managerment capabilties of the processor.
    /// </summary>
    /// <returns>A list of an array of short integers which represent the power managerment capabilties of the processor, or null if non is found.</returns>
    public List<ushort[]?>? GetProcessorPowerManagementCapabilities()
    {
        var list = GetWmiObject(
            "PowerManagementCapabilities",
            "Win32_Processor",
            value => value as ushort[]
        )
            ?.Where(value => value != null && value.Length > 0)
            .ToList();

        return list?.Any() == true ? list : null;
    }

    /// <summary>
    /// Retrieves a the friendly (mapped) values of the processors power management capabilities.
    /// </summary>
    /// <returns>A list of a list of strings which represent the friendly values of the processors power management capabilties, or null if none is found.</returns>
    public List<List<string?>?>? GetProcessorPowerManagementCapabilitiesFriendlyValue()
    {
        var list = GetProcessorPowerManagementCapabilities()
            ?.Select(capability =>
            {
                var friendlyValues = capability
                    ?.Select(value => MapPowerManagementCapabilitiesToFriendlyValue(value))
                    .Where(value => value != null)
                    .ToList();

                return friendlyValues != null && friendlyValues.Any() ? friendlyValues : null;
            })
            .Where(value => value != null)
            .ToList();

        return list?.Any() == true ? list : null;
    }

    /// <summary>
    /// Maps a value from PowerManagementCapabilities to its human readable name.
    /// </summary>
    /// <param name="pwrMngmtCapabiltity">The string representation of a power management capability, or null if mapping fails.</param>
    /// <returns></returns>
    public string? MapPowerManagementCapabilitiesToFriendlyValue(ushort pwrMngmtCapabiltity)
    {
        var mapping = new Dictionary<ushort, string>
        {
            { 0, "Unknown" },
            { 1, "Not Supported" },
            { 2, "Disabled" },
            { 3, "Enabled" },
            { 4, "Power Saving Modes Entered Automatically" },
            { 5, "Power State Settable" },
            { 6, "Power Cycling Supported" },
            { 7, "Timed Power On Supported" }
        };

        return mapping.TryGetValue(pwrMngmtCapabiltity, out string? friendlyValue)
            ? friendlyValue
            : null;
    }

    /// <summary>
    /// Retrieves the status of power management support for the processor.
    /// </summary>
    /// <returns>A list of boolean values which represent whether or not power managerment is supported on the processor, or null if none is found.</returns>
    public List<bool>? GetProcessorPowerManagementSupported()
    {
        var list = GetWmiObject(
            "PowerManagementSupported",
            "Win32_Processor",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the ID of the processor.
    /// </summary>
    /// <returns>A list of string values representing the ID of the processor, or null if none is found.</returns>
    public List<string?>? GetProcessorProcessorId()
    {
        var list = GetWmiObject(
            "ProcessorId",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retreives the processor type.
    /// </summary>
    /// <returns>A list of short integers which represnt the type of processor, or null, if none is found.</returns>
    public List<ushort>? GetProcessorProcessorType()
    {
        var list = GetWmiObject(
            "ProcessorType",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrives the friendly (mapped) values of the processor type.
    /// </summary>
    /// <returns>A list of strings which represent the human readable processor type.</returns>
    public List<string?>? GetProcessorProcessorTypeFriendlyValue()
    {
        var list = GetProcessorProcessorType()
            ?.Select(MapProcessorTypeToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps a short integer to its human readable string representation.
    /// </summary>
    /// <param name="type">The string representation of the processor type, or null if mapping fails.</param>
    /// <returns></returns>
    public string? MapProcessorTypeToFriendlyValue(ushort type)
    {
        return type switch
        {
            1 => "Other",
            2 => "Unknown",
            3 => "Central Processor",
            4 => "Math Processor",
            5 => "DSP Processor",
            6 => "Video Processor",
            _ => null,
        };
    }

    /// <summary>
    /// Retrieves the processor revision.
    /// </summary>
    /// <returns>The short integer representation of the processor revision, or null if none is found.</returns>
    public List<ushort>? GetProcessorRevision()
    {
        var list = GetWmiObject(
            "Revision",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the role of the processor.
    /// </summary>
    /// <returns>A list of strings representing the role of the processor, or null if none is found.</returns>
    public List<string?>? GetProcessorRole()
    {
        var list = GetWmiObject(
            "Role",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the staus of second level address translation exentensions of the processor.
    /// </summary>
    /// <returns>A list of booleans which represent the status support for second level address translation exentensions, or null if none is found.</returns>
    public List<bool>? GetProcessorSecondLevelAddressTranslationExtensions()
    {
        var list = GetWmiObject(
            "SecondLevelAddressTranslationExtensions",
            "Win32_Processor",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the processor serial number.
    /// </summary>
    /// <returns>A list of strings representing the processor serial number, or null if none is found.</returns>
    public List<string?>? GetProcessorSerialNumber()
    {
        var list = GetWmiObject(
            "SerialNumber",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the processor socket designation.
    /// </summary>
    /// <returns>A list of strings representing the processor socket designation, or null if none is found.</returns>
    public List<string?>? GetProcessorSocketDesignation()
    {
        var list = GetWmiObject(
            "SocketDesignation",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the status of the processor.
    /// </summary>
    /// <returns>A list of strings representing the status of the processor, or null if none is found.</returns>
    public List<string?>? GetProcessorStatus()
    {
        var list = GetWmiObject(
            "Status",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();
        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the status info of the processor.
    /// </summary>
    /// <returns>A list of short integers representing the status info of the processor, or null if none is found.</returns>
    public List<ushort>? GetProcessorStatusInfo()
    {
        var list = GetWmiObject(
            "StatusInfo",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            ?.Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the friendly (mapped) value of the processor status info.
    /// </summary>
    /// <returns>A list of strings which represent the human readable status info, or null if none is found.</returns>
    public List<string?>? GetProcessorStatusInfoFriendlyValue()
    {
        var list = GetProcessorStatusInfo()
            ?.Select(MapProcessorStatusInfoToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps a short integer to its human readable string representation.
    /// </summary>
    /// <param name="statusInfo">The short integer to convert to the human readable status info.</param>
    /// <returns>A string representing of the status info, or null if mapping fails.</returns>
    public string? MapProcessorStatusInfoToFriendlyValue(ushort statusInfo)
    {
        return statusInfo switch
        {
            1 => "Other",
            2 => "Unknown",
            3 => "Enabled",
            4 => "Disabled",
            5 => "Not Applicable",
            _ => null,
        };
    }

    /// <summary>
    /// Retrieves the processor stepping.
    /// </summary>
    /// <returns>A list of strings representing the procesor stepping.</returns>
    public List<string?>? GetProcessorStepping()
    {
        var list = GetWmiObject(
            "Stepping",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the system creation class name of the processor.
    /// </summary>
    /// <returns>A list of strings representing the system creation class name, or null if none is found.</returns>
    public List<string?>? GetProcessorSystemCreationClassName()
    {
        var list = GetWmiObject(
            "SystemCreationClassName",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the system name of the processor.
    /// </summary>
    /// <returns>A list of strings representing the system name, or null if none is found.</returns>
    public List<string?>? GetProcessorSystemName()
    {
        var list = GetWmiObject(
            "SystemName",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retieves the prcessor thread count (logical cores).
    /// </summary>
    /// <returns>
    /// A list of integers which represent the number of logical cores, or null if none is found.
    /// </returns>
    public List<uint>? GetProcessorThreadCount()
    {
        var list = GetWmiObject(
            "ThreadCount",
            "Win32_Processor",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the unique ID of the processor.
    /// </summary>
    /// <returns>A list of strings representing the unique ID, or null if none is found.</returns>
    public List<string?>? GetProcessorUniqueID()
    {
        var list = GetWmiObject(
            "UniqueId",
            "Win32_Processor",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the upgrade method of the processor.
    /// </summary>
    /// <returns>A list containin the upgrade method of the processor, or null if none is found.</returns>
    public List<ushort>? GetProcessorUpgradeMethod()
    {
        var list = GetWmiObject(
            "UpgradeMethod",
            "Win32_Processor",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Retrieves the friendly (mapped) value of the processor upgrade method.
    /// </summary>
    /// <returns>A list of strings repersenting the human readable processer upgrade method, or null if none is found.</returns>
    public List<string?>? GetProcessorUpgradeMethodFriendlyValue()
    {
        var list = GetProcessorUpgradeMethod()
            ?.Select(MapProcessorUpgradeMethodToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /// <summary>
    /// Maps a short integer to its human readable string representation.
    /// </summary>
    /// <param name="upgradeMethod">The uprade method to map.</param>
    /// <returns>The string representaion of the processer upgrade method, or null if none is found.</returns>
    public string? MapProcessorUpgradeMethodToFriendlyValue(ushort upgradeMethod)
    {
        return upgradeMethod switch
        {
            1 => "Other",
            2 => "Unknown",
            3 => "Daughter Board",
            4 => "ZIF Socket",
            5 => "Replacement/Piggy Back",
            6 => "None",
            7 => "LIF Socket",
            8 => "Slot 1",
            9 => "Slot 2",
            10 => "370 Pin Socket",
            11 => "Slot A",
            12 => "Slot M",
            13 => "Socket 423",
            14 => "Socket A (Socket 462)",
            15 => "Socket 478",
            16 => "Socket 754",
            17 => "Socket 940",
            18 => "Socket 939",
            _ => null,
        };
    }

    /*
    * -------------------------------------------------------------------------------------------------------
    * ---- MEMORY INFORMATION METHODS
    * -------------------------------------------------------------------------------------------------------
    *
    * Win32_PhysicalMemory
    * https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-physicalmemory
    *
    */

    public List<uint>? GetMemoryAttributes()
    {
        var list = GetWmiObject(
            "Attributes",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryBankLabel()
    {
        var list = GetWmiObject(
            "BankLabel",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetMemoryCapacity()
    {
        var list = GetWmiObject(
            "Capacity",
            "Win32_PhysicalMemory",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryCaption()
    {
        var list = GetWmiObject(
            "Caption",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemoryConfiguredClockSpeed()
    {
        var list = GetWmiObject(
            "ConfiguredClockSpeed",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemoryConfiguredVoltage()
    {
        var list = GetWmiObject(
            "ConfiguredVoltage",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryCreationClassName()
    {
        var list = GetWmiObject(
            "CreationClassName",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetMemoryDataWidth()
    {
        var list = GetWmiObject(
            "DataWidth",
            "Win32_PhysicalMemory",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryDescription()
    {
        var list = GetWmiObject(
            "Description",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryDeviceLocator()
    {
        var list = GetWmiObject(
            "DeviceLocator",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetMemoryFormFactor()
    {
        var list = GetWmiObject(
            "FormFactor",
            "Win32_PhysicalMemory",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryFormFactorFriendlyValue()
    {
        var list = GetMemoryFormFactor()
            ?.Select(MapMemoryFormFactorToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public string? MapMemoryFormFactorToFriendlyValue(ushort formFactor)
    {
        return formFactor switch
        {
            0 => "Unknown",
            1 => "Other",
            2 => "SIP",
            3 => "DIP",
            4 => "ZIP",
            5 => "SOJ",
            6 => "Proprietary",
            7 => "SIMM",
            8 => "DIMM",
            9 => "TSOP",
            10 => "PGA",
            11 => "RIMM",
            12 => "SODIMM",
            13 => "SRIMM",
            14 => "SMD",
            15 => "SSMP",
            16 => "QFP",
            17 => "TQFP",
            18 => "SOIC",
            19 => "LCC",
            20 => "PLCC",
            21 => "BGA",
            22 => "FPBGA",
            23 => "LGA",
            _ => null,
        };
    }

    public List<bool>? GetMemoryHotSwappable()
    {
        var list = GetWmiObject(
            "HotSwappable",
            "Win32_PhysicalMemory",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<DateTime>? GetMemoryInstallDate()
    {
        var list = GetWmiObject(
            "InstallDate",
            "Win32_PhysicalMemory",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(value!.ToString()))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetMemoryInterleaveDataDepth()
    {
        var list = GetWmiObject(
            "InterleaveDataDepth",
            "Win32_PhysicalMemory",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemoryInterleavePosition()
    {
        var list = GetWmiObject(
            "InterleavePosition",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryManufacturer()
    {
        var list = GetWmiObject(
            "Manufacturer",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemoryMaxVoltage()
    {
        var list = GetWmiObject(
            "MaxVoltage",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetMemoryMemoryType()
    {
        var list = GetWmiObject(
            "Type",
            "Win32_PhysicalMemory",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryTypeFriendlyValue()
    {
        var list = GetMemoryMemoryType()
            ?.Select(MapMemoryTypeToFriendlyValue)
            .Where(value => value != null)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public string? MapMemoryTypeToFriendlyValue(ushort type)
    {
        return type switch
        {
            0 => "Unknown",
            1 => "Other",
            2 => "DRAM",
            3 => "Synchronous DRAM",
            4 => "Cache DRAM",
            5 => "EDO",
            6 => "EDRAM",
            7 => "VRAM",
            8 => "SRAM",
            9 => "RAM",
            10 => "ROM",
            11 => "Flash",
            12 => "EEPROM",
            13 => "FEPROM",
            14 => "EPROM",
            15 => "CDRAM",
            16 => "3DRAM",
            17 => "SDRAM",
            18 => "SGRAM",
            19 => "RDRAM",
            20 => "DDR",
            21 => "DDR2",
            22 => "DDR2 FB-DIMM",
            // 23 => MSDN contains no definition
            24 => "DDR3",
            25 => "FBD2",
            26 => "DDR4",
            _ => null,
        };
    }

    public List<uint>? GetMemoryMinimumVoltage()
    {
        var list = GetWmiObject(
            "MinimumVoltage",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryModel()
    {
        var list = GetWmiObject(
            "Model",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryName()
    {
        var list = GetWmiObject(
            "Name",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryOtherIdentifyingInfo()
    {
        var list = GetWmiObject(
            "OtherIdentifyingInfo",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryPartNumber()
    {
        var list = GetWmiObject(
            "PartNumber",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemoryPositionInRow()
    {
        var list = GetWmiObject(
            "PositionInRow",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMemoryPoweredOn()
    {
        var list = GetWmiObject(
            "PoweredOn",
            "Win32_PhysicalMemory",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMemoryRemovable()
    {
        var list = GetWmiObject(
            "Removable",
            "Win32_PhysicalMemory",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMemoryReplaceable()
    {
        var list = GetWmiObject(
            "Replaceable",
            "Win32_PhysicalMemory",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemorySerialNumber()
    {
        var list = GetWmiObject(
            "SerialNumber",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemorySKU()
    {
        var list = GetWmiObject(
            "SKU",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemorySMBIOSMemoryType()
    {
        var list = GetWmiObject(
            "SMBIOSMemoryType",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetMemorySpeed()
    {
        var list = GetWmiObject(
            "Speed",
            "Win32_PhysicalMemory",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryStatus()
    {
        var list = GetWmiObject(
            "Status",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMemoryTag()
    {
        var list = GetWmiObject(
            "Tag",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetMemoryTotalWidth()
    {
        var list = GetWmiObject(
            "TotalWidth",
            "Win32_PhysicalMemory",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort> GetMemoryTypeDetail()
    {
        var list = GetWmiObject(
            "TypeDetail",
            "Win32_PhysicalMemory",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null!;
    }

    public List<string?>? GetMemoryVersion()
    {
        var list = GetWmiObject(
            "Version",
            "Win32_PhysicalMemory",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /*
    * -------------------------------------------------------------------------------------------------------
    * ---- STORAGE INFORMATION METHODS
    * -------------------------------------------------------------------------------------------------------
    *
    * Win32_DiskDrive
    * https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-diskdrive
    *
    */

    public List<ushort>? GetStorageAvailability()
    {
        var list = GetWmiObject(
            "Availability",
            "Win32_DiskDrive",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageBytesPerSector()
    {
        var list = GetWmiObject(
            "BytesPerSector",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort[]?>? GetStorageCapabilities()
    {
        var list = GetWmiObject("Capabilities", "Win32_DiskDrive", value => value as ushort[])
            ?.Where(value => value != null && value.Length > 0)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string[]?>? GetStorageCapabilityDescriptions()
    {
        var list = GetWmiObject(
            "CapabilityDescriptions",
            "Win32_DiskDrive",
            value => value as string[]
        )
            ?.Where(value => value != null && value.Length > 0)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageCaption()
    {
        var list = GetWmiObject(
            "Caption",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageCompressionMethod()
    {
        var list = GetWmiObject(
            "CompressionMethod",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageConfigManagerErrorCode()
    {
        var list = GetWmiObject(
            "ConfigManagerErrorCode",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetStorageConfigManagerUserConfig()
    {
        var list = GetWmiObject(
            "ConfigManagerUserConfig",
            "Win32_DiskDrive",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageCreationClassName()
    {
        var list = GetWmiObject(
            "CreationClassName",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageDefaultBlockSize()
    {
        var list = GetWmiObject(
            "DefaultBlockSize",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageDescription()
    {
        var list = GetWmiObject(
            "Description",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageDeviceID()
    {
        var list = GetWmiObject(
            "DeviceID",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetStorageErrorCleared()
    {
        var list = GetWmiObject(
            "ErrorCleared",
            "Win32_DiskDrive",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageErrorDescription()
    {
        var list = GetWmiObject(
            "ErrorDescription",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageErrorMethodology()
    {
        var list = GetWmiObject(
            "ErrorMethodology",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageFirmwareRevision()
    {
        var list = GetWmiObject(
            "FirmwareRevision",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageIndex()
    {
        var list = GetWmiObject(
            "Index",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<DateTime>? GetStorageInstallDate()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        var list = GetWmiObject(
            "InstallDate",
            "Win32_DiskDrive",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(value!.ToString()))
            .ToList();
#pragma warning restore CA1416 // Validate platform compatibility
        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageInterfaceType()
    {
        var list = GetWmiObject(
            "InterfaceType",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageLastErrorCode()
    {
        var list = GetWmiObject(
            "LastErrorCode",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageManufacturer()
    {
        var list = GetWmiObject(
            "Manufacturer",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageMaxBlockSize()
    {
        var list = GetWmiObject(
            "MaxBlockSize",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageMaxMediaSize()
    {
        var list = GetWmiObject(
            "MaxMediaSize",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetStorageMediaLoaded()
    {
        var list = GetWmiObject(
            "MediaLoaded",
            "Win32_DiskDrive",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageMediaType()
    {
        var list = GetWmiObject(
            "MediaType",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageMinBlockSize()
    {
        var list = GetWmiObject(
            "MinBlockSize",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageModel()
    {
        var list = GetWmiObject(
            "Model",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageName()
    {
        var list = GetWmiObject(
            "Name",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetStorageNeedsCleaning()
    {
        var list = GetWmiObject(
            "NeedsCleaning",
            "Win32_DiskDrive",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageNumberOfMediaSupported()
    {
        var list = GetWmiObject(
            "NumberOfMediaSupported",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStoragePartitions()
    {
        var list = GetWmiObject(
            "Partitions",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStoragePNPDeviceID()
    {
        var list = GetWmiObject(
            "PNPDeviceID",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort[]?>? GetStoragePowerManagementCapabilities()
    {
        var list = GetWmiObject(
            "PowerManagementCapabilities",
            "Win32_DiskDrive",
            value => value as ushort[]
        )
            ?.Where(value => value != null && value.Length > 0)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetStoragePowerManagementSupported()
    {
        var list = GetWmiObject(
            "PowerManagementSupported",
            "Win32_DiskDrive",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageSCSIBus()
    {
        var list = GetWmiObject(
            "SCSIBus",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetStorageSCSILogicalUnit()
    {
        var list = GetWmiObject(
            "SCSILogicalUnit",
            "Win32_DiskDrive",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetStorageSCSIPort()
    {
        var list = GetWmiObject(
            "SCSIPort",
            "Win32_DiskDrive",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetStorageSCSITargetId()
    {
        var list = GetWmiObject(
            "SCSITargetId",
            "Win32_DiskDrive",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageSectorsPerTrack()
    {
        var list = GetWmiObject(
            "SectorsPerTrack",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageSerialNumber()
    {
        var list = GetWmiObject(
            "SerialNumber",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageSignature()
    {
        var list = GetWmiObject(
            "Signature",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageSize()
    {
        var list = GetWmiObject(
            "Size",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageStatus()
    {
        var list = GetWmiObject(
            "Status",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetStorageStatusInfo()
    {
        var list = GetWmiObject(
            "StatusInfo",
            "Win32_DiskDrive",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageSystemCreationClassName()
    {
        var list = GetWmiObject(
            "SystemCreationClassName",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetStorageSystemName()
    {
        var list = GetWmiObject(
            "SystemName",
            "Win32_DiskDrive",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageTotalCylinders()
    {
        var list = GetWmiObject(
            "TotalCylinders",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageTotalHeads()
    {
        var list = GetWmiObject(
            "TotalHeads",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageTotalSectors()
    {
        var list = GetWmiObject(
            "TotalSectors",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetStorageTotalTracks()
    {
        var list = GetWmiObject(
            "TotalTracks",
            "Win32_DiskDrive",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetStorageTracksPerCylinder()
    {
        var list = GetWmiObject(
            "TracksPerCylinder",
            "Win32_DiskDrive",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /*
    * -------------------------------------------------------------------------------------------------------
    * ---- MOTHERBOARD INFORMATION METHODS
    * -------------------------------------------------------------------------------------------------------
    *
    * Win32_BaseBoard
    * https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-baseboard
    *
    */

    public List<string?>? GetMotherboardCaption()
    {
        var list = GetWmiObject(
            "Caption",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string[]?>? GetMotherboardConfigOptions()
    {
        var list = GetWmiObject("ConfigOptions", "Win32_BaseBoard", value => value as string[])
            ?.Where(value => value != null && value.Length > 0)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardCreationClassName()
    {
        var list = GetWmiObject(
            "CreationClassName",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<float>? GetMotherboardDepth()
    {
        var list = GetWmiObject(
            "Depth",
            "Win32_BaseBoard",
            value => value != null ? (float?)Convert.ToSingle(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardDescription()
    {
        var list = GetWmiObject(
            "Description",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<float>? GetMotherboardHeight()
    {
        var list = GetWmiObject(
            "Height",
            "Win32_BaseBoard",
            value => value != null ? (float?)Convert.ToSingle(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardHostingBoard()
    {
        var list = GetWmiObject(
            "HostingBoard",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardHotSwappable()
    {
        var list = GetWmiObject(
            "HotSwappable",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<DateTime>? GetMotherboardInstallDate()
    {
        var list = GetWmiObject(
            "InstallDate",
            "Win32_BaseBoard",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(value!.ToString()))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardManufacturer()
    {
        var list = GetWmiObject(
            "Manufacturer",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardModel()
    {
        var list = GetWmiObject(
            "Model",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardName()
    {
        var list = GetWmiObject(
            "Name",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardOtherIdentifyingInfo()
    {
        var list = GetWmiObject(
            "OtherIdentifyingInfo",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardPartNumber()
    {
        var list = GetWmiObject(
            "PartNumber",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardPoweredOn()
    {
        var list = GetWmiObject(
            "PoweredOn",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardProduct()
    {
        var list = GetWmiObject(
            "Product",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardRemovable()
    {
        var list = GetWmiObject(
            "Removable",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardReplaceable()
    {
        var list = GetWmiObject(
            "Replaceable",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardRequirementsDescription()
    {
        var list = GetWmiObject(
            "RequirementsDescription",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardRequiresDaughterBoard()
    {
        var list = GetWmiObject(
            "RequiresDaughterBoard",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardSerialNumber()
    {
        var list = GetWmiObject(
            "SerialNumber",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardSKU()
    {
        var list = GetWmiObject(
            "SKU",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardSlotLayout()
    {
        var list = GetWmiObject(
            "SlotLayout",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetMotherboardSpecialRequirements()
    {
        var list = GetWmiObject(
            "SpecialRequirements",
            "Win32_BaseBoard",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardStatus()
    {
        var list = GetWmiObject(
            "Status",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardTag()
    {
        var list = GetWmiObject(
            "Tag",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetMotherboardVersion()
    {
        var list = GetWmiObject(
            "Version",
            "Win32_BaseBoard",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<float>? GetMotherboardWeight()
    {
        var list = GetWmiObject(
            "Weight",
            "Win32_BaseBoard",
            value => value != null ? (float?)Convert.ToSingle(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<float>? GetMotherboardWidth()
    {
        var list = GetWmiObject(
            "Width",
            "Win32_BaseBoard",
            value => value != null ? (float?)Convert.ToSingle(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    /*
    * -------------------------------------------------------------------------------------------------------
    * ---- OPERATING SYSTEM INFORMATION METHODS
    * -------------------------------------------------------------------------------------------------------
    *
    * Win32_OperatingSystem
    * https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-operatingsystem
    *
    */

    public List<string?>? GetOsBootDevice()
    {
        var list = GetWmiObject(
            "BootDevice",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsBuildNumber()
    {
        var list = GetWmiObject(
            "BuildNumber",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsBuildType()
    {
        var list = GetWmiObject(
            "BuildType",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCaption()
    {
        var list = GetWmiObject(
            "Caption",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCodeSet()
    {
        var list = GetWmiObject(
            "CodeSet",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCountryCode()
    {
        var list = GetWmiObject(
            "CountryCode",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCreationClassName()
    {
        var list = GetWmiObject(
            "CreationClassName",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCSCreationClassName()
    {
        var list = GetWmiObject(
            "CSCreationClassName",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCSDVersion()
    {
        var list = GetWmiObject(
            "CSDVersion",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsCSName()
    {
        var list = GetWmiObject(
            "CSName",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<short>? GetOsCurrentTimeZone()
    {
        var list = GetWmiObject(
            "CurrentTimeZone",
            "Win32_OperatingSystem",
            value => value != null ? (short?)Convert.ToInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsDataExecutionPrevention_Available()
    {
        var list = GetWmiObject(
            "DataExecutionPrevention_Available",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsDataExecutionPrevention_32BitApplications()
    {
        var list = GetWmiObject(
            "DataExecutionPrevention_32BitApplications",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsDataExecutionPrevention_Drivers()
    {
        var list = GetWmiObject(
            "DataExecutionPrevention_Drivers",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<byte>? GetOsDataExecutionPrevention_SupportPolicy()
    {
        var list = GetWmiObject(
            "DataExecutionPrevention_SupportPolicy",
            "Win32_OperatingSystem",
            value => value != null ? (byte?)Convert.ToByte(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsDebug()
    {
        var list = GetWmiObject(
            "Debug",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsDescription()
    {
        var list = GetWmiObject(
            "Description",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsDistributed()
    {
        var list = GetWmiObject(
            "Distributed",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsEncryptionLevel()
    {
        var list = GetWmiObject(
            "EncryptionLevel",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<byte>? GetOsForegroundApplicationBoost()
    {
        // ForegroundApplicationBoost contains default value, from docs:
        // uint8 ForegroundApplicationBoost = 2;

        var list = GetWmiObject(
            "ForegroundApplicationBoost",
            "Win32_OperatingSystem",
            value => value != null ? (byte?)Convert.ToByte(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsFreePhysicalMemory()
    {
        var list = GetWmiObject(
            "FreePhysicalMemory",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsFreeSpaceInPagingFiles()
    {
        var list = GetWmiObject(
            "FreeSpaceInPagingFiles",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsFreeVirtualMemory()
    {
        var list = GetWmiObject(
            "FreeVirtualMemory",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<DateTime>? GetOsInstallDate()
    {
        var list = GetWmiObject(
            "InstallDate",
            "Win32_OperatingSystem",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(value!.ToString()))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsLargeSystemCache()
    {
        var list = GetWmiObject(
            "LargeSystemCache",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<DateTime>? GetOsLastBootUpTime()
    {
        var list = GetWmiObject(
            "LastBootUpTime",
            "Win32_OperatingSystem",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(value!.ToString()))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<DateTime>? GetOsLocalDateTime()
    {
        var list = GetWmiObject(
            "LocalDateTime",
            "Win32_OperatingSystem",
            value => value != null ? value : null
        )
            ?.Where(value => value != null)
            .Select(value => ManagementDateTimeConverter.ToDateTime(value!.ToString()))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsLocale()
    {
        var list = GetWmiObject(
            "Locale",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsManufacturer()
    {
        var list = GetWmiObject(
            "Manufacturer",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsMaxNumberOfProcesses()
    {
        var list = GetWmiObject(
            "MaxNumberOfProcesses",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsMaxProcessMemorySize()
    {
        var list = GetWmiObject(
            "MaxProcessMemorySize",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string[]?>? GetOsMUILanguages()
    {
        var list = GetWmiObject("MUILanguages", "Win32_OperatingSystem", value => value as string[])
            ?.Where(value => value != null && value.Length > 0)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsName()
    {
        var list = GetWmiObject(
            "Name",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsNumberOfLicensedUsers()
    {
        var list = GetWmiObject(
            "NumberOfLicensedUsers",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsNumberOfProcesses()
    {
        var list = GetWmiObject(
            "NumberOfProcesses",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsNumberOfUsers()
    {
        var list = GetWmiObject(
            "NumberOfUsers",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsOperatingSystemSKU()
    {
        var list = GetWmiObject(
            "OperatingSystemSKU",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsOrganization()
    {
        var list = GetWmiObject(
            "Organization",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsOSArchitecture()
    {
        var list = GetWmiObject(
            "OSArchitecture",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsOSLanguage()
    {
        var list = GetWmiObject(
            "OSLanguage",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsOSProductSuite()
    {
        var list = GetWmiObject(
            "OSProductSuite",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetOsOSType()
    {
        var list = GetWmiObject(
            "OSType",
            "Win32_OperatingSystem",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsOtherTypeDescription()
    {
        var list = GetWmiObject(
            "OtherTypeDescription",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsPAEEnabled()
    {
        var list = GetWmiObject(
            "PAEEnabled",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsPlusProductID()
    {
        var list = GetWmiObject(
            "PlusProductID",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsPlusVersionNumber()
    {
        var list = GetWmiObject(
            "PlusVersionNumber",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsPortableOperatingSystem()
    {
        var list = GetWmiObject(
            "PortableOperatingSystem",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<bool>? GetOsPrimary()
    {
        var list = GetWmiObject(
            "Primary",
            "Win32_OperatingSystem",
            value => value != null ? (bool?)Convert.ToBoolean(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsProductType()
    {
        var list = GetWmiObject(
            "ProductType",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsRegisteredUser()
    {
        var list = GetWmiObject(
            "RegisteredUser",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsSerialNumber()
    {
        var list = GetWmiObject(
            "SerialNumber",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetOsServicePackMajorVersion()
    {
        var list = GetWmiObject(
            "ServicePackMajorVersion",
            "Win32_OperatingSystem",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ushort>? GetOsServicePackMinorVersion()
    {
        var list = GetWmiObject(
            "ServicePackMinorVersion",
            "Win32_OperatingSystem",
            value => value != null ? (ushort?)Convert.ToUInt16(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsSizeStoredInPagingFiles()
    {
        var list = GetWmiObject(
            "SizeStoredInPagingFiles",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsStatus()
    {
        var list = GetWmiObject(
            "Status",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<uint>? GetOsSuiteMask()
    {
        var list = GetWmiObject(
            "SuiteMask",
            "Win32_OperatingSystem",
            value => value != null ? (uint?)Convert.ToUInt32(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsSystemDevice()
    {
        var list = GetWmiObject(
            "SystemDevice",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsSystemDirectory()
    {
        var list = GetWmiObject(
            "SystemDirectory",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsSystemDrive()
    {
        var list = GetWmiObject(
            "SystemDrive",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsTotalSwapSpaceSize()
    {
        var list = GetWmiObject(
            "TotalSwapSpaceSize",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsTotalVirtualMemorySize()
    {
        var list = GetWmiObject(
            "TotalVirtualMemorySize",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<ulong>? GetOsTotalVisibleMemorySize()
    {
        var list = GetWmiObject(
            "TotalVisibleMemorySize",
            "Win32_OperatingSystem",
            value => value != null ? (ulong?)Convert.ToUInt64(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsVersion()
    {
        var list = GetWmiObject(
            "Version",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<string?>? GetOsWindowsDirectory()
    {
        var list = GetWmiObject(
            "WindowsDirectory",
            "Win32_OperatingSystem",
            value => value != null ? Convert.ToString(value) : null
        )
            ?.Where(value => !string.IsNullOrEmpty(value))
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<byte>? GetOsQuantumLength()
    {
        var list = GetWmiObject(
            "QuantumLength",
            "Win32_OperatingSystem",
            value => value != null ? (byte?)Convert.ToByte(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }

    public List<byte>? GetOsQuantumType()
    {
        var list = GetWmiObject(
            "QuantumType",
            "Win32_OperatingSystem",
            value => value != null ? (byte?)Convert.ToByte(value) : null
        )
            ?.Where(value => value != null)
            .Select(value => value!.Value)
            .ToList();

        return list?.Count > 0 ? list : null;
    }
}
