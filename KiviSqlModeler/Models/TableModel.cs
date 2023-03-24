using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models
{
    public class TableModel
    {
        /// <summary>
        /// Название таблицы
        /// </summary>
        public string Name { get; set; }

        public double Left { get; set; }

        public double Top { get; set; }

        /// <summary>
        /// Колеекция атрибутов таблицы
        /// </summary>
        public ObservableCollection<ColumnModel> Columns { get; set; } = new ObservableCollection<ColumnModel>();

        // коллекция связей
        public ObservableCollection<TableModel> FKTable = new ObservableCollection<TableModel>();

        public TableModel(string name, double left = 0, double top = 0)
        {
            Name = name;
            Left = left;
            Top = top;
        }
    }
}
