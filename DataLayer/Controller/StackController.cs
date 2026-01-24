using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Controller
{    
    internal static class StackController
    {
        private static readonly DbConfig appSettings = Configuration.LoadSettings();
        private static readonly string connectionStringMain = Configuration.GetConnectionStrings(appSettings.MainConn);

        public static bool CreateStack(Stack stack)
        {
            throw new NotImplementedException();
        }

        public static bool UpdatedStack(Stack stack)
        {
            throw new NotImplementedException();
        }

        public static bool DeleteStack(Stack stack)
        {
            throw new NotImplementedException();
        }

        public static List<Stack> GetAllStacks()
        {
            throw new NotImplementedException();
        }
    }
}
