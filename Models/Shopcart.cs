using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Shopcart
{
    public int IdshopCart { get; set; }

    public string Idsession { get; set; } = null!;

    public int Iduser { get; set; }

    public int Idbook { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpirationDate { get; set; }

    public virtual Book IdbookNavigation { get; set; } = null!;

    public virtual Client IduserNavigation { get; set; } = null!;
}
