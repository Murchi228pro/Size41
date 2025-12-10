using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _41Size.Data;
using _41Size.Models;
using _41Size.View;

namespace _41Size.Tools
{
    public class Session : IDisposable
    {
        public User? CurrentUser { get; private set; }

        public Order CurrentOrder = new Order {  };

        public string CurrentUserName
        {
            get
            {
                if (CurrentUser == null)
                {
                    return "Гость";
                }
                return CurrentUser.UserFullName;
            }
        }

        public Role CurrentRole
        {
            get
            {
                if (CurrentUser == null)
                {
                    return Yarullin41Context.Instance.Roles.Where(r => r.RoleName.Contains("клиент")).First();
                }
                return Yarullin41Context.Instance
                .Roles
                .Where(r => r.RoleId == CurrentUser.UserRole)
                .First();
            }
        }

        public Session()
        {
            
        }

        public Session(User user) 
        {
            if (user == null)
            {
                throw new Exception("User is incorrect!");
            }
            CurrentUser = user;
            Order? userOrder = Yarullin41Context.Instance.Orders.Where(o => o.UserIdNavigation == user).FirstOrDefault();
            if (userOrder != null)
            {
                CurrentOrder.OrderProducts = Yarullin41Context.Instance.OrderProducts.Where(o => o.OrderId == userOrder.OrderId).ToList();
                CurrentOrder = userOrder;
            }
            else
            {
                CurrentOrder = new Order();
            }
        }

        public void Dispose()
        {
            CurrentUser = null;
        }
    }
}
