using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models.DataModels
{
    public class DolgnostEntity : MyTables
    {
        public int id { get; set; }
        public string Dolgnost { get; set; }
    }
}
