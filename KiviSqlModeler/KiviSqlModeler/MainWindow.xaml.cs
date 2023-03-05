using KiviSqlModeler.Model;
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

namespace KiviSqlModeler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            var rt = new TableModel(1, "aboba");
            rt.Columns.Add(new Column("строка1", "string"));
            rt.Columns.Add(new Column("строка2", "int"));
            rt.pk = new Column("primary key", "int", Column.PKFK.pk);

            myTable.NameTable = rt.Name;
            myTable.Pk = rt.pk;
            myTable.Columns = rt.Columns; 
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            this.DragMove();
        }

        private void cnvMain_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void cnvMain_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void cnvMain_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void cmAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmLine_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
