using KiviSqlModeler.Models;
using KiviSqlModeler.Models.DataModels;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : UiPage
    {
        public ProfilePage()
        {
            InitializeComponent();

            DGItems();

            tbEmail.Text = AuthProfile.AuthProfileEmail;
            
        }

        public void DGItems()
        {
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            ObservableCollection<ProfileProject> profileProjects = MyApi.GetTable<ProfileProject>("ProfileProject");

            foreach (var project in profileProjects)
            {
                if (project.ProfileID == AuthProfile.AuthProfileID)
                {
                    projects.Add(MyApi.GetRecord<Project>("Project",project.ProjectID));
                }
            }
            dgProject.ItemsSource = projects;
            dgProject.SelectedValuePath = "id";
            dgProject.CanUserDeleteRows = false;
            dgProject.CanUserAddRows = false;
            dgProject.IsReadOnly = true;
            dgProject.SelectionMode = DataGridSelectionMode.Single;
        }

        private void dgProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGridRow = (Project)dgProject.SelectedItem;
            if (dataGridRow != null ) 
            {
                tbProject.Text = dataGridRow.ProjectName;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbProject.Text.Trim() != "")
            {
                try
                {
                    MyTables table = new Project()
                    {
                        ProjectName = tbProject.Text.Trim(),
                        LastDate = DateTime.Now,
                        JsonProject = ""
                    };
                    MyApi.PostRecord("Project", table);
                    MyTables table2 = new ProfileProject()
                    {
                        ProfileID = AuthProfile.AuthProfileID,
                        ProjectID = MyApi.GetTable<Project>("Project").Last().id
                    };
                    MyApi.PostRecord("ProfileProject", table2);

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
            if (tbProject.Text.Trim() != "" && dgProject.SelectedIndex != -1)
            {
                try
                {
                    MyTables table = new Project()
                    {
                        id = ((Project)dgProject.SelectedItem).id,
                        ProjectName = tbProject.Text.Trim(),
                        LastDate = DateTime.Now,
                        JsonProject = ""
                    };
                    MyApi.PutRecord("Project", table);
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
            if (dgProject.SelectedItem != null)
            {
                if (MessageBox.Show("Вы действительно хотите безвозвратно удалить?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ObservableCollection<ProfileProject> profileProjects = MyApi.GetTable<ProfileProject>("ProfileProject");
                    MyApi.DeleteRecord("ProfileProject", profileProjects.LastOrDefault(x => x.ProjectID == ((Project)dgProject.SelectedItem).id).id);
                    MyApi.DeleteRecord("Project", ((Project)dgProject.SelectedItem).id);
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

        private void btnProfileSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text.Trim() != "" && tbPassword.Text.Trim() != "")
            {
                try
                {
                    MyTables table = new Profile()
                    {
                        id = AuthProfile.AuthProfileID,
                        email = tbEmail.Text,
                        Password = MyApi.CreateSHA256(tbPassword.Text.Trim()),
                        DolgnostID = AuthProfile.AuthDolgnostID
                    };
                    MyApi.PutRecord("Profile", table);
                }
                catch (Exception)
                {
                    MessageBox.Show("Нет соединения с сервером", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста введите данные", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
