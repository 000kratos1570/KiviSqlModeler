using KiviSqlModeler.Models;
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
using Wpf.Ui.Controls;

namespace KiviSqlModeler.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainProfilePage.xaml
    /// </summary>
    public partial class MainProfilePage : UiPage
    {
        public MainProfilePage()
        {
            InitializeComponent();

            if (AuthProfile.IsLoggedIn())
            {
                if (AuthProfile.AuthDolgnostID == 1)
                    ProfileFrame.NavigationService.Navigate(new ProfilePage());
                else
                    ProfileFrame.NavigationService.Navigate(new AdminPage());
            }
            else
            {
                 ProfileFrame.NavigationService.Navigate(new AuthPage());
            }
        }
    }
}
