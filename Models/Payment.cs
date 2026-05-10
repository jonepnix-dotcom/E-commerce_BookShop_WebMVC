using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Payment
{
    public int Idpayment { get; set; }

    public string Type { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Bookorder> Bookorders { get; set; } = new List<Bookorder>();
}
