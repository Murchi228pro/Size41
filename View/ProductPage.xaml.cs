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
using _41Size.Tools;
using _41Size.Models;
using _41Size.Data;
using System.Diagnostics;
using Microsoft.Identity.Client;

namespace _41Size.View
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page, INotifyPropertyChanged
    {

        private string _searchField = string.Empty;

        public string SearchField
        {
            get => _searchField;
            set
            {
                _searchField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Products)));
            }
        }

        private int _costSortingDirection = 1;

        public int CostSortingDirection
        {
            get => _costSortingDirection;
            set
            {
                _costSortingDirection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Products)));
            }
        }

        private Tuple<byte, byte> _discountFilter;

        public Tuple<byte, byte> DiscountFilter
        {
            get => _discountFilter;
            set
            {
                _discountFilter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Products)));
            }
        }

        private List<Tuple<byte, byte>> _discoutFilterList = new()
        {
            new(0, 10),
            new(10, 15),
            new(15, 100),
            new(0, 100),
        };

        public List<Tuple<byte, byte>> DiscountFilterList => _discoutFilterList;

        public int InOrderProductCount => Manager.CurrentSession.CurrentOrder.OrderProductCount;

        public Product? SelectedProduct { get; set; }

        public IEnumerable<Product> Products
        {
            get
            {
                int minFilter = DiscountFilter.Item1;
                int maxFilter = DiscountFilter.Item2;
                return Yarullin41Context.Instance
                .Products
                .Where(p => p.ProductName.Contains(SearchField))
                .Where(p => p.ProductDiscountAmount >= minFilter && p.ProductDiscountAmount < maxFilter)
                .OrderBy(p => p.ProductCost * CostSortingDirection)
                .ToList();
            }
        }

        public string UserName => Manager.CurrentSession.CurrentUserName;

        public string UserRole => Manager.CurrentSession.CurrentRole.RoleName;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProductPage()
        {
            _discountFilter = new(0, 100);
            InitializeComponent();
            this.DataContext = this;
        }

        public void ProductClick(object? sender, RoutedEventArgs e)
        {
            Debug.WriteLine(sender.GetType());
        }

        public void AddToOrderClick(object? sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null) return;
            var currentOrderProduct = Manager.CurrentSession
                    .CurrentOrder
                    .OrderProducts
                    .Where(op => op.ProductArticleNumber == SelectedProduct.ProductArticleNumber)
                    .FirstOrDefault(new OrderProduct
                    {
                        OrderNavigation = Manager.CurrentSession.CurrentOrder,
                        ProductArticleNumber = SelectedProduct.ProductArticleNumber,
                        ProductArticleNumberNavigation = SelectedProduct,
                        OrderCount = 0
                    });

            if (currentOrderProduct.OrderCount + 1 > SelectedProduct.ProductQuantityInStock)
            {
                return;
            }
            if (!Manager.CurrentSession.CurrentOrder.OrderProducts.Contains(currentOrderProduct))
            {
                Manager.CurrentSession.CurrentOrder.OrderProducts.Add(currentOrderProduct);
            }
            currentOrderProduct.OrderCount++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InOrderProductCount)));
        }

        public void OpenCartClick(object? sender, RoutedEventArgs e)
        {
            Manager.Navigate(new OrderPage());
        }

        public void SetOrderDirectionUp(object? sender, EventArgs e)
        {
            CostSortingDirection = 1;
        }

        public void SetOrderDirectionDown(object? sender, EventArgs e) 
        {
            CostSortingDirection = -1;
        }
    }
}
