using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Model
{
    public class TableModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // первичный ключ
        public Column pk { get; set; }

        // коллекция столбцев
        public ObservableCollection<Column> Columns = new ObservableCollection<Column>();

        // коллекция связей
        private ObservableCollection<TableModel>? FKTable;

        public void AddFKTo(TableModel tableModel)
        {
            if (FKTable == null)
            {
                FKTable = new ObservableCollection<TableModel>();
            }
            if (tableModel.pk == null)
            {
                throw new Exception("Первичный ключ не указан");
            }
            Columns.Add(new Column(tableModel.pk.name, tableModel.pk.type, Column.PKFK.fk));
            FKTable.Add(tableModel);
        }

        public void RemoveFKTo(TableModel tableModel)
        {
            if (FKTable == null)
            {
                throw new Exception("Таблица не содержит связи");
            }
            if (!FKTable.Contains(tableModel))
            {
                throw new Exception("Таблица не содержит указанную связь для удаленя");
            }
            Columns.Remove(new Column(tableModel.pk.name, tableModel.pk.type, Column.PKFK.fk));
            FKTable.Remove(tableModel);
        }

        public TableModel(int id, string name)
        {
            this.Id = id;
            Name = name;
        }
    }
}
