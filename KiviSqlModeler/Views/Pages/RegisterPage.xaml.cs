using KiviSqlModeler.Models.DataModels;
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
using MessageBox = System.Windows.MessageBox;

namespace KiviSqlModeler.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : UiPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text.Trim() != "" && tbPassword.Text.Trim() != "")
            {
                try
                {
                    var values = MyApi.GetRecord<Profile>("Profile", tbEmail.Text.Trim());
                    if (values == null)
                    {
                        Profile profile = new Profile
                        {
                            email = tbEmail.Text.Trim(),
                            Password = MyApi.CreateSHA256(tbPassword.Text.Trim()),
                            DolgnostID = 1
                        };
                        MyApi.PostRecord("Profile", profile);
                        AuthProfile.Set(MyApi.GetRecord<Profile>("Profile", profile.email));
                        NavigationService.Navigate(new ProfilePage());

                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким Email уже существует", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Нет соединения с сервером", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Введите данные", "Данные пусты", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
