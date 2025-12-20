using System;
using System.Collections.Generic;
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

namespace _41Size.View
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page, INotifyPropertyChanged
    {

        private bool _isSubWindowOpened = false;
        public bool IsSubWindowOpened {
            get => _isSubWindowOpened; 
            set { 
                _isSubWindowOpened = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSubWindowOpened)));
            } 
        }

        public Order? SelectedOrder { get; set; }


        public IEnumerable<Order> Orders {
            get => Yarullin41Context.Instance.Orders.
                OrderBy(o => o.OrderDate)
                .ToList();
                }

        public AdminPage()
        {
            InitializeComponent();
            this.DataContext = this;
            Yarullin41Context.Instance.SavedChanges += (o, e) => {
                PropertyChanged?.Invoke(this, new(nameof(Orders)));
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void EditWindowOpen_Click(object? sender, RoutedEventArgs e)
        {
            if (SelectedOrder != null)
            {
                IsSubWindowOpened = true;
                Window subWindow = new SubWindows.AdminOrderWindow(SelectedOrder);
                subWindow.Show();
                subWindow.Closed += (o, e) => IsSubWindowOpened = false;
            }
            
        }

        public void RemoveOrder_Click(object? sender, RoutedEventArgs e)
        {
            if (SelectedOrder != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить заказ?", "Потверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.No) return;

                IEnumerable<OrderProduct> orderProducts = Yarullin41Context.Instance.OrderProducts.Where(op => op.OrderNavigation == SelectedOrder);

                foreach(OrderProduct orderProduct in orderProducts)
                {
                    Yarullin41Context.Instance.OrderProducts.Remove(orderProduct);
                }
                Yarullin41Context.Instance.SaveChanges();


                Yarullin41Context.Instance.Orders.Remove(SelectedOrder);

                Yarullin41Context.Instance.SaveChanges();

                SelectedOrder = null;
            }
        }
    }
}
