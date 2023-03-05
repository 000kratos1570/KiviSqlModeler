using KiviSqlModeler.Model;
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

namespace KiviSqlModeler.MyUserControl
{
    /// <summary>
    /// Логика взаимодействия для Table.xaml
    /// </summary>
    public partial class Table : UserControl
    {
        public static readonly DependencyProperty NameTableProperty = DependencyProperty.Register(
            nameof(Table.NameTable), typeof(string), 
            typeof(Table),
            new FrameworkPropertyMetadata(default(string),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PkProperty = DependencyProperty.Register(
            nameof(Table.Pk), typeof(Column),
            typeof(Table),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            nameof(Table.Columns), typeof(ObservableCollection<Column>),
            typeof(Table),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Table()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string NameTable
        {
            get => (string)GetValue(NameTableProperty);
            set => SetValue(NameTableProperty, value);
        }

        public Column Pk
        {
            get =>(Column)GetValue(PkProperty);
            set => SetValue(PkProperty, value);
        }

        public ObservableCollection<Column> Columns
        {
            get =>(ObservableCollection<Column>)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }
    }
}
