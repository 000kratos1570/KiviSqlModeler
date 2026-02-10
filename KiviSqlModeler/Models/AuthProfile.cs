using KiviSqlModeler.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiviSqlModeler.Models
{
    public static class AuthProfile
    {
        public static int AuthProfileID { get; set; } = -1;
        public static int AuthDolgnostID { get; set; }
        public static string AuthProfileEmail { get; set; }
        public static string AuthProfilePassword { get; set; }

        public static void LogOut()
        {
            AuthProfileID = -1;
            AuthProfileEmail = "";
            AuthProfilePassword = "";
            AuthDolgnostID = -1;
        }

        public static bool IsLoggedIn()
        {
            return AuthProfileID > -1;
        }

        public static void Set(Profile profile)
        {
            AuthProfileID = profile.id;
            AuthProfileEmail = profile.email;
            AuthProfilePassword = profile.Password;
            AuthDolgnostID = profile.DolgnostID;
        }
    }
}
