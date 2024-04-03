namespace WTNS.Telemetry;

/// <summary>
/// Enforces the requirements for a host machine profiler.
///
/// The implementations can be found in TODODODODODODOD.
///
/// The profiler is responsible for collecting information about the host machine.
///
/// The profiler is used to collect information about the host machine, such as:
/// - Operating system information
/// - Motherboard information
/// - CPU information
/// - GPU information
/// - RAM information
/// - Storage information
/// - Network information
///
/// </summary>
public interface IProfiler
{
    public OsInfo GetOsInfo();

    /// <summary>
    /// Collects information relating to motherboard installed on the host machine.
    /// </summary>
    /// <returns>A structure containing motherboard related information.</returns>
    public MotherboardInfo GetMotherboardInfo();

    /// <summary>
    /// Collects information relating to CPU(s) installed on the host machine.
    /// </summary>
    /// <returns>A structure containing CPU related information.</returns>
    public CpuInfo GetProcessorInfo();

    /// <summary>
    /// Collects information relating to GPU(s) installed on the host machine.
    /// </summary>
    /// <returns>A structure containing GPU related information.</returns>
    public GpuInfo GetGpuInfo(); // Assuming there might be more than one GPU

    /// <summary>
    /// Collects information relating to RAM installed on the host machine.
    /// </summary>
    /// <returns>A structure containing RAM related information.</returns>
    public MemoryInfo GetMemoryInfo();

    /// <summary>
    /// Collects information relating to storage devices installed on the host machine.
    /// </summary>
    /// <returns>A structure containing storage related information.</returns>
    public StorageInfo GetStorageInfo(); // Assuming there might be more than one Disk

    /// <summary>
    /// Collects information relating to network devices installed on the host machine.
    /// </summary>
    /// <returns>A structure containing storage related information.</returns>
    public NetInfo GetNetInfo();
}
