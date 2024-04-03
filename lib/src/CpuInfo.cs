namespace Wtns.Me.Lib;

public struct CpuInfo
{
    public List<ushort>? AddressWidth;
    public List<ushort>? Architecture;
    public List<string>? ArchitectureFriendlyValue;
    public List<string>? AssetTag;
    public List<ushort>? Availability;
    public List<string>? AvailabilityFriendlyValue;
    public List<uint>? Characteristics;
    public List<List<string>>? CharacteristicsFriendlyValue;
    public List<uint>? ConfigManagerErrorCode;
    public List<List<string>>? ConfigManagerErrorCodeFriendlyValue;
    public List<bool>? ConfigManagerUserConfig;

    // STOP HERE
    public List<ushort>? CpuStatus;
    public List<string>? CpuStatusFriendlyValue;
    public List<string>? CreationClassName;
    public List<uint>? CurrentClockSpeed;
    public List<ushort>? CurrentVoltage;
    public List<decimal>? CurrentVoltageFriendlyValue;
    public List<ushort>? DataWidth;
    public List<string>? Description;
    public List<string>? DeviceId;
    public List<bool>? ErrorCleared;
    public List<string>? ErrorDescription;
    public List<uint>? ExtClock;
    public List<ushort>? Family;
    public List<string>? FamilyFriendlyValue;
    public List<DateTime>? InstallDate;
    public List<uint>? L2CacheSize;
    public List<uint>? L2CacheSpeed;
    public List<uint>? L3CacheSize;
    public List<uint>? L3CacheSpeed;
    public List<uint>? LastErrorCode;

    /// <summary>
    /// The name used to identify the CPU.
    /// </summary>
    public List<string>? Name;

    /// <summary>
    /// The Id (also known as hardware Id) assigned to the processor. Not all configurations or cpus will report this value.
    /// These are considered 'Unassigned' Id's and should be handled gracefully.
    /// </summary>
    public List<string>? Id;

    /// <summary>
    /// <para>
    /// The CPU `Caption` string typically follows the standard structure:
    /// </para>
    /// <para>
    /// [Architecture] [Family] [Model] [Stepping] [Vendor ID] [Version] [Cache Size]
    /// </para>
    /// </summary>
    public List<string>? Caption;
    public List<string>? Manufacturer;

    public List<(int Physical, int Logical)>? CoreCount;
    public List<(int Level2, int Level3)>? CacheSizeInMb;
}
