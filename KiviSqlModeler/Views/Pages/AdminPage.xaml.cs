using KiviSqlModeler.Models.DataModels;
using KiviSqlModeler.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ProfileView
    {
        public int id { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public int DolgnostID { get; set; }
        public string Dolgnost { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : UiPage
    {
        public AdminPage()
        {
            InitializeComponent();

            DGItems();

            cbDolgnost.ItemsSource = MyApi.GetTable<DolgnostEntity>("Dolgnost");
            cbDolgnost.SelectedValuePath = "id";
            cbDolgnost.DisplayMemberPath = "Dolgnost";
        }

        public void DGItems()
        {
            ObservableCollection<Profile> projects = MyApi.GetTable<Profile>("Profile");
            ObservableCollection<ProfileView> profileViews = new ObservableCollection<ProfileView>();
            foreach (var profile in projects)
            {
                if (profile.id == AuthProfile.AuthProfileID)
                    continue;
                profileViews.Add(new ProfileView
                {
                    id = profile.id,
                    email = profile.email,
                    Password = profile.Password,
                    DolgnostID = profile.DolgnostID,
                    Dolgnost = MyApi.GetRecord<DolgnostEntity>("Dolgnost", profile.DolgnostID).Dolgnost
                });
            }

            dgProfile.ItemsSource = profileViews;
            dgProfile.SelectedValuePath = "id";
            dgProfile.CanUserDeleteRows = false;
            dgProfile.CanUserAddRows = false;
            dgProfile.IsReadOnly = true;
            dgProfile.SelectionMode = DataGridSelectionMode.Single;
        }

        private void dgProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var profile = (ProfileView)dgProfile.SelectedItem;
            if (profile != null)
            {
                tbEmail.Text = profile.email;
                cbDolgnost.SelectedValue = profile.DolgnostID;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text.Trim() != "" && cbDolgnost.SelectedIndex != -1)
            {
                try
                {
                    MyTables table = new Profile()
                    {
                        email = tbEmail.Text.Trim(),
                        Password = MyApi.CreateSHA256(tbPass.Text.Trim()),
                        DolgnostID = (int)cbDolgnost.SelectedValue
                    };
                    MyApi.PostRecord("Profile", table);

                    DGItems();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нет соединения с сервером", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text.Trim() != "" && cbDolgnost.SelectedIndex != -1 && dgProfile.SelectedIndex != -1)
            {
                try
                {
                    MyTables table = new Profile()
                    {
                        id = ((ProfileView)dgProfile.SelectedItem).id,
                        email = tbEmail.Text.Trim(),
                        Password = MyApi.CreateSHA256(tbPass.Text.Trim()),
                        DolgnostID = (int)cbDolgnost.SelectedValue
                    };
                    MyApi.PutRecord("Profile", table);
                    DGItems();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нет соединения с сервером", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfile.SelectedItem != null)
            {
                if (MessageBox.Show("Вы действительно хотите безвозвратно удалить? Все проекты этого пользователя будут удалены!", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ObservableCollection<ProfileProject> profileProjects = MyApi.GetTable<ProfileProject>("ProfileProject");
                    foreach (var profileProject in profileProjects)
                    {
                        if (profileProject.ProfileID == ((ProfileView)dgProfile.SelectedItem).id)
                        {
                            MyApi.DeleteRecord("ProfileProject", profileProject.id);
                            MyApi.DeleteRecord("Project", profileProject.ProjectID);
                        }
                    }
                    MyApi.DeleteRecord("Profile", ((ProfileView)dgProfile.SelectedItem).id);
                    DGItems();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста выделите элемент", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnEx_Click(object sender, RoutedEventArgs e)
        {
            AuthProfile.LogOut();
            NavigationService.Navigate(new AuthPage());
        }
    }
}
