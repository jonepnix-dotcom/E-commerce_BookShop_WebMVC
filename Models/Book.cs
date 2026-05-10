using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Book
{
    public int Idbook { get; set; }

    public int IdbookType { get; set; }

    public string BookName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int TotalQuantity { get; set; }

    public DateOnly PublicationDate { get; set; }

    public bool Status { get; set; }

    public int Iddiscount { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<Evaluate> Evaluates { get; set; } = new List<Evaluate>();

    public virtual Booktype IdbookTypeNavigation { get; set; } = null!;

    public virtual Discount IddiscountNavigation { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Shopcart> Shopcarts { get; set; } = new List<Shopcart>();

    public virtual ICollection<Warehousetransaction> Warehousetransactions { get; set; } = new List<Warehousetransaction>();
}
