using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Booktype
{
    public int IdbookType { get; set; }

    public string BookTypeName { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
