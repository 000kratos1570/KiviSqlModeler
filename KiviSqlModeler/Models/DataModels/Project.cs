using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models.DataModels
{
    public class Project : MyTables
    {
        public int id { get; set; }
        public string ProjectName { get; set; }
        public DateTime LastDate { get; set; }
        public string JsonProject { get; set; }
    }
}
