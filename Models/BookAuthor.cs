using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class BookAuthor
{
    public int Id { get; set; }

    public int Idbook { get; set; }

    public int Idauthor { get; set; }

    public virtual Author IdauthorNavigation { get; set; } = null!;

    public virtual Book IdbookNavigation { get; set; } = null!;
}
