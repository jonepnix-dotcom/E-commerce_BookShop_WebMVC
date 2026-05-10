using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class EmailVerification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public virtual Client User { get; set; } = null!;
}
