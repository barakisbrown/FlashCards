using DataLayer.Models;

namespace DataLayer.Controller
{    
    internal static class StackController
    {
        private static readonly DbConfig appSettings = Configuration.LoadSettings();
        // FIX CONNECTION STRING ISSUES

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
