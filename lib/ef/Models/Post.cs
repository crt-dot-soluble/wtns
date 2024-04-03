using System;
using System.Collections.Generic;

namespace lib.ef.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string PostContent { get; set; } = null!;

    public DateTime? PostDate { get; set; }

    public virtual User User { get; set; } = null!;
}
