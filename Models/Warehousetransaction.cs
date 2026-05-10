using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Warehousetransaction
{
    public int IdwarehouseTransaction { get; set; }

    public int Idsupplier { get; set; }

    public int Idbook { get; set; }

    public DateOnly TransactionDate { get; set; }

    public int Quantity { get; set; }

    public string TransactionType { get; set; } = null!;

    public virtual Book IdbookNavigation { get; set; } = null!;

    public virtual Supplier IdsupplierNavigation { get; set; } = null!;
}
