using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Image
{
    public int Idimage { get; set; }

    public int Idbook { get; set; }

    public string? Name { get; set; }

    public bool Status { get; set; }

    public virtual Book IdbookNavigation { get; set; } = null!;
}
