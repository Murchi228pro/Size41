using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using _41Size.Data;

namespace _41Size.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly OrderDate { get; set; }

    [NotMapped]
    public DateTime OrderDateDisplay
    {
        get => OrderDate.ToDateTime(TimeOnly.MinValue);
        set => OrderDate = DateOnly.FromDateTime(value);
    }

    public DateOnly OrderDeliveryDate { get; set; }

    [NotMapped]
    public DateTime OrderDeliveryDateDisplay
    {
        get => OrderDeliveryDate.ToDateTime(TimeOnly.MinValue);
        set => OrderDeliveryDate = DateOnly.FromDateTime(value);
    }

    public int OrderPickupPoint { get; set; }

    public string? OrderClientName { get; set; }

    public int OrderGetCode { get; set; }

    public string OrderStatus { get; set; } = null!;

    public int? OrderUserId { get; set; }


    public virtual PickupPoint OrderPickupPointNavigation { get; set; } = null!;

    [NotMapped]
    public virtual PickupPoint PickupPoint {
        get => Yarullin41Context.Instance.PickupPoints.Where(pp => pp.PointId == OrderPickupPoint).First();
    }

    public virtual User? UserIdNavigation { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = null!;

    [NotMapped]
    public virtual decimal OrderSum
    {
        get
        {
            var orderProductList = Yarullin41Context.Instance
                .OrderProducts
                .Where(op => op.OrderId == this.OrderId).ToList();
            decimal sum = 0;

            foreach (var op in orderProductList)
            {

                sum += Yarullin41Context.Instance.Products.Where(p => p.ProductArticleNumber == op.ProductArticleNumber).First().ProductPrice * op.OrderCount.GetValueOrDefault(0);
            }

            return sum;
        }
    }

    

    [NotMapped]
    public virtual int OrderProductCount
    {
        get => Yarullin41Context.Instance.OrderProducts.Where(op => op.OrderProductId == this.OrderId).Count();
    }
}
