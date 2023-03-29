﻿using KiviSqlModeler.Views.UserControlers;
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
    internal class ConnectionModel
    {
        public TableUC Source { get; set; }
        public TableUC Destination { get; set; }
        public Path? Path { get; set; }
        public Path? ShapePath { get; set; }
        public ShapeEnum Shape { get; set; }
        public bool Dashed { get; set; }
        //public bool Fill { get; set; }

        public int CIndexCanvasTable { get; set; }
        public int DIndexCanvasTable { get; set; }
        public TableModel SourceTable { get; set; }
        public ColumnModel SourceColumn { get; set; }
        public TableModel DestinationTable { get; set; }
        public ColumnModel DestinationColumn { get; set; }
    }

    public enum ShapeEnum
    { Arrow, Triangle, Diamond }
}
