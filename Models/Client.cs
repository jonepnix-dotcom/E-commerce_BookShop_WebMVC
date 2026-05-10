using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Client
{
    public int Iduser { get; set; }

    public string UserName { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phones { get; set; }

    public DateOnly? Birthday { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool Status { get; set; }

    public int? Authorization { get; set; }

    public virtual ICollection<Bookorder> Bookorders { get; set; } = new List<Bookorder>();

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<Evaluate> Evaluates { get; set; } = new List<Evaluate>();

    public virtual ICollection<Loginhistory> Loginhistories { get; set; } = new List<Loginhistory>();

    public virtual ICollection<Shopcart> Shopcarts { get; set; } = new List<Shopcart>();
}
