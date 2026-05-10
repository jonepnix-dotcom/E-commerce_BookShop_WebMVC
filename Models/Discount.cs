using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Discount
{
    public int Iddiscount { get; set; }

    public string? Description { get; set; }

    public decimal DiscountValue { get; set; }

    public bool Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual ICollection<Bookorder> Bookorders { get; set; } = new List<Bookorder>();

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
