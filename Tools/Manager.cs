using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using _41Size.Data;
using _41Size.View;
using _41Size.Models;

namespace _41Size.Tools
{
    public static class Manager
    {
        private static Frame _mainFrame;

        public static Session? CurrentSession { get; private set; }


        public static Frame MainFrame
        {
            get => _mainFrame;
            set => _mainFrame = value;
        }

        public static void Navigate(object element)
        {
            if (MainFrame != null)
            {
                MainFrame.Navigate(element);
            }
        }

        public static void CreateSession(User user)
        {
            if (CurrentSession != null)
            {
                DestroySession();
            }
            CurrentSession = new Session(user);
            switch(user.UserRoleNavigation.RoleName.ToLower())
            {
                case "клиент":
                    Navigate(new ProductPage());
                    break;
                case "администратор":
                case "менеджер":
                    Navigate(new AdminPage());
                    break;
            }
        }

        public static void CreateVisitorSession()
        {
            CurrentSession = new Session();
            Navigate(new ProductPage());
        }

        public static void DestroySession()
        {
            CurrentSession.Dispose();
        }

        public static void ReturnBack()
        {
            MainFrame.GoBack();
        }
    }
}
