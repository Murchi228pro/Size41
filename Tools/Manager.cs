using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _41Size.Tools
{
    public static class Manager
    {
        private static Frame _mainFrame;

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

        public static void ReturnBack()
        {
            MainFrame.GoBack();
        }
    }
}
