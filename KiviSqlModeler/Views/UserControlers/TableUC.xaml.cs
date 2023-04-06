using KiviSqlModeler.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
using System.Xml;

namespace KiviSqlModeler.Views.UserControlers
{
    /// <summary>
    /// Логика взаимодействия для TableUC.xaml
    /// </summary>
    public partial class TableUC : UserControl
    {
        public static readonly DependencyProperty NameTableProperty = DependencyProperty.Register(
            nameof(TableUC.NameTable), typeof(string),
            typeof(TableUC),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            nameof(TableUC.Columns), typeof(ObservableCollection<ColumnModel>),
            typeof(TableUC),
            new FrameworkPropertyMetadata(default(ObservableCollection<ColumnModel>), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TableUC(TableModel table)
        {
            InitializeComponent();

            MouseLeftButtonDown += TableUC_MouseLeftButtonDown;
            MouseLeftButtonUp += TableUC_MouseLeftButtonUp;
            MouseMove += TableUC_MouseMove;

            NameTable = table.Name;
            Columns = table.Columns;
            DataContext = this;
        }

        private bool isDragging = false;
        private Point lastPosition;

        private void TableUC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastPosition = e.GetPosition(Parent as UIElement);
            CaptureMouse();
        }

        private void TableUC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ReleaseMouseCapture();
        }

        private void TableUC_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(Parent as UIElement);
                double deltaX = currentPosition.X - lastPosition.X;
                double deltaY = currentPosition.Y - lastPosition.Y;
                double newLeft = Canvas.GetLeft(this) + deltaX;
                double newTop = Canvas.GetTop(this) + deltaY;
                Canvas.SetLeft(this, newLeft);
                Canvas.SetTop(this, newTop);
                lastPosition = currentPosition;
            }
        }

        public string NameTable
        {
            get => (string)GetValue(NameTableProperty);
            set => SetValue(NameTableProperty, value);
        }

        public ObservableCollection<ColumnModel> Columns
        {
            get => (ObservableCollection<ColumnModel>)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }
    }
}
