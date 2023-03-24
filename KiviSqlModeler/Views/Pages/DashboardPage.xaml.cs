using KiviSqlModeler.Models;
using KiviSqlModeler.Views.UserControlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;
using MessageBox = System.Windows.MessageBox;

namespace KiviSqlModeler.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : UiPage
    {
        private ObservableCollection<TableModel> LogTable = new ObservableCollection<TableModel>();
        private ObservableCollection<TableModel> PhyTable = new ObservableCollection<TableModel>();

        public DashboardPage()
        {
            InitializeComponent();

            HideProperies();
        }

        /// <summary>
        /// Скрыть меню заполнения данных
        /// </summary>
        public void HideProperies()
        {
            borderProperties.Visibility = Visibility.Hidden;
            MySplitter.Visibility = Visibility.Hidden;
            GridEditPanel.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
            GridEditPanel.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
        }

        /// <summary>
        /// Показать меню заполнения данных
        /// </summary>
        public void VisibleProperies()
        {
            borderProperties.Visibility = Visibility.Visible;
            MySplitter.Visibility = Visibility.Visible;
            GridEditPanel.ColumnDefinitions[1].Width = GridLength.Auto;
            GridEditPanel.ColumnDefinitions[2].Width = new GridLength(2, GridUnitType.Star);
            GridEditPanel.ColumnDefinitions[2].MinWidth = 150;
        }

        /// <summary>
        /// Прокрутка меню заполнения данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                ScrollViewer scroll =
                (sender as Grid).Parent as ScrollViewer;

                if (e.Delta < 0)
                    scroll.LineRight();
                else
                    scroll.LineLeft();
            }
        }

        /// <summary>
        /// Кнопка добавления таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddTable_Click(object sender, RoutedEventArgs e)
        {
            TableModel tableModel = new TableModel("table" + LogTable.Count);
            tableModel.Columns.Add(new ColumnModel("column","aboba" + LogTable.Count));
            tableModel.Columns.Add(new ColumnModel("column2","aboba" + LogTable.Count));
            tableModel.Columns.Add(new ColumnModel("column3","aboba" + LogTable.Count));
            int x = 10;
            int y = 10;
            foreach (var tab in LogTable)
            {
                if (!LogTable.Any(tab => tab.Top == y && tab.Left == x))
                {
                    break;
                }

                x += 10;
                y += 10;
            }
            tableModel.Top = y;
            tableModel.Left = x;

            TableUC myTable = GetTableUC(tableModel);
            EditPanel.Children.Add(myTable);

            LogTable.Add(tableModel);
            TableModel phyTableModel = new TableModel(tableModel.Name);
            foreach (var column in tableModel.Columns)
            {
                phyTableModel.Columns.Add(new ColumnModel(column.Name, column.Type, column.Pkfk, column.IsNull));
            }

            phyTableModel.Top = tableModel.Top;
            phyTableModel.Left = tableModel.Left;
            PhyTable.Add(phyTableModel);
        }

        public int selectedTable;
        public int selectedColumn;

        //private void DGSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (selectedTable == LogTable.IndexOf((sender as System.Windows.Controls.DataGrid).DataContext as TableModel))
        //    {
        //        var dataRowView = (ColumnModel)(sender as System.Windows.Controls.DataGrid).SelectedItem;
        //        if (dataRowView != null)
        //        {
        //            tbColumnName.Text = dataRowView.Name;
        //            tbColumnType.Text = dataRowView.Type;
        //            cbIsNull.IsChecked = dataRowView.IsNull == ColumnModel.isNull.Null ? true : false;
        //            cbPK.IsChecked = dataRowView.Pkfk == ColumnModel.PKFK.pk ? true : false;
        //        }
        //    }
        //}

        /// <summary>
        /// Заполнение меню данных выбранной таблицой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyTable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisibleProperies();
            int index = EditPanel.Children.IndexOf((TableUC)sender);
            SelectedTable(index);
            SelectedColumn(LogTable[index].Columns[0]);
            selectedColumn = 0;
        }

        /// <summary>
        /// Заполняет выбранный столбец в меню данных
        /// </summary>
        /// <param name="columnModel"></param>
        public void SelectedColumn(ColumnModel columnModel)
        {
            tbColumnName.Text = columnModel.Name;
            tbColumnType.Text = columnModel.Type;
            cbIsNull.IsChecked = columnModel.IsNull == ColumnModel.isNull.Null ? true : false;
            cbPK.IsChecked = columnModel.Pkfk == ColumnModel.PKFK.pk ? true : false;
        }

        /// <summary>
        /// Заполняет выбранную таблицу в меню данных
        /// </summary>
        /// <param name="index"></param>
        public void SelectedTable(int index)
        {
            selectedTable = index;
            tbTableName.Text = LogTable[index].Name;
        }

        /// <summary>
        /// запоминание нового раположения элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableUC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LogTable[EditPanel.Children.IndexOf((TableUC)sender)].Left = Canvas.GetLeft((TableUC)sender);
            LogTable[EditPanel.Children.IndexOf((TableUC)sender)].Top = Canvas.GetTop((TableUC)sender);

        }

        /// <summary>
        /// Кнопка закрытия меню данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseTable_Click(object sender, RoutedEventArgs e)
        {
            HideProperies();
        }

        /// <summary>
        /// Создание эллемента управления
        /// </summary>
        /// <param name="tableModel"> данные для эллемента</param>
        /// <returns></returns>
        public TableUC GetTableUC(TableModel tableModel)
        {
            TableUC myTable = new TableUC(tableModel);
            myTable.MouseLeftButtonUp += TableUC_MouseLeftButtonUp;
            myTable.MouseLeftButtonDown += MyTable_MouseLeftButtonDown;
            Canvas.SetTop(myTable, tableModel.Top);
            Canvas.SetLeft(myTable, tableModel.Left);
            return myTable;
        }

        public void RedrowTable(int selectedTable)
        {
            TableUC myTable = GetTableUC(LogTable[selectedTable]);
            EditPanel.Children.RemoveAt(selectedTable);
            EditPanel.Children.Insert(selectedTable, myTable);
        }

        /// <summary>
        /// Удаления столбца из выбранной таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveColumn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedColumn != -1)
            {
                LogTable[selectedTable].Columns.RemoveAt(selectedColumn);
                PhyTable[selectedTable].Columns.RemoveAt(selectedColumn);

                selectedColumn--;
                if (LogTable[selectedTable].Columns.Count != 0 && selectedColumn == -1)
                    selectedColumn++;

                RedrowTable(selectedTable);
            }
        }

        /// <summary>
        /// Добавление столбца в выбранную таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            LogTable[selectedTable].Columns.Add(new ColumnModel("column"));
            PhyTable[selectedTable].Columns.Add(new ColumnModel("column"));

            selectedColumn = LogTable[selectedTable].Columns.Count - 1;
            SelectedColumn(LogTable[selectedTable].Columns[selectedColumn]);

            RedrowTable(selectedTable);
        }

        /// <summary>
        /// Заполнения меню данных предыдущим столбце
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreviousColumn_Click(object sender, RoutedEventArgs e)
        {
            selectedColumn = selectedColumn == 0 ? LogTable[selectedTable].Columns.Count - 1 : selectedColumn - 1;
            SelectedColumn(LogTable[selectedTable].Columns[selectedColumn]);
        }

        /// <summary>
        /// Заполнения меню данных следующим столбце
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextColumn_Click(object sender, RoutedEventArgs e)
        {
            selectedColumn = selectedColumn == LogTable[selectedTable].Columns.Count - 1 ? 0 : selectedColumn + 1;
            SelectedColumn(LogTable[selectedTable].Columns[selectedColumn]);
        }

        /// <summary>
        /// Сохраняет изменения таблица (названия и выбранного столбца)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTable_Click(object sender, RoutedEventArgs e)
        {
            LogTable[selectedTable].Name = tbTableName.Text.Trim();
            LogTable[selectedTable].Columns[selectedColumn].Name = tbColumnName.Text.Trim();
            LogTable[selectedTable].Columns[selectedColumn].Type = tbColumnType.Text.Trim();
            LogTable[selectedTable].Columns[selectedColumn].Pkfk = cbPK.IsChecked == true ? ColumnModel.PKFK.pk : ColumnModel.PKFK.none;
            LogTable[selectedTable].Columns[selectedColumn].IsNull = cbIsNull.IsChecked == true ? ColumnModel.isNull.Null : ColumnModel.isNull.NotNull;

            RedrowTable(selectedTable);

        }

        private readonly MatrixTransform _transform = new MatrixTransform();
        private Point _initialMousePosition;

        private void EditPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                _initialMousePosition = _transform.Inverse.Transform(e.GetPosition(EditPanel));
            }

        }

        private void EditPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point mousePosition = _transform.Inverse.Transform(e.GetPosition(EditPanel));
                Vector delta = Point.Subtract(mousePosition, _initialMousePosition);
                var translate = new TranslateTransform(delta.X, delta.Y);
                _transform.Matrix = translate.Value * _transform.Matrix;

                foreach (UIElement child in EditPanel.Children)
                {
                    child.RenderTransform = _transform;
                }
            }
        }

        public float Zoomfactor { get; set; } = 1.1f;

        private void EditPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            float scaleFactor = Zoomfactor;
            if (e.Delta < 0)
            {
                scaleFactor = 1f / scaleFactor;
            }

            Point mousePostion = e.GetPosition(EditPanel);

            Matrix scaleMatrix = _transform.Matrix;
            scaleMatrix.ScaleAt(scaleFactor, scaleFactor, mousePostion.X, mousePostion.Y);

            double currentScale = (float) Math.Sqrt(scaleMatrix.M11 * scaleMatrix.M11 + scaleMatrix.M12 * scaleMatrix.M12);
            if (currentScale < 0.3)
            {
                return;
            }
            else if (currentScale > 3)
            {
                return;
            }

            _transform.Matrix = scaleMatrix;

            foreach (UIElement child in EditPanel.Children)
            {
                double x = Canvas.GetLeft(child);
                double y = Canvas.GetTop(child);

                double sx = x * scaleFactor;
                double sy = y * scaleFactor;

                Canvas.SetLeft(child, sx);
                Canvas.SetTop(child, sy);

                child.RenderTransform = _transform;

            }
        }

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            CreateArrowBetween((TableUC)EditPanel.Children[0], (TableUC)EditPanel.Children[1], EditPanel);
        }

        private void CreateArrowBetween(TableUC startElement, TableUC endElemend, Panel parentContainer)
        {
            SolidColorBrush arrowBrush = Brushes.Red;

            // Center the line horizontally and vertically.
            // Get the positions of the controls that should be connected by a line.
            Point centeredArrowStartPosition = startElement.TransformToAncestor(parentContainer)
              .Transform(new Point(startElement.ActualWidth / 2, startElement.ActualHeight / 2));

            Point centeredArrowEndPosition = endElemend.TransformToAncestor(parentContainer)
              .Transform(new Point(endElemend.ActualWidth / 2, endElemend.ActualHeight / 2));

            // Draw the line between two controls
            var arrowLine = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                X1 = centeredArrowStartPosition.X,
                Y2 = centeredArrowEndPosition.Y,
                X2 = centeredArrowEndPosition.X,
                Y1 = centeredArrowStartPosition.Y
            };
            parentContainer.Children.Add(
              arrowLine);

            // Create the arrow tip of the line. The arrow has a width of 8px and a height of 8px,
            // where the position of arrow tip and the line's end are the same
            var arrowLineTip = new Polygon() { Fill = Brushes.Red };
            var leftRectanglePoint = new Point(centeredArrowEndPosition.X - 4, centeredArrowEndPosition.Y + 8);
            var rightRectanglePoint = new Point(
              centeredArrowEndPosition.X + 4,
              centeredArrowEndPosition.Y + 8);
            var rectangleTipPoint = new Point(centeredArrowEndPosition.X, centeredArrowEndPosition.Y);
            var myPointCollection = new PointCollection
            {
                leftRectanglePoint,
                rightRectanglePoint,
                rectangleTipPoint
            };
            arrowLineTip.Points = myPointCollection;
            parentContainer.Children.Add(
              arrowLineTip);
        }
    }
}