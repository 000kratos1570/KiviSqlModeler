using KiviSqlModeler.Views.UserControlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KiviSqlModeler.Models
{
    public class DataKivModel
    {
        public ObservableCollection<TableModel> LogTableFile { get; set; }
        public ObservableCollection<TableModel> PhyTableFile { get; set; }

        // Данные связей для сохранения
        public ObservableCollection<ShapeEnum> Shape { get; set; } = new ObservableCollection<ShapeEnum>();
        public ObservableCollection<bool> Dashed { get; set; } = new ObservableCollection<bool>();
        public ObservableCollection<int> CIndexCanvasTable { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> DIndexCanvasTable { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> SourceTable { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> SourceColumn { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> DestinationTable { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<int> DestinationColumn { get; set; } = new ObservableCollection<int>();



        public void SaveConnections(ObservableCollection<ConnectionModel> connections)
        {

            foreach (var connection in connections)
            {
                Shape.Add(connection.Shape);
                Dashed.Add(connection.Dashed);
                CIndexCanvasTable.Add(connection.CIndexCanvasTable);
                DIndexCanvasTable.Add(connection.DIndexCanvasTable);
                SourceTable.Add(connection.SourceTable);
                SourceColumn.Add(connection.SourceColumn);
                DestinationTable.Add(connection.DestinationTable);
                DestinationColumn.Add(connection.DestinationColumn);
            }
        }

        public ObservableCollection<ConnectionModel> LoadConnections(Canvas EditPanel)
        {
            ObservableCollection<ConnectionModel> connections = new ObservableCollection<ConnectionModel>();
            for (int i = 0; i < SourceTable.Count; i++)
            {
                ConnectionModel connectionModel = new ConnectionModel()
                {
                    Source = (TableUC)EditPanel.Children[CIndexCanvasTable[i]],
                    Destination = (TableUC)EditPanel.Children[DIndexCanvasTable[i]],
                    Dashed = false,
                    Shape = ShapeEnum.Arrow,
                    DIndexCanvasTable = DIndexCanvasTable[i],
                    CIndexCanvasTable = CIndexCanvasTable[i],
                    SourceTable = SourceTable[i],
                    DestinationTable = DestinationTable[i],
                    SourceColumn = SourceColumn[i],
                    DestinationColumn = DestinationColumn[i],
                };
                connections.Add(connectionModel);
            }

            return connections;
        }
    }
}
