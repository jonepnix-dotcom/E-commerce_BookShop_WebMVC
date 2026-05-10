using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Tax
{
    public int Idtax { get; set; }

    public DateTime Date { get; set; }

    public int Tax1 { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Bookorder> Bookorders { get; set; } = new List<Bookorder>();
}
