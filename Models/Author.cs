using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Author
{
    public int Idauthor { get; set; }

    public string AuthorName { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
