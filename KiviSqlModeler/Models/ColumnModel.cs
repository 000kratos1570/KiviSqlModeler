using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models
{
    public class ColumnModel
    {
        public enum PKFK { pk, fk, pkfk, none };
        public enum isNull { Null, NotNull};

        public string Name { get; set; }
        public string Type { get; set; }
        public PKFK Pkfk { get; set; }
        public isNull IsNull { get; set; }

        public ColumnModel(string name)
        {
            this.Name = name;
        }

        public ColumnModel(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        public ColumnModel(string name, string type, isNull isNull)
        {
            this.Name = name;
            this.Type = type;
            IsNull = isNull;
        }

        public ColumnModel(string name, string type, isNull isNull, PKFK value)
        {
            this.Name = name;
            this.Type = type;
            this.Pkfk = value;
            this.IsNull = isNull;
        }
    }
}
