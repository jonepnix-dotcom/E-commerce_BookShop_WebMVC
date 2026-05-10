using System;
using System.Collections.Generic;

namespace TheLight_JoneBookShop_WebMVC.Models;

public partial class Ship
{
    public int Idship { get; set; }

    public string ProvinceName { get; set; } = null!;

    public int Shipcost { get; set; }
}
