using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _41Size.Data;
using _41Size.Models;
using _41Size.Tools;
using Azure.Identity;
using Microsoft.IdentityModel.Tokens;

namespace _41Size.View
{
    /// <summary>
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public IEnumerable<PickupPoint> PickupPoints => Yarullin41Context.Instance.PickupPoints.ToList();

        public PickupPoint? SelectedPickupPoint { get; set; }

        public bool IsAuthorized => Manager.CurrentSession.CurrentUser != null;

        public string ClientUserName => Manager.CurrentSession.CurrentUserName;

        public string? ClientFullNameWritable { get; set; }

        public ObservableCollection<OrderProduct> SelectedProducts { get; set; }

        private DateTime? _orderDate;
        public DateTime? OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                if (value != null)
                    OrderDeliveryDate = value.Value.AddDays(3);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderDate)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderDeliveryDate)));
            }
        }

        public int? OrderCode
        {
            get
            {
                return Manager.CurrentSession.CurrentOrder.OrderGetCode;
            }
        }

        public DateTime OrderDeliveryDate { get; set; }

        public OrderPage()
        {
            SelectedProducts = new(Manager.CurrentSession.CurrentOrder.OrderProducts);
            OrderDate = Manager.CurrentSession.CurrentOrder.OrderDate.ToDateTime(TimeOnly.MinValue);
            SelectedPickupPoint = Manager.CurrentSession.CurrentOrder.OrderPickupPointNavigation;
            InitializeComponent();
            this.DataContext = this;
        }

        public void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Manager.CurrentSession.CurrentOrder.OrderProducts = new List<OrderProduct>(SelectedProducts);
            Manager.ReturnBack();
        }

        public void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Manager.ReturnBack();
        }

        public void ProductAddingClick(object? sender, RoutedEventArgs e)
        {
            OrderProduct? orderProduct = ((ContentControl)sender).DataContext as OrderProduct;
            if (orderProduct != null && orderProduct.OrderCount < orderProduct.Product.ProductQuantityInStock)
            {
                orderProduct.DisplayOrderCount++;
            }

        }

        public void ProductRemovingClick(object? sender, RoutedEventArgs e)
        {
            OrderProduct? orderProduct = ((ContentControl)sender).DataContext as OrderProduct;
            if (orderProduct != null && orderProduct.OrderCount > 1)
            {
                orderProduct.DisplayOrderCount--;
            }
            if (orderProduct.OrderCount == 1)
            {
                RemoveFromCart(orderProduct);
            }
        }

        public void ProductClearClick(object? sender, RoutedEventArgs e)
        {
            OrderProduct? orderProduct = ((ContentControl)sender).DataContext as OrderProduct;
            RemoveFromCart(orderProduct);
        }

        public void RemoveFromCart(OrderProduct? orderProduct)
        {
            var result = MessageBox.Show("Точно хотите убрать товар из корзины?", "Потверждение", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (orderProduct != null && result == MessageBoxResult.Yes)
            {
                SelectedProducts.Remove(orderProduct);
            }
        }

        public void CartClearClick(object? sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Точно хотите очистить корзину?", "Потверждение", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
                SelectedProducts.Clear();
        }

        public void SaveToUserOrder(object? sender, RoutedEventArgs e)
        {
            Order order = Manager.CurrentSession.CurrentOrder;
            try
            {
                Validate();
                order.OrderProducts = new List<OrderProduct>(SelectedProducts);
                order.OrderUserId = Manager.CurrentSession.CurrentUser.UserId;
                order.OrderDate = DateOnly.FromDateTime(OrderDate.Value);
                order.OrderDeliveryDate = DateOnly.FromDateTime(OrderDeliveryDate);
                order.OrderPickupPoint = SelectedPickupPoint.PointId;
                order.OrderGetCode = Yarullin41Context.Instance.Orders.OrderBy(o => o.OrderGetCode).Last().OrderGetCode + 1;
                order.OrderClientName = IsAuthorized ? ClientFullNameWritable : ClientUserName;
                order.OrderStatus = "Новый";
                if (!Yarullin41Context.Instance.Orders.Contains(order))
                {
                    Yarullin41Context.Instance.Orders.Add(order);
                }
                foreach (var orderProduct in SelectedProducts)
                {
                    if (!Yarullin41Context.Instance.OrderProducts.Contains(orderProduct))
                    {
                        Yarullin41Context.Instance.OrderProducts.Add(orderProduct);
                    }
                }
                Yarullin41Context.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Validate()
        {
            if (SelectedPickupPoint == null)
            {
                throw new Exception("Пожалуйста, выберите пункт выдачи");
            }

            if (OrderDate < DateTime.Now || OrderDate == null)
            {
                throw new Exception("Пожалуйста, выберите корректную дату отправки");
            }

            if (ClientFullNameWritable.IsNullOrEmpty() && !IsAuthorized)
            {
                throw new Exception("Вы не указали ФИО");
            }
        }
    }
}
