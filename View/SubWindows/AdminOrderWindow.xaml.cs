using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _41Size.Data;
using _41Size.Models;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace _41Size.View.SubWindows
{
    /// <summary>
    /// Логика взаимодействия для AdminOrderWindow.xaml
    /// </summary>
    public partial class AdminOrderWindow : Window , INotifyPropertyChanged
    {
        public Order EditableOrder { get; set; }

        public ObservableCollection<OrderProduct> OrderItems { get; set; }

        public virtual decimal OrderSum
        {
            get
            {
                decimal sum = 0;
                if (OrderItems.IsNullOrEmpty()) return sum;
                foreach (OrderProduct orderProduct in OrderItems)
                {
                    sum += (decimal)orderProduct.OrderCount * orderProduct.Product.ProductCost;
                }
                return sum;
            }
        }

        public virtual decimal OrderSumWithDiscount
        {
            get
            {
                decimal sum = 0;
                if (OrderItems.IsNullOrEmpty()) return sum;
                foreach (OrderProduct orderProduct in OrderItems)
                {
                    sum += (decimal)orderProduct.OrderCount * orderProduct.Product.ProductPrice;
                }
                return sum;
            }
        }

        public virtual decimal OrderDiscountSum
        {
            get
            {
                decimal sum = 0;
                if (OrderItems.IsNullOrEmpty()) return sum;
                foreach (OrderProduct orderProduct in OrderItems)
                {
                    sum += (decimal)orderProduct.OrderCount * (orderProduct.Product.ProductCost - orderProduct.Product.ProductPrice);
                }
                return sum;
            }
        }


        public AdminOrderWindow(Order order)
        {
            InitializeComponent();
            this.DataContext = this;
            EditableOrder = order;
            Closed += OnClosed;
            OrderItems = new ObservableCollection<OrderProduct>(EditableOrder.OrderProducts ?? new List<OrderProduct>());
            OrderItems.CollectionChanged += OnItemsUpdate;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnClosed(object sender, EventArgs e)
        {
            Yarullin41Context.Instance.ChangeTracker.Clear();
            Closed -= OnClosed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            EditableOrder.OrderProducts = OrderItems;
            Yarullin41Context.Instance.SaveChanges();
            Close();
        }

        public void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (ContentControl)sender;
            OrderProduct orderElement = (OrderProduct)item.DataContext;
            OrderItems.Remove(orderElement);
        }

        public void OnItemsUpdate(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new(nameof(OrderSum)));
            PropertyChanged?.Invoke(this, new(nameof(OrderDiscountSum)));
            PropertyChanged?.Invoke(this, new(nameof(OrderSumWithDiscount)));

        }
    }
}
