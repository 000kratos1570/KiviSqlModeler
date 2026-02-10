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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace KiviSqlModeler.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для OpenProjectWindow.xaml
    /// </summary>
    public partial class OpenProjectWindow : UiWindow
    {
        public string Json { get; private set; }
        private bool Save;
        public OpenProjectWindow(bool save, string json)
        {
            InitializeComponent();
            this.Save = save;
            this.Json = json;
            DGItems();
        }

        public void DGItems()
        {
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            ObservableCollection<ProfileProject> profileProjects = MyApi.GetTable<ProfileProject>("ProfileProject");

            foreach (var project in profileProjects)
            {
                if (project.ProfileID == AuthProfile.AuthProfileID)
                {
                    projects.Add(MyApi.GetRecord<Project>("Project", project.ProjectID));
                }
            }
            dgProject.ItemsSource = projects;
            dgProject.SelectedValuePath = "id";
            dgProject.CanUserDeleteRows = false;
            dgProject.CanUserAddRows = false;
            dgProject.IsReadOnly = true;
            dgProject.SelectionMode = DataGridSelectionMode.Single;
        }

        private void btnProject_Click(object sender, RoutedEventArgs e)
        {
            if (Save)
            {
                if (dgProject.SelectedIndex != -1)
                {
                    try
                    {
                        var project = (Project)dgProject.SelectedItem;
                        MyTables table = new Project()
                        {
                            id = project.id,
                            ProjectName = project.ProjectName,
                            LastDate = DateTime.Now,
                            JsonProject = Json
                        };
                        MyApi.PutRecord("Project", table);
                        
                    }
                    catch (Exception)
                    {
                        System.Windows.MessageBox.Show("Нет соединения с сервером", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Close();
                }
            }
            else
            {
                if (dgProject.SelectedIndex != -1)
                {
                    Json = ((Project)dgProject.SelectedItem).JsonProject;
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
