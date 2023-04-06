using KiviSqlModeler.Models;
using KiviSqlModeler.Views.UserControlers;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private ObservableCollection<ConnectionModel> connections = new ObservableCollection<ConnectionModel>();

        public DashboardPage()
        {
            InitializeComponent();
            HideProperies();
        }

        #region MenuData
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
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                ScrollViewer scrollViewer = (ScrollViewer)sender;
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            }
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
                SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn]);

                RedrowTable(selectedTable, selectedCanvasElement);
            }
        }

        /// <summary>
        /// Добавление столбца в выбранную таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            LogTable[selectedTable].Columns.Add(new ColumnModel("column", ""));
            PhyTable[selectedTable].Columns.Add(new ColumnModel("column", ""));

            selectedColumn = LogTable[selectedTable].Columns.Count - 1;
            SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn]);

            RedrowTable(selectedTable, selectedCanvasElement);
        }

        /// <summary>
        /// Заполнения меню данных предыдущим столбце
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreviousColumn_Click(object sender, RoutedEventArgs e)
        {
            selectedColumn = selectedColumn == 0 ? GetModel()[selectedTable].Columns.Count - 1 : selectedColumn - 1;
            SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn]);
        }

        /// <summary>
        /// Заполнения меню данных следующим столбце
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextColumn_Click(object sender, RoutedEventArgs e)
        {
            selectedColumn = selectedColumn == LogTable[selectedTable].Columns.Count - 1 ? 0 : selectedColumn + 1;
            SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn]);
        }

        /// <summary>
        /// Сохраняет изменения таблица (названия и выбранного столбца)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTable_Click(object sender, RoutedEventArgs e)
        {
            GetModel()[selectedTable].Name = tbTableName.Text.Trim();
            GetModel()[selectedTable].Columns[selectedColumn].Name = tbColumnName.Text.Trim();
            GetModel()[selectedTable].Columns[selectedColumn].Type = tbColumnType.Text.Trim();
            if (cbPK.IsChecked == true)
            {
                if (GetModel()[selectedTable].Columns[selectedColumn].Pkfk == ColumnModel.PKFK.fk)
                {
                    GetModel()[selectedTable].Columns[selectedColumn].Pkfk = ColumnModel.PKFK.pkfk;
                }
                else
                {
                    GetModel()[selectedTable].Columns[selectedColumn].Pkfk = ColumnModel.PKFK.pk;
                }
            }
            GetModel()[selectedTable].Columns[selectedColumn].Pkfk = cbPK.IsChecked == true ? ColumnModel.PKFK.pk : ColumnModel.PKFK.none;
            GetModel()[selectedTable].Columns[selectedColumn].IsNull = cbIsNull.IsChecked == true ? ColumnModel.isNull.Null : ColumnModel.isNull.NotNull;

            RedrowTable(selectedTable, selectedCanvasElement);

        }

        #endregion MenuData

        /// <summary>
        /// Кнопка добавления таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddTable_Click(object sender, RoutedEventArgs e)
        {
            TableModel tableModel = new TableModel("table" + LogTable.Count);
            tableModel.Columns.Add(new ColumnModel("column", "int",ColumnModel.isNull.NotNull, ColumnModel.PKFK.pk));
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

            LogTable.Add(tableModel);
            TableModel phyTableModel = new TableModel(tableModel.Name);
            foreach (var column in tableModel.Columns)
            {
                phyTableModel.Columns.Add(new ColumnModel(column.Name, column.Type, column.IsNull, column.Pkfk));
            }

            phyTableModel.Top = tableModel.Top;
            phyTableModel.Left = tableModel.Left;
            PhyTable.Add(phyTableModel);

            TableUC myTable = GetTableUC(cbModel.SelectedIndex == 0 ? tableModel : phyTableModel);
            EditPanel.Children.Add(myTable);
        }

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            isAddArrow = !isAddArrow;
            if (isAddArrow)
            {
                btnAddRow.BorderBrush = Application.Current.FindResource("TextFillColorPrimaryBrush") as SolidColorBrush;
            }
            else
            {
                firstCanvasTable = -1;
                firstColumn = -1;
                firstTable = -1;
                ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                btnAddRow.BorderBrush = new SolidColorBrush(Color.FromArgb(0x12, 0xFF, 0xFF, 0xFF));
            }
        }

        private void cbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditPanel == null) return;
            foreach (var table in EditPanel.Children.OfType<TableUC>().ToList())
            {
                int selectedCanvasElement = EditPanel.Children.IndexOf(table);
                int selectedTable = EditPanel.Children.OfType<TableUC>().ToList().IndexOf(table);
                EditPanel.Children.RemoveAt(selectedCanvasElement);
                TableUC myTable = GetTableUC(GetModel()[selectedTable]);
                EditPanel.Children.Insert(selectedCanvasElement, myTable);
            }
            selectedCanvasElement = -1;
            selectedColumn = -1;
            selectedTable = -1;
            HideProperies();
        }

        //bool isTrash = false;

        //private void btnTrashcan_Click(object sender, RoutedEventArgs e)
        //{
        //    isTrash = !isTrash;                                              //// Удаление таблицы без визуальных багов
        //    if (isTrash)
        //    {
        //        btnAddRow.BorderBrush = Application.Current.FindResource("TextFillColorPrimaryBrush") as SolidColorBrush;
        //    }
        //    else
        //    {
        //        btnAddRow.BorderBrush = new SolidColorBrush(Color.FromArgb(0x12, 0xFF, 0xFF, 0xFF));
        //    }
        //}

        #region TableControl

        public int selectedTable;
        public int selectedCanvasElement;
        public int selectedColumn;

        public ObservableCollection<TableModel> GetModel()
        {
            if (cbModel.SelectedIndex == 0)
            {
                return LogTable;
            }
            else
            {
                return PhyTable;
            }
        }

        public int GetTableIndex(object sender)
        {
            return EditPanel.Children.OfType<TableUC>().ToList().IndexOf((TableUC)sender);
        }

        public int GetTableIndex(TableUC sender)
        {
            return EditPanel.Children.OfType<TableUC>().ToList().IndexOf(sender);
        }

        /// <summary>
        /// Заполнение меню данных выбранной таблицой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyTable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisibleProperies();
            int index = GetTableIndex(sender);
            SelectedTable(index, EditPanel.Children.IndexOf((TableUC)sender));
            SelectedColumn(GetModel()[index].Columns[0]);
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
            cbPK.IsChecked = (columnModel.Pkfk == ColumnModel.PKFK.pk || columnModel.Pkfk == ColumnModel.PKFK.pkfk) ? true : false;
        }

        /// <summary>
        /// Заполняет выбранную таблицу в меню данных
        /// </summary>
        /// <param name="index"></param>
        public void SelectedTable(int index, int indexCanvasElement)
        {
            selectedCanvasElement = indexCanvasElement;
            selectedTable = index;
            tbTableName.Text = GetModel()[index].Name;
        }


        /// <summary>
        /// запоминание нового раположения элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableUC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)   // Добавить создание нескольких ВК по нескольким ПК
        {
            //if (isTrash)
            //{
            //    LogTable.RemoveAt(GetTableIndex(sender));                                              //// Удаление таблицы без визуальных багов
            //    PhyTable.RemoveAt(GetTableIndex(sender));
            //    EditPanel.Children.RemoveAt(EditPanel.Children.IndexOf((TableUC)sender));

            //}

            LogTable[GetTableIndex(sender)].Left = Canvas.GetLeft((TableUC)sender);
            LogTable[GetTableIndex(sender)].Top = Canvas.GetTop((TableUC)sender);
            PhyTable[GetTableIndex(sender)].Left = Canvas.GetLeft((TableUC)sender);
            PhyTable[GetTableIndex(sender)].Top = Canvas.GetTop((TableUC)sender);

            if (isAddArrow)
            {
                int indexCanvasElement = EditPanel.Children.IndexOf((TableUC)sender);
                int index = GetTableIndex(sender);
                int indexColumn = ((TableUC)EditPanel.Children[indexCanvasElement]).dgTable.SelectedIndex;

                if (indexColumn == -1)
                {
                    return;
                }
                else
                {
                    if (firstColumn == -1)
                    {
                        firstCanvasTable = indexCanvasElement;
                        firstTable = index;
                        firstColumn = indexColumn;
                    }
                    else if (firstCanvasTable != index)
                    {
                        LogTable[index].Columns.Add(new ColumnModel("column", "", ColumnModel.isNull.Null, ColumnModel.PKFK.fk));
                        PhyTable[index].Columns.Add(new ColumnModel("column", "", ColumnModel.isNull.Null, ColumnModel.PKFK.fk));

                        RedrowTable(index, indexCanvasElement);

                        TableUC first = (TableUC)EditPanel.Children[firstCanvasTable];
                        TableUC second = (TableUC)EditPanel.Children[indexCanvasElement];
                        connections.Add(new ConnectionModel()
                        {
                            Source = first,
                            Destination = second,
                            Dashed = false,
                            Shape = ShapeEnum.Arrow,
                            DIndexCanvasTable = indexCanvasElement,
                            CIndexCanvasTable = firstCanvasTable,
                            SourceTable = firstTable,
                            DestinationTable = index,
                            SourceColumn = firstColumn,
                            DestinationColumn = GetModel()[index].Columns.Count - 1,
                        });
                        CreateArrowBetween();
                        ((TableUC)EditPanel.Children[indexCanvasElement]).dgTable.SelectedIndex = -1;
                        ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                        firstColumn = -1;
                        firstCanvasTable = -1;
                        firstTable = -1;
                    }
                }
            }
        }

        private void MyTable_MouseMove(object sender, MouseEventArgs e)
        {
            CreateArrowBetween();
        }

        #endregion TableControl


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
            myTable.MouseMove += MyTable_MouseMove;

            myTable.MouseRightButtonDown += MyTable_MouseRightButtonDown;
            Canvas.SetTop(myTable, tableModel.Top);
            Canvas.SetLeft(myTable, tableModel.Left);
            return myTable;
        }

        private TableUC deleteTable;

        private void MyTable_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new()
            {
                PlacementTarget = (TableUC)sender,
                Placement = PlacementMode.Mouse,
                IsOpen = true
            };
            deleteTable = (TableUC)sender;
            Wpf.Ui.Controls.MenuItem removePath = new() { Header = "Удалить" };
            removePath.Click += RemoveTable;
            contextMenu.Items.Add(removePath);
        }

        private void RemoveTable(object sender, RoutedEventArgs e) 
        {
            int indexTable = GetTableIndex(deleteTable);
            int indexCanvasElement = EditPanel.Children.IndexOf(deleteTable);
            
            if (isAddArrow)
            {
                isAddArrow = !isAddArrow;
                firstCanvasTable = -1;
                firstColumn = -1;
                firstTable = -1;
                ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                btnAddRow.BorderBrush = new SolidColorBrush(Color.FromArgb(0x12, 0xFF, 0xFF, 0xFF));
            }

            if (selectedTable == indexTable)
            {
                selectedTable = -1;
                selectedCanvasElement = -1;
                selectedColumn = -1;
                HideProperies();
            }

            EditPanel.Children.RemoveAt(indexCanvasElement);
            LogTable.RemoveAt(indexTable);
            PhyTable.RemoveAt(indexTable);

            // удаление связей

            foreach (var con in connections)
            {
                if (con.SourceTable == indexTable)
                {
                    RemoveArrow(con.Path);
                }
            }
        }

        public void RedrowTable(int selectedTable, int selectedCanvasElement)
        {
            TableUC myTable = GetTableUC(GetModel()[selectedTable]);
            EditPanel.Children.RemoveAt(selectedCanvasElement);
            EditPanel.Children.Insert(selectedCanvasElement, myTable);
            foreach (var c in connections)
            {
                if (c.CIndexCanvasTable == selectedCanvasElement)
                {
                    c.Source = myTable;
                    c.SourceTable = selectedTable;
                }
                if (c.DIndexCanvasTable == selectedCanvasElement)
                {
                    c.Destination = myTable;
                    c.DestinationTable =selectedTable;
                }
            }
        }

        #region Zooming and Panning

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

            double currentScale = (float)Math.Sqrt(scaleMatrix.M11 * scaleMatrix.M11 + scaleMatrix.M12 * scaleMatrix.M12);
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
            CreateArrowBetween();
        }

        #endregion Zooming and Panning

        #region Arrow control

        private int firstCanvasTable = -1;
        private int firstTable = -1;
        private int firstColumn = -1;
        private bool isAddArrow = false;

        private void CreateArrowBetween()
        {
            foreach (var c in connections)
            {
                double halfSourceWidth = c.Source.ActualWidth / 2;
                double halfSourceHeight = c.Source.ActualHeight / 2;
                double halfDestinationWidth = c.Destination.ActualWidth / 2;
                double halfDestinationHeight = c.Destination.ActualHeight / 2;
                double SrcX = Canvas.GetLeft(c.Source) + (c.Source.ActualWidth / 2);
                double SrcY = Canvas.GetTop(c.Source) + (c.Source.ActualHeight / 2);
                double DstX = Canvas.GetLeft(c.Destination) + halfDestinationWidth;
                double DstY = Canvas.GetTop(c.Destination) + halfDestinationHeight;
                var brash = Application.Current.FindResource("TextFillColorPrimaryBrush") as SolidColorBrush;

                if (c.Path is null)
                {
                    c.Path = SetPath();
                    c.Path.Stroke = brash;
                    c.Path.PreviewMouseRightButtonDown += Path_PreviewMouseRightButtonDown;
                    EditPanel.Children.Add(c.Path);
                }
                if (c.ShapePath is null)
                {
                    c.ShapePath = SetPath();
                    c.ShapePath.Stroke = brash;
                    EditPanel.Children.Add(c.ShapePath);
                }
                c.ShapePath.Fill = brash;
                c.Path.StrokeDashArray = c.Dashed ? new DoubleCollection() { 3, 3 } : null;
                Point SrcP = new() { X = SrcX, Y = SrcY };
                Point DstP = new() { X = DstX, Y = DstY };
                double distanceX = SrcP.X - DstP.X;
                double distanceY = SrcP.Y - DstP.Y;
                bool sideConnections = Math.Abs(distanceY) <= Math.Max(c.Source.ActualHeight, c.Destination.ActualHeight);

                Point StartPoint = new()
                {
                    X = SrcP.X - (sideConnections ? distanceX > 0 ? halfSourceWidth : (-halfSourceWidth) : 0),
                    Y = SrcP.Y - (sideConnections ? 0 : distanceY > 0 ? halfSourceHeight : (-halfSourceHeight))
                };
                Point EndPoint = new()
                {
                    X = DstP.X + (sideConnections ? distanceX > 0 ? halfDestinationWidth : (-halfDestinationWidth) : 0),
                    Y = DstP.Y + (sideConnections ? 0 : distanceY > 0 ? halfDestinationHeight : (-halfDestinationHeight))
                };

                double distanceX2 = StartPoint.X - EndPoint.X;
                double distanceY2 = StartPoint.Y - EndPoint.Y;
                bool sideConnections2 = Math.Abs(distanceY) <= Math.Max(c.Source.ActualHeight, c.Destination.ActualHeight);
                PathFigureCollection pathFigures = new() {
                    new PathFigure() {
                        IsClosed = false,
                        IsFilled = false,
                        StartPoint = StartPoint,
                        Segments = new() {
                            new LineSegment { Point = new() { X = StartPoint.X - (sideConnections2 ? distanceX2 / 2 : 0), Y = StartPoint.Y - (sideConnections2 ? 0 : distanceY2 / 2) } },
                            new LineSegment { Point = new() { X = EndPoint.X + (sideConnections2 ? distanceX2 / 2 : 0), Y = EndPoint.Y + (sideConnections2 ? 0 : distanceY2 / 2) } },
                            new LineSegment { Point = EndPoint }
                        }
                    }
                };
                c.Path.Data = new PathGeometry() { Figures = pathFigures };
                PathFigureCollection shapePathFigures = ShapePath(c.Shape, sideConnections, DstP, distanceX, distanceY, halfDestinationWidth, halfDestinationHeight);
                c.ShapePath.Data = new PathGeometry() { Figures = shapePathFigures };
            }
        }

        private Path SetPath()
        {
            return new Path
            {
                StrokeThickness = 2
            };
        }

        private void Path_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new()
            {
                PlacementTarget = (Path)sender,
                Placement = PlacementMode.Mouse,
                IsOpen = true
            };
            Wpf.Ui.Controls.MenuItem removePath = new() { Header = "Удалить" };
            removePath.Click += RemovePath_Click;
            contextMenu.Items.Add(removePath);
        }

        private static PathFigureCollection ShapePath(ShapeEnum shape, bool sideConnections, Point DstP, double distanceX, double distanceY, double halfDestinationWidth, double halfDestinationHeight)
        {
            return new()
            {
                new PathFigure()
                {
                    IsClosed = shape is ShapeEnum.Diamond or ShapeEnum.Triangle,
                    IsFilled = true,
                    StartPoint = new() {
                        X = DstP.X + (sideConnections ? distanceX > 0 ? halfDestinationWidth + 5 : (-halfDestinationWidth) - 5 : 5),
                        Y = DstP.Y + (sideConnections ? 5 : distanceY > 0 ? halfDestinationHeight + 5 : (-halfDestinationHeight) - 5)
                    },
                    Segments = new()
                    {
                        new LineSegment { Point = new() {
                            X = DstP.X + (sideConnections ? distanceX > 0 ? halfDestinationWidth : -halfDestinationWidth : 0),
                            Y = DstP.Y + (sideConnections ? 0 : distanceY > 0 ? halfDestinationHeight : -halfDestinationHeight) }
                        },
                        new LineSegment { Point = new() {
                            X = DstP.X + (sideConnections ? distanceX > 0 ? halfDestinationWidth + 5 : (-halfDestinationWidth) - 5 : -5),
                            Y = DstP.Y + (sideConnections ? -5 : distanceY > 0 ? halfDestinationHeight + 5 : (-halfDestinationHeight) - 5) }
                        },
                        shape == ShapeEnum.Diamond ? new LineSegment
                        {
                            Point = new() {
                            X = DstP.X + (sideConnections ? distanceX > 0 ? halfDestinationWidth + 10 : (-halfDestinationWidth) - 10 : 0),
                            Y = DstP.Y + (sideConnections ? 0 : distanceY > 0 ? halfDestinationHeight + 10 : (-halfDestinationHeight) - 10) }
                        } : new LineSegment() { Point = new() {
                            X = DstP.X + (sideConnections ? distanceX > 0 ? halfDestinationWidth + 5 : (-halfDestinationWidth) - 5 : -5),
                            Y = DstP.Y + (sideConnections ? -5 : distanceY > 0 ? halfDestinationHeight + 5 : (-halfDestinationHeight) - 5) }
                        }
                    }
                }
            };
        }

        public void RemoveArrow(Path path)
        {
            ConnectionModel connection = connections.Single(c => c.Path == path);
            EditPanel.Children.Remove(connection.Path);
            EditPanel.Children.Remove(connection.ShapePath);

            LogTable[connection.DestinationTable].Columns.RemoveAt(connection.DestinationColumn);
            PhyTable[connection.DestinationTable].Columns.RemoveAt(connection.DestinationColumn);

            if(connection.DestinationTable == selectedTable && connection.DestinationColumn == selectedColumn)
            {
                selectedColumn--;
                if (LogTable[selectedTable].Columns.Count != 0 && selectedColumn == -1)
                    selectedColumn++;
                SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn]);

                RedrowTable(selectedTable, selectedCanvasElement);
            }

            connections.Remove(connection);
        }

        private void RemovePath_Click(object sender, RoutedEventArgs e)
        {
            if (isAddArrow)
            {
                isAddArrow = !isAddArrow;
                firstCanvasTable = -1;
                firstColumn = -1;
                firstTable = -1;
                ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                btnAddRow.BorderBrush = new SolidColorBrush(Color.FromArgb(0x12, 0xFF, 0xFF, 0xFF));
            }

            Path path = (Path)((ContextMenu)((Wpf.Ui.Controls.MenuItem)sender).Parent).PlacementTarget;
            RemoveArrow(path);
        }

        #endregion Arrow control

    }
}