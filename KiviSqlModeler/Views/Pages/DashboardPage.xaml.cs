using KiviSqlModeler.Models;
using KiviSqlModeler.Views.UserControlers;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using IOStream = System.IO;
using Newtonsoft.Json;
using System.Reflection;
using KiviSqlModeler.Views.Windows;

namespace KiviSqlModeler.Views.Pages
{
    
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : UiPage
    {
        private ObservableCollection<TableModel> LogTable = new ObservableCollection<TableModel>();
        public static ObservableCollection<TableModel> PhyTable = new ObservableCollection<TableModel>();
        public static ObservableCollection<ConnectionModel> connections = new ObservableCollection<ConnectionModel>();

        public int selectedTable; // Индекс выбранной таблицы в колекции.
        public int selectedCanvasElement; // Индекс выбранной таблицы в элементах канвас.
        public int selectedColumn; // Индекс выбранного столбца

        private int firstCanvasTable = -1; // Индекс первой таблицы в элементах канвас (для создания связи).
        private int firstTable = -1; // Индекс первой таблицы в колекции (для создания связи).
        private int firstColumn = -1; // Индекс атрибута (для создания связи)                                    /// изменнить на колекцию

        /// <summary>
        /// Выбраннная модель схемы
        /// </summary>
        /// <returns></returns>
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

        public DashboardPage()
        {
            InitializeComponent();
            HideProperies();
        }

        #region WorckPlace

        /// <summary>
        /// запоминание нового раположения элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableUC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)   // Добавить создание нескольких ВК по нескольким ПК
        {
            LogTable[GetTableIndex(sender)].Left = Canvas.GetLeft((TableUC)sender);
            LogTable[GetTableIndex(sender)].Top = Canvas.GetTop((TableUC)sender);
            PhyTable[GetTableIndex(sender)].Left = Canvas.GetLeft((TableUC)sender);
            PhyTable[GetTableIndex(sender)].Top = Canvas.GetTop((TableUC)sender);

            if (btnAddRow.IsChecked == true)
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
                    else if (firstTable != index)
                    {
                        string name = ((TableUC)EditPanel.Children[firstCanvasTable]).NameTable;
                        LogTable[index].Columns.Add(new ColumnModel(name, "int", ColumnModel.isNull.Null, ColumnModel.PKFK.fk));
                        PhyTable[index].Columns.Add(new ColumnModel(name, "int", ColumnModel.isNull.Null, ColumnModel.PKFK.fk));

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

            ContextMenu contextMenu = new();
            Wpf.Ui.Controls.MenuItem removePath = new() { Header = "Удалить" };
            removePath.Click += RemoveTable;
            contextMenu.Items.Add(removePath);
            myTable.ContextMenu = contextMenu;
            myTable.ContextMenuOpening += MyTable_ContextMenuOpening;

            Canvas.SetTop(myTable, tableModel.Top);
            Canvas.SetLeft(myTable, tableModel.Left);
            return myTable;
        }

        private TableUC deleteTable;

        private void MyTable_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TableUC table = sender as TableUC;
            if (table != null)
            {
                deleteTable = table;
            }
        }

        private void RemoveTable(object sender, RoutedEventArgs e)
        {
            int indexTable = GetTableIndex(deleteTable);
            int indexCanvasElement = EditPanel.Children.IndexOf(deleteTable);

            // удаление связей

            foreach (var con in connections)
            {
                if (con.SourceTable == indexTable || con.DestinationTable == indexTable)
                {
                    RemoveArrow(con.Path);
                    if (connections.Count == 0)
                        break;
                }
            }

            if (btnAddRow.IsChecked == true && firstCanvasTable != -1)
            {
                ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                btnAddRow.IsChecked = false;
                firstCanvasTable = -1;
                firstColumn = -1;
                firstTable = -1;
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
                    c.DestinationTable = selectedTable;
                }
            }
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
        float scaleFactor = 1;

        private void EditPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scaleFactor = Zoomfactor;
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

            //CreateArrowBetween();
        }

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

