using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Orderdetail
{
    public int IdorderDetails { get; set; }

    public int IdbookOrder { get; set; }

    public int Idbook { get; set; }

    public int Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Book IdbookNavigation { get; set; } = null!;

    public virtual Bookorder IdbookOrderNavigation { get; set; } = null!;
}
