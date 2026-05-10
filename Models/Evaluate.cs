using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Evaluate
{
    public int Idevaluate { get; set; }

    public int Iduser { get; set; }

    public int Idbook { get; set; }

    public string Content { get; set; } = null!;

    public DateTime EvaluationDate { get; set; }

    public bool Status { get; set; }

    public virtual Book IdbookNavigation { get; set; } = null!;

    public virtual Client IduserNavigation { get; set; } = null!;
}