                //var sUC = (TableUC)EditPanel.Children[c.CIndexCanvasTable];
                //var dUC = (TableUC)EditPanel.Children[c.DIndexCanvasTable];
                //double halfSourceWidth = sUC.ActualWidth / 2;
                //double halfSourceHeight = sUC.ActualHeight / 2;
                //double halfDestinationWidth = dUC.ActualWidth / 2;
                //double halfDestinationHeight = dUC.ActualHeight / 2;
                //double SrcX = Canvas.GetLeft(sUC) + (sUC.ActualWidth / 2);
                //double SrcY = Canvas.GetTop(sUC) + (sUC.ActualHeight / 2);
                //double DstX = Canvas.GetLeft(dUC) + halfDestinationWidth;
                //double DstY = Canvas.GetTop(dUC) + halfDestinationHeight;

                var brash = Application.Current.FindResource("TextFillColorPrimaryBrush") as SolidColorBrush;

                if (c.Path is null)
                {
                    c.Path = SetPath();
                    c.Path.Stroke = brash;

                    ContextMenu contextMenu = new();
                    Wpf.Ui.Controls.MenuItem removePath = new() { Header = "Удалить" };
                    removePath.Click += RemovePath_Click;
                    contextMenu.Items.Add(removePath);
                    c.Path.ContextMenu = contextMenu;
                    c.Path.ContextMenuOpening += Path_ContextMenuOpening; ;

                    c.Path.RenderTransform = _transform;
                    EditPanel.Children.Add(c.Path);
                }
                if (c.ShapePath is null)
                {
                    c.ShapePath = SetPath();
                    c.ShapePath.Stroke = brash;
                    c.ShapePath.RenderTransform = _transform;
                    EditPanel.Children.Add(c.ShapePath);
                }
                c.Path.Stroke = brash;
                c.ShapePath.Stroke = brash;
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
                c.Path.Data = new PathGeometry() { Figures = pathFigures};
                PathFigureCollection shapePathFigures = ShapePath(c.Shape, sideConnections, DstP, distanceX, distanceY, halfDestinationWidth, halfDestinationHeight);
                c.ShapePath.Data = new PathGeometry() { Figures = shapePathFigures };

                
            }
        }

        private Path deleteArrow;

        private void Path_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Path arrow = sender as Path;
            if (arrow != null)
            {
                deleteArrow = arrow;
            }
        }

        private Path SetPath()
        {
            return new Path
            {
                StrokeThickness = 2
            };
        }

        private PathFigureCollection ShapePath(ShapeEnum shape, bool sideConnections, Point DstP, double distanceX, double distanceY, double halfDestinationWidth, double halfDestinationHeight)
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

        public void RemoveArrow(Path deleteArrow)
        {
            ConnectionModel connection = connections.FirstOrDefault(c => c.Path == deleteArrow);
            EditPanel.Children.Remove(connection.Path);
            EditPanel.Children.Remove(connection.ShapePath);

            LogTable[connection.DestinationTable].Columns.RemoveAt(connection.DestinationColumn);
            PhyTable[connection.DestinationTable].Columns.RemoveAt(connection.DestinationColumn);

            if (connection.DestinationTable == selectedTable && connection.DestinationColumn == selectedColumn)
            {
                selectedColumn--;
                if (LogTable[selectedTable].Columns.Count != 0 && selectedColumn == -1)
                    selectedColumn++;
                if (selectedColumn == -1)
                {
                    tbColumnName.Text = "";
                    tbColumnType.Text = "";
                    cbIsNull.IsChecked = true;
                    cbPK.IsChecked = false;
                }
                else
                    SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn], selectedColumn);

                RedrowTable(selectedTable, selectedCanvasElement);
            }

            connections.Remove(connection);
        }

        private void RemovePath_Click(object sender, RoutedEventArgs e)
        {
            if (btnAddRow.IsChecked == true && firstCanvasTable != -1)
            {
                ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                btnAddRow.IsChecked = false;
                firstCanvasTable = -1;
                firstColumn = -1;
                firstTable = -1;
            }

            RemoveArrow(deleteArrow);
        }

        #endregion WorckPlace

        #region Tools

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Kiv modeller json (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                EditPanel.Children.Clear();
                LogTable = new ObservableCollection<TableModel>();
                PhyTable = new ObservableCollection<TableModel>();
                connections = new ObservableCollection<ConnectionModel>();

                var json = IOStream.File.ReadAllText(openFileDialog.FileName);
                DataKivModel data = JsonConvert.DeserializeObject<DataKivModel>(json);
                LogTable = data.LogTableFile;
                PhyTable = data.PhyTableFile;


                if (PhyTable.Count == 0) return;
                foreach (var table in GetModel())
                {
                    TableUC myTable = GetTableUC(table);
                    EditPanel.Children.Add(myTable);
                }
                selectedCanvasElement = -1;
                selectedColumn = -1;
                selectedTable = -1;
                HideProperies();

                connections = data.LoadConnections(EditPanel);
                CreateArrowBetween();
            }
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "KivModel";
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.Filter = "Kiv modeller json (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                DataKivModel dataKivModel = new DataKivModel();

                dataKivModel.PhyTableFile = PhyTable;
                dataKivModel.LogTableFile = LogTable;
                dataKivModel.SaveConnections(connections);

                string json = JsonConvert.SerializeObject(dataKivModel);
                IOStream.File.WriteAllText(saveFileDialog.FileName, json);
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
            tableModel.Columns.Add(new ColumnModel("column0", "int", ColumnModel.isNull.NotNull, ColumnModel.PKFK.pk));
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

        /// <summary>
        /// Создание связей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            if (btnAddRow.IsChecked == false && firstCanvasTable != -1)
            {
                ((TableUC)EditPanel.Children[firstCanvasTable]).dgTable.SelectedIndex = -1;
                firstCanvasTable = -1;
                firstColumn = -1;
                firstTable = -1;
            }
        }

        /// <summary>
        /// Выбор модели схемы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditPanel == null) return;
            foreach (var table in EditPanel.Children.OfType<TableUC>().ToList())
            {
                int selectedCanvasElement = EditPanel.Children.IndexOf(table);
                int selectedTable = EditPanel.Children.OfType<TableUC>().ToList().IndexOf(table);
                RedrowTable(selectedTable, selectedCanvasElement);
            }
            selectedCanvasElement = -1;
            selectedColumn = -1;
            selectedTable = -1;
            HideProperies();
        }


        #endregion Tools

        #region MenuData


        /// <summary>
        /// Заполнение меню данных выбранной таблицой.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyTable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisibleProperies();
            int index = GetTableIndex(sender);
            SelectedTable(index, EditPanel.Children.IndexOf((TableUC)sender));
            if (GetModel()[index].Columns.Count == 0)
                return;
            SelectedColumn(GetModel()[index].Columns[0],0);
            selectedColumn = 0;
        }

        /// <summary>
        /// Заполняет выбранный столбец в меню данных.
        /// </summary>
        /// <param name="columnModel"></param>
        public void SelectedColumn(ColumnModel columnModel, int index)
        {
            tbColumnName.Text = columnModel.Name;
            tbColumnType.Text = columnModel.Type;
            cbIsNull.IsChecked = columnModel.IsNull == ColumnModel.isNull.Null ? true : false;
            cbPK.IsChecked = (columnModel.Pkfk == ColumnModel.PKFK.pk || columnModel.Pkfk == ColumnModel.PKFK.pkfk) ? true : false;
            lbIndexColumn.Content = index;
        }

        /// <summary>
        /// Заполняет выбранную таблицу в меню данных.
        /// </summary>
        /// <param name="index"></param>
        public void SelectedTable(int index, int indexCanvasElement)
        {
            selectedCanvasElement = indexCanvasElement;
            selectedTable = index;
            tbTableName.Text = GetModel()[index].Name;
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
                var D = connections.FirstOrDefault(c => c.DestinationTable == selectedTable);

                if (D != null)
                {
                    if (D.DestinationColumn == selectedColumn)
                    {
                        RemoveConnection(D);
                        return;
                    }

                    if (selectedColumn < D.DestinationColumn)
                    {
                        D.DestinationColumn--;
                        connections[connections.IndexOf(D)] = D;
                    }
                }

                LogTable[selectedTable].Columns.RemoveAt(selectedColumn);
                PhyTable[selectedTable].Columns.RemoveAt(selectedColumn);

                selectedColumn--;
                if (LogTable[selectedTable].Columns.Count != 0 && selectedColumn == -1)
                    selectedColumn++;
                if (selectedColumn == -1)
                {
                    tbColumnName.Text = "";
                    tbColumnType.Text = "";
                    cbIsNull.IsChecked = true;
                    cbPK.IsChecked = false;
                }
                else
                    SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn], selectedColumn);

                RedrowTable(selectedTable, selectedCanvasElement);
            }
        }

        private void RemoveConnection(ConnectionModel C)
        {
            while (C.DestinationColumn == selectedColumn)
            {
                RemoveArrow(C.Path);
                C = connections.FirstOrDefault(c => c.SourceTable == selectedTable);
                if (C == null)
                    break;
            }
        }

        /// <summary>
        /// Добавление столбца в выбранную таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            LogTable[selectedTable].Columns.Add(new ColumnModel("column" + LogTable[selectedTable].Columns.Count, ""));
            PhyTable[selectedTable].Columns.Add(new ColumnModel("column" + LogTable[selectedTable].Columns.Count, ""));

            selectedColumn = LogTable[selectedTable].Columns.Count - 1;
            SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn], selectedColumn);

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
            SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn], selectedColumn);
        }

        /// <summary>
        /// Заполнения меню данных следующим столбце
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextColumn_Click(object sender, RoutedEventArgs e)
        {
            selectedColumn = selectedColumn == LogTable[selectedTable].Columns.Count - 1 ? 0 : selectedColumn + 1;
            SelectedColumn(GetModel()[selectedTable].Columns[selectedColumn], selectedColumn);
        }

        /// <summary>
        /// Сохраняет изменения таблица (названия и выбранного столбца)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveTable_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception)
            {

            }
        }


        #endregion MenuData

        private void btnOpenDB_Click(object sender, RoutedEventArgs e)
        {
            OpenProjectWindow openProjectWindow = new OpenProjectWindow(false, "");
            if (openProjectWindow.ShowDialog() == true)
            {
                string json = openProjectWindow.Json;

                EditPanel.Children.Clear();
                LogTable = new ObservableCollection<TableModel>();
                PhyTable = new ObservableCollection<TableModel>();
                connections = new ObservableCollection<ConnectionModel>();

                DataKivModel data = JsonConvert.DeserializeObject<DataKivModel>(json);
                LogTable = data.LogTableFile;
                PhyTable = data.PhyTableFile;


                if (PhyTable.Count == 0) return;
                foreach (var table in GetModel())
                {
                    TableUC myTable = GetTableUC(table);
                    EditPanel.Children.Add(myTable);
                }
                selectedCanvasElement = -1;
                selectedColumn = -1;
                selectedTable = -1;
                HideProperies();

                connections = data.LoadConnections(EditPanel);
                CreateArrowBetween();
            }
        }

        private void btnSavDB_Click(object sender, RoutedEventArgs e)
        {
            DataKivModel dataKivModel = new DataKivModel();

            dataKivModel.PhyTableFile = PhyTable;
            dataKivModel.LogTableFile = LogTable;
            dataKivModel.SaveConnections(connections);

            string json = JsonConvert.SerializeObject(dataKivModel);
            new OpenProjectWindow(true, json).ShowDialog();
        }

        private void UiPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (AuthProfile.IsLoggedIn())
            {
                btnOpenDB.IsEnabled = true;
                btnSavDB.IsEnabled = true; 
            }
            else
            {
                btnOpenDB.IsEnabled = false;
                btnSavDB.IsEnabled = false;
            }
        }
    }
}