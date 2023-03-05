using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Model
{
    public class Column
    {
        public enum PKFK { pk, fk};

        public string name {  get; set; }
        public string type {  get; set; }
        public string? pkfk { get; set; }

        public Column(string name, string type)
        {
            this.name = name;
            this.type = type;
            pkfk = null;
        }

        public Column(string name, string type, PKFK value)
        {
            this.name = name;
            this.type = type;
            switch (value)
            {
                case PKFK.pk:
                    {
                        pkfk = "ПК";
                        break;
                    }
                case PKFK.fk:
                    {
                        pkfk = "ВК";
                        break;
                    }
            }
        }
    }
}
