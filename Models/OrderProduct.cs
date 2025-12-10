using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace _41Size.Models;

public partial class OrderProduct : INotifyPropertyChanged
{
    public int OrderProductId { get; set; }

    public string ProductArticleNumber { get; set; } = null!;

    public int OrderId { get; set; }

    public int? OrderCount { get; set; }

    [NotMapped]
    public int? DisplayOrderCount
    {
        get => OrderCount;
        set
        {
            OrderCount = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayOrderCount)));
        }
    }

    public virtual Order Order { get; set; } = null!;

    [NotMapped]
    public virtual Product Product => ProductArticleNumberNavigation;

    public virtual Product ProductArticleNumberNavigation { get; set; } = null!;

    public event PropertyChangedEventHandler? PropertyChanged;
}
