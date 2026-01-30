using DataLayer.Models;

namespace DataLayer.Controller
{
    internal static class CardController
    {
        private static readonly DbConfig appSettings = Configuration.LoadSettings();
        // FIX CONNECTION STRING ISSUES

        public static bool CreateCard(Card card)
        {
            throw new NotImplementedException();
        }

        public static bool UpdatedCard(Card card)
        {
            throw new NotImplementedException();
        }

        public static bool DeleteCard(Card card, int id) 
        {
            throw new NotImplementedException();
        }

        public static List<Card> GetAllCards() 
        {
            throw new NotImplementedException();
        }
    }
}
