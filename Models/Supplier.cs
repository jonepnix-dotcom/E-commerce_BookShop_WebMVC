using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Supplier
{
    public int Idsupplier { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Phones { get; set; }

    public string? Address { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Warehousetransaction> Warehousetransactions { get; set; } = new List<Warehousetransaction>();
}
