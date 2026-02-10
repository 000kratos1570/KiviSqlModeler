using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models.DataModels
{
    public class Profile : MyTables
    {
        public int id { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public int DolgnostID { get; set; }
    }
}
