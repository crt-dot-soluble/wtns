namespace Wtns.Me.Lib.Net.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Bio { get; set; }

    public string Hash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public ulong Active { get; set; }
}

public partial class User
{
    public Session? Session = null;
}
