using KiviSqlModeler.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Wpf.Ui.Controls;

namespace KiviSqlModeler.Views.Pages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataPage : UiPage
    {
        private ObservableCollection<TableModel> PhyTable = new ObservableCollection<TableModel>();
        private ObservableCollection<ConnectionModel> Connections = new ObservableCollection<ConnectionModel>();
        public Dictionary<string, string> TablesCode = new Dictionary<string, string>();

        public DataPage()
        {
            InitializeComponent();
        }

        private void UiPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ObservableCollection<TableModel> phyTable = DashboardPage.PhyTable;
            ObservableCollection<ConnectionModel> connections = DashboardPage.connections;
            Connections = connections;
            PhyTable = phyTable;
            GenerateSql();
        }

        public void GenerateSql()
        {
            if (PhyTable.Count == 0)
            {
                return;
            }

            
            foreach(var table in PhyTable)
            {
                try
                {
                    infinityError = 0;
                    if (!TablesCode.ContainsKey(table.Name))
                        TablesCode.Add(table.Name, GetTableCode(table));
                }
                catch
                {
                    System.Windows.MessageBox.Show("Связи зациклины", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    TablesCode = new Dictionary<string, string>();
                    return;
                }
                
            }

            string AllCode = "";
            foreach(var tebleCode in TablesCode)
            {
                AllCode += tebleCode.Value;
            }

            tbSQL.Text = AllCode;
        }

        public int infinityError;

        private string GetTableCode(TableModel table)
        {
            infinityError++;
            if (infinityError > PhyTable.Count)
            {
                return null;
            }
            string ThisTableCode = new string($"CREATE TABLE {table.Name}\n(\n");
            foreach (var atrebut in table.Columns)
            {
                if (atrebut.Pkfk == ColumnModel.PKFK.pk)
                {
                    ThisTableCode += $"  {atrebut.Name} SERIAL {IsNull(atrebut.IsNull)} constraint PK_{atrebut.Name} primary key,";
                }
                else if (atrebut.Pkfk == ColumnModel.PKFK.none)
                {
                    ThisTableCode += $" {atrebut.Name} {atrebut.Type} {IsNull(atrebut.IsNull)},";
                }
                else if (atrebut.Pkfk == ColumnModel.PKFK.fk || atrebut.Pkfk == ColumnModel.PKFK.pkfk)
                {
                    ConnectionModel ThisConnecrion = Connections.FirstOrDefault(x => x.DestinationTable == PhyTable.IndexOf(table));
                    ThisTableCode += $" {atrebut.Name} {atrebut.Type} {IsNull(atrebut.IsNull)} references {PhyTable[ThisConnecrion.SourceTable].Name}({PhyTable[ThisConnecrion.SourceTable].Columns[ThisConnecrion.SourceColumn].Name}),";
                    if (!TablesCode.ContainsKey(PhyTable[ThisConnecrion.SourceTable].Name))
                        TablesCode.Add(PhyTable[ThisConnecrion.SourceTable].Name, GetTableCode(PhyTable[ThisConnecrion.SourceTable]));
                }
                ThisTableCode += "\n";
            }
            ThisTableCode += ");\n\n";
            return ThisTableCode;
        }

        public string IsNull(ColumnModel.isNull isNull)
        {
            if (ColumnModel.isNull.NotNull == isNull)
            {
                return "NOT NULL";
            }
            else
            {
                return "NULL";
            }
        }
    }
}
