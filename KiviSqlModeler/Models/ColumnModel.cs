using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KiviSqlModeler.Models.ColumnModel;

namespace KiviSqlModeler.Models
{
    public class ColumnModel
    {
        public enum PKFK
        {
            pk, fk, pkfk, none
        };

        public enum isNull { Null, NotNull };

        public string Name { get; set; }
        public string Type { get; set; }
        public string PkfkString { get; private set; }
        private PKFK _pkfk;
        public PKFK Pkfk
        {
            get { return _pkfk; }
            set
            {
                _pkfk = value;
                switch (_pkfk)
                {
                    case PKFK.pk:
                        PkfkString = "ПК";
                        break;
                    case PKFK.fk:
                        PkfkString = "ВК";
                        break;
                    case PKFK.pkfk:
                        PkfkString = "ПВК";
                        break;
                    case PKFK.none:
                        PkfkString = "";
                        break;
                }
            }
        }
        public isNull IsNull { get; set; }

        public ColumnModel(string name, string type, isNull isNull = isNull.Null, PKFK value = PKFK.none)
        {
            this.Name = name;
            this.Type = type;
            this.Pkfk = value;
            this.IsNull = isNull;
        }
    }
}
