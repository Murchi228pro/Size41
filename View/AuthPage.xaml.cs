using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Diagnostics;
using _41Size.Models;
using _41Size.Data;
using _41Size.Tools;

namespace _41Size.View
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page, INotifyPropertyChanged
    {

        public string LoginField
        {
            get => _loginField;
            set
            {
                _loginField = value;
                LogInfo = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoginField)));
            }
        }

        private string _loginField;

        public string PasswordField
        {
            get => _passwordField;
            set
            {
                _passwordField = value;
                LogInfo = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PasswordField)));
            }
        }

        private string _logInfo;
        public string LogInfo
        {
            get => _logInfo;
            set
            {
                _logInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogInfo)));
            }
        }

        private string _captchaField;

        public string CaptchaField
        {
            get => _captchaField;
            set
            {
                _captchaField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CaptchaField)));
            }
        }

        private Captcha _captcha;


        private string _passwordField;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AuthPage()
        {
            InitializeComponent();
            this.DataContext = this;
            UpdateCaptcha();
        }

        public void TryLogin(object? sender, EventArgs e)
        {
            if (_captcha.IsCaptchaValid(CaptchaField))
            {
                try
                {
                    User user = GetUser();
                    LogInfo = Yarullin41Context.Instance.Roles.Where(r => r.RoleId == user.UserRole).First().RoleName;
                    Manager.CreateSession(user);
                    return;
                }
                catch (ArgumentNullException ex)
                {
                    LogInfo = ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    LogInfo = ex.Message;
                }
                catch (Exception ex)
                {
                    LogInfo = ex.Message;
                    MessageBox.Show("Unhandled error!", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            else
            {
                LogInfo = "Неверная капча!";
            }
            UpdateCaptcha();
        }

        public void LoginAsVisitor(object? sender, EventArgs e)
        {
            Manager.CreateVisitorSession();
        }

        public User GetUser()
        {
            if (LoginField == null || PasswordField == null)
            {
                throw new ArgumentNullException("Fields is empty!");
            }
            IEnumerable<User> userSequence = Yarullin41Context.Instance
                .Users
                .Where(u => u.UserLogin == LoginField);
            if (userSequence.Count() == 0)
            {
                throw new InvalidOperationException("User not finded!");
            }
            User user = userSequence.First();
            if (user.UserPassword == PasswordField)
            {
                return user;
            }
            throw new InvalidOperationException("Password is incorrect!");
        }

        public void UpdateCaptcha()
        {
            _captcha = new Captcha();
            _captcha.Create(CaptchaImage);
        }
    }
}
