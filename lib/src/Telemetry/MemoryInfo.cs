namespace WTNS.Telemetry;

public struct MemoryInfo
{
    /// <summary>
    /// The attributes of the physical memory. The attributes consists of a single integer value, denotinig the rank.
    /// </summary>
    /// <remarks>
    /// A memory rank is a block or area of data that is created using some, or all, of the memory chips on a module
    /// A rank is a data block that is 64 bits wide. On systems that support Error Correction Code (ECC) an additional
    /// 8 bits are added, which makes the data block 72 bits wide.
    /// Depending on how a memory module is engineered, it may have one, two, or four blocks of 64-bit wide data areas
    /// (or 72-bit wide in the case of ECC modules.)
    /// This is referred to as single-rank, dual-rank, and quad-rank.
    /// </remarks>
    public List<uint>? Attributes;
    public List<string?>? BankLabel;
    public List<ulong>? Capacity;
    public List<string?>? Caption;
    public List<uint>? ConfiguredClockSpeed;
    public List<uint>? ConfiguredVoltage;
    public List<string?>? CreationClassName;
    public List<ushort>? DataWidth;
    public List<string?>? Description;
    public List<string?>? DeviceLocator;
    public List<ushort>? FormFactor;
    public List<bool>? HotSwappable;
    public List<DateTime>? InstallDate;
    public List<ushort>? InterleaveDataDepth;
    public List<uint>? InterleavePosition;
    public List<string?>? Manufacturer;
    public List<uint>? MaxVoltage;
    public List<ushort>? MemoryType;
    public List<uint>? MinVoltage;
    public List<string?>? Model;
    public List<string?>? Name;
    public List<string?>? OtherIdentifyingInfo;
    public List<string?>? PartNumber;
    public List<uint>? PositionInRow;
    public List<bool>? PoweredOn;
    public List<bool>? Removable;
    public List<bool>? Replaceable;
    public List<bool>? SerialNumber;
    public List<string?>? SKU;
    public List<uint>? SMBIOSMemoryType;
    public List<uint>? Speed;
    public List<string?>? Status;
    public List<string?>? Tag;
    public List<ushort>? TotalWidth;
    public List<ushort>? TypeDetail;
    public List<string?>? Version;
}
