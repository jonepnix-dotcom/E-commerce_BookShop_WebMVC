using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Bookorder
{
    public int IdbookOrder { get; set; }

    public int Iduser { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public int Iddiscount { get; set; }

    public int Idvoucher { get; set; }

    public int Idtax { get; set; }

    public int Idpayment { get; set; }

    public string Address { get; set; } = null!;

    public DateTime? DeliveryDate { get; set; }

    public virtual Discount IddiscountNavigation { get; set; } = null!;

    public virtual Payment IdpaymentNavigation { get; set; } = null!;

    public virtual Tax IdtaxNavigation { get; set; } = null!;

    public virtual Client IduserNavigation { get; set; } = null!;

    public virtual Voucher IdvoucherNavigation { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();
}
