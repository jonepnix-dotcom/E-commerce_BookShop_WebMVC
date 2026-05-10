using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Loginhistory
{
    public int IdloginHistory { get; set; }

    public int Iduser { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime LogoutTime { get; set; }

    public virtual Client IduserNavigation { get; set; } = null!;
}
