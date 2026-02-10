using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models.DataModels
{
    public class ProfileProject : MyTables
    {
        public int id { get; set; }
        public int ProfileID { get; set; }
        public int ProjectID { get; set; }
    }
}
