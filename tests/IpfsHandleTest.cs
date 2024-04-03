using Wtns.Me.Lib.Net;

namespace tests;

/*** TEST CASES FOR IpfsHandle.cs
*
*    The following tests assume and require a local IPFS daemon running @ http://127.0.0.1:5001.
*    If all tests fail this is an indication no connection could be made.
*
*    Temporary files and folders are contained within the directory `./files/` (`../../../files/` is the path relative to the executable)
*    and are created upon requirement.
*    Dispose() will recursively delete this directory at the end of each test which required it.
***/

public class IpfsHandleTest
{
    public IpfsHandleTest()
    {
        Directory.CreateDirectory("../../../files/");
        using (StreamWriter s = File.CreateText("../../../files/testAddFile.txt"))
        {
            s.WriteLine("Hello World!");
            s.Flush();
            s.Close();
            s.Dispose();
        }
    }

    private void Dispose() // Call on all tests that create directories or files
    {
        Directory.Delete("../../../files/", true); // Remove the temporary folders and files.
    }

    [Fact]
    public void IpfsHandle_GetPeerId_ReturnNotNull()
    {
        var testCase = IpfsHandle.GetPeerId();
        Assert.NotNull(testCase);
    }

    [Fact]
    public void IpfsHandle_AddText_ReturnNotNull()
    {
        var testCase = IpfsHandle.AddText("Hello World!");
        Assert.NotNull(testCase);
    }

    [Fact]
    public void IpfsHandle_AddFile_ReturnNotNull()
    {
        var testCase = IpfsHandle.AddFile("../../../files/testAddFile.txt");
        Assert.NotNull(testCase);
        Dispose();
    }

    [Fact]
    public void IpfsHandle_GetFile_ReturnTrue()
    {
        var testCase = IpfsHandle.GetFile(
            "QmQzij4fuG8Xm3CbwWos6JA7sokZBPutbWeCt3Rw8prz6s",
            "testGetFile",
            "../../../files/"
        );
        Assert.True(testCase);
        Dispose();
    }

    [Fact]
    public void IpfsHandle_Pin_ReturnNotNull()
    {
        var testCase = IpfsHandle.Pin("QmQzij4fuG8Xm3CbwWos6JA7sokZBPutbWeCt3Rw8prz6s");
        Assert.NotNull(testCase);
    }

    [Fact]
    public void IpfsHandle_Unpin_ReturnNotNull()
    {
        var testCase = IpfsHandle.Unpin("QmQzij4fuG8Xm3CbwWos6JA7sokZBPutbWeCt3Rw8prz6s");
        Assert.NotNull(testCase);
    }
}
