using KiviSqlModeler.Models;
using KiviSqlModeler.Models.DataModels;
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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : UiPage
    {
        public AuthPage()
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
                    if (values != null)
                    {
                        if (values.Password == MyApi.CreateSHA256(tbPassword.Text.Trim()))
                        {
                            AuthProfile.Set(values);

                            switch (values.DolgnostID)
                            {
                                case 1: //Покупатель - User // user user1234
                                    {
                                        NavigationService.Navigate(new ProfilePage());
                                        break;
                                    }
                                case 2://Администратор - Admin // admin admin1234
                                    {
                                        NavigationService.Navigate(new AdminPage());
                                        break;
                                    }
                                default:
                                    {
                                        MessageBox.Show("Ваша роль не зарегистрированна в базе данных", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void linkRegistration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
