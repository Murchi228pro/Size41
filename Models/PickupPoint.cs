using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace _41Size.Models;

public partial class PickupPoint
{
    public int PointId { get; set; }

    public int PointMailIndex { get; set; }

    public string PointCity { get; set; } = null!;

    public string PointStreet { get; set; } = null!;

    public int PointHouseNumber { get; set; }

    [NotMapped]
    public string FullAddress =>
        $"{PointMailIndex}, {PointCity}, {PointStreet} {PointHouseNumber}";

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
