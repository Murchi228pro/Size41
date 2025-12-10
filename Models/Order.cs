using System;
using System.Collections.Generic;

namespace _41Size.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly OrderDeliveryDate { get; set; }

    public int OrderPickupPoint { get; set; }

    public string? OrderClientName { get; set; }

    public int OrderGetCode { get; set; }

    public string OrderStatus { get; set; } = null!;

    public int? OrderUserId { get; set; }

    public virtual PickupPoint OrderPickupPointNavigation { get; set; } = null!;

    public virtual User? UserIdNavigation { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
