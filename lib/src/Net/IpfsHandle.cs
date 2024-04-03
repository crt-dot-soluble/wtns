using Ipfs;
using WTNS.Cli;

namespace WTNS.Net;

/// <summary>
/// Uses an existing, locally running, IPFS daemon to complete a given request.
/// Acts as a wrapper for the <see cref="Ipfs"/> namespaces with additional abstraction
/// and utility functions.
/// <para>
/// Note: CID, or Content Identifier, is a uniue label that points to content on IPFS.
/// </para>
/// </summary>
public static class IpfsHandle
{
    /// <summary>
    /// Underlying IpfsClient Object, hidden to avoid redundency in code and code completions.
    /// </summary>
    private static Ipfs.Http.IpfsClient _ipfsClient = new();

    /// <summary>
    /// The default directory to store files from IPFS.
    /// </summary>
    public const string DefaultDownloadDirectory = "./ipfs/downloads/";

    /// <summary>
    /// Gets the Peer ID of the local IPFS daemon. If the Peer ID cannot be acquired (i.e. IPFS daemon is offline)
    /// or is otherwise equal to null, this function will return null. Use null checking, or a try catch block to avoid crashes
    /// where PeerId is equal to null.
    /// </summary>
    /// <returns>The peer ID, if one is found, otherwise null.</returns>
    public static string? GetPeerId()
    {
        try
        {
            var pid = _ipfsClient.IdAsync().Result.Id?.ToString();
            return pid;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null;
        }
    }

    /// <summary>
    /// Reinitializes the underlying <see cref="Ipfs.Http.IpfsClient"/> object using a new connection string. i.e. http://127.0.0.1:5001
    /// </summary>
    /// <param name="url">The connection URL to use (i.e. 'http://127.0.0.1:5001')</param>
    public static void SetConnectionUrl(string url = "http://127.0.0.1:5001")
    {
        try
        {
            _ipfsClient = new Ipfs.Http.IpfsClient(url);
            Repl.Log($"IPFS Connection URL set to {url}.");
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
        }
    }

    /// <summary>
    /// Adds a string of text to IPFS.
    /// </summary>
    /// <returns>The cid of the uploaded text.</returns>
    public static string AddText(string text)
    {
        // Add text to IPFS Client
        var hash = _ipfsClient.FileSystem.AddTextAsync(text);
        var output = new string[]
        {
            "IPFS AddText operation completed.",
            $"Source Text: {text}",
            $"Content ID (CID): {hash.Result.Id}"
        };
        Repl.Log(output);
        return hash.Result.Id;
    }

    /// <summary>
    /// Adds a file to IPFS.
    /// </summary>
    /// <param name="path">The path to the file to be added.</param>
    /// <returns></returns>
    public static string? AddFile(string path)
    {
        try
        {
            var cid = _ipfsClient.FileSystem.AddFileAsync(path).Result.Id;
            var output = new string[]
            {
                "IPFS AddFile operation completed.",
                $"Source File: {path}",
                $"Content ID (CID): {cid}"
            };

            Repl.Log(output);
            return cid;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null;
        }
    }

    /// <summary>
    /// Adds a directory to IPFS.
    /// </summary>
    /// <param name="path">The path to the directory to add.</param>
    /// <returns>The cid of the added Directory.</returns>
    public static string? AddDirectory(string path)
    {
        try
        {
            var cid = _ipfsClient.FileSystem.AddDirectoryAsync(path).Result.Id;

            var output = new string[]
            {
                "IPFS AddDirectory operation completed.",
                $"Source Directory: {path}",
                $"Content ID (CID): {cid}"
            };

            Repl.Log(output);
            return cid;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null;
        }
    }

