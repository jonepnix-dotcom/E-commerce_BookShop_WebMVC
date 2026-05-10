using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Discounthistory
{
    public int IddiscountHistory { get; set; }

    public int Idbook { get; set; }

    public int Iddiscount { get; set; }

    public DateTime ApplicableDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public bool Status { get; set; }

    public virtual Book IdbookNavigation { get; set; } = null!;

    public virtual Discount IddiscountNavigation { get; set; } = null!;
}
