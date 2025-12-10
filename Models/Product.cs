using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace _41Size.Models;

public partial class Product
{
    public string ProductArticleNumber { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal ProductCost { get; set; }

    public byte ProductMaxDiscount { get; set; }

    public string ProductManufacturer { get; set; } = null!;

    public string ProductStatus { get; set; } = null!;

    public string ProductCategory { get; set; } = null!;

    public byte ProductDiscountAmount { get; set; }

    public int ProductQuantityInStock { get; set; }

    public string ProductDescription { get; set; } = null!;

    public string ProductPhoto { get; set; } = null!;

    [NotMapped]
    public virtual string ProductPhotoPath => $"/Content/ProductImages/{ProductPhoto}"; 

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