    /// <summary>
    /// Downloads a file from IPFS as an archive (.tar) file.
    /// </summary>
    /// <param name="cid">The CID to use when retrieving the file.</param>
    /// <param name="name">What the emitted file will be named.</param>
    /// <param name="outputDir">The directory to save the file to.</param>
    /// <returns>True if the operation completed successfully, false otherwise.</returns>
    public static bool GetFile(string cid, string? name = null, string? outputDir = null)
    {
        name = (name == null) ? cid : name;
        outputDir = (outputDir == null) ? DefaultDownloadDirectory : outputDir;
        Directory.CreateDirectory(Path.Combine(outputDir));

        if (!IsOnline())
        {
            Repl.Log(
                "Null Peer ID - IPFS daemon is offline or otherwise unreachable.",
                false,
                StructuredOutput.OutputMode.WARNING
            );
            return false; // If Peer ID is null, IPFS is offline (or not running)
        }

        try
        {
            using (var s = _ipfsClient.FileSystem.GetAsync(cid).Result)
            {
                using (
                    var fs = new FileStream(
                        Path.Combine(outputDir, name + ".tar"),
                        FileMode.Create,
                        FileAccess.Write
                    )
                )
                {
                    s.CopyTo(fs);

                    var lines = new string[]
                    {
                        "IPFS GetFile operation completed.",
                        $"Source CID: {cid}",
                        $"Destination Directory: {Path.Combine(outputDir)}",
                        $"Destination File: {Path.Combine(name)}",
                    };

                    Repl.Log(lines, false, StructuredOutput.OutputMode.INFO);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return false;
        }
    }

    /// <summary>
    /// Gets Text data from IPFS.
    /// </summary>
    /// <param name="cid">The CID to use when retrieving the text.</param>
    /// <param name="name">The name of the file - defaults to a timestamp.</param>
    /// <param name="outputDir">The directory to save the file to.</param>
    /// <returns>The text read that was read from the file, otherwise null.</returns>
    public static string? GetText(string cid, string? name = null, string? outputDir = null)
    {
        name = (name == null) ? cid : name;
        outputDir = (outputDir == null) ? DefaultDownloadDirectory : outputDir;
        Directory.CreateDirectory(Path.Combine(outputDir));

        if (!IsOnline())
        {
            Repl.Log(
                "Null Peer ID - IPFS daemon is offline or otherwise unreachable.",
                false,
                StructuredOutput.OutputMode.WARNING
            );
            return null; // If Peer ID is null, IPFS is offline (or not running)
        }

        try
        {
            var text = _ipfsClient.FileSystem.ReadAllTextAsync(cid).Result;
            File.WriteAllText(Path.Combine(outputDir, name + ".txt"), text);
            var lines = new string[]
            {
                "IPFS GetText operation completed.",
                $"Source CID: {cid}",
                $"Destination Directory: {Path.Combine(outputDir)}",
                $"Destination File: {Path.Combine(name)}",
                $"Text: `{text}`"
            };
            Repl.Log(lines, false, StructuredOutput.OutputMode.INFO);
            return text;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null;
        }
    }

    /// <summary>
    /// Pins a given CID to the local IPFS daemon.
    /// </summary>
    /// <param name="cid">The CID of the file/directory you wish to pin.</param>
    /// <returns>The CID of the pinned content, if it was pinned. Otherwise, returns null.</returns>
    public static string? Pin(string cid)
    {
        try
        {
            _ipfsClient.Pin.AddAsync(cid).Wait();
            Repl.Log($"Successfully pinned CID: {cid}");
            return cid;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null;
        }
    }

    /// <summary>
    /// Gets and returns a list of all CIDs pinned to the local IPFS daemon.
    /// </summary>
    /// <returns>A List of CIDs pinned to the local IPFS daemon. </returns>
    public static IEnumerable<Cid>? GetAllPins()
    {
        try
        {
            var pins = _ipfsClient.Pin.ListAsync().Result;
            return pins;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null; // If Peer ID is null, IPFS is offline (or not running)
        }
    }

    /// <summary>
    /// Removes the pinned content for a given CID.
    /// </summary>
    /// <param name="cid">The CID to unpin.</param>
    /// <returns>The CID of content, if it was unpinned. Otherwise, returns null.</returns>
    public static string? Unpin(string cid)
    {
        try
        {
            _ipfsClient.Pin.RemoveAsync(cid).Wait();
            Repl.Log($"Successfully unpinned CID: {cid}");
            return cid;
        }
        catch (Exception e)
        {
            Repl.Log(e.Message, false, StructuredOutput.OutputMode.WARNING);
            return null;
        }
    }

    /// <summary>
    /// Checks whether or not a local IPFS daemon is currently online/reachable by attempting to acquire the Peer ID.
    /// </summary>
    /// <returns>True if the peer ID was found, false if the peer ID could not be determined.</returns>
    public static bool IsOnline()
    {
        return GetPeerId() != null;
    }
}
