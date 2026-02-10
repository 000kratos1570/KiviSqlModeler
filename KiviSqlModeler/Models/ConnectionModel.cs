using KiviSqlModeler.Views.UserControlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace KiviSqlModeler.Models
{
    public class ConnectionModel
    {
        public TableUC Source { get; set; }
        public TableUC Destination { get; set; }
        public Path? Path { get; set; }
        public Path? ShapePath { get; set; }
        public ShapeEnum Shape { get; set; }
        public bool Dashed { get; set; }
        //public bool Fill { get; set; } 
        /// <summary>
        /// Индекс таблицы в канвасе.
        /// </summary>
        public int CIndexCanvasTable { get; set; }
        /// <summary>
        /// Индекс таблицы в канвасе
        /// </summary>
        public int DIndexCanvasTable { get; set; }
        /// <summary>
        /// Индекс таблицы в коллекции
        /// </summary>
        public int SourceTable { get; set; } 
        public int SourceColumn { get; set; }
        /// <summary>
        /// Индекс таблицы в коллекции
        /// </summary>
        public int DestinationTable { get; set; }  
        public int DestinationColumn { get; set; }
    }

    public enum ShapeEnum
    { Arrow, Triangle, Diamond }
}
