using Dapper;
using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace DataLayer.Controller
{
    public class CardController
    {
        private readonly static DbUser DataSource = Configuration.GetUserSecretsConnStrings();
        private const string SelectAllSql = "SELECT * FROM dbo.Cards";
        private const string InsertSql = "INSERT INTO dbo.Cards(Prompt, Answer, StackID) VALUES(@PROMPT, @ANSWER, @CARDID)";
        private const string UpdateSql = "Update dbo.cards SET Prompt = @Prompt, Answer = @Answer, StackID = @StackID" + " WHERE dbo.cards.ID = @ID";
        private const string DeleteSql = "DELETE FROM dbo.Cards WHERE dbo.Cards.ID = @ID";
        private const string CardsPerStackSql = "SELECT COUNT(*) FROM Cards WHERE Cards.StackID = @fkey";
        // FIX CONNECTION STRING ISSUES
        private List<Card> _cards = [];
        public CardController()
        {
            LoadAllRecords();
        }

        public bool AddCard(string? prompt,string? answer,int? cardId = 1)
        {
            // CHECK IF EITHER STRING IS NULL OR EMPTY
            if (prompt.IsNullOrEmpty() || answer.IsNullOrEmpty())
                return false;            
            using var conn = MakeConnection;
            object[] param = { new { prompt, answer, cardId } };
            var success = conn.Execute(InsertSql, param) == 1;
            if (success)
            {
                Synced = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddCard(Card card)
        {
            // CHECK IF EITHER STRING IS NULL OR EMPTY
            if (card.Prompt.IsNullOrEmpty() || card.Answer.IsNullOrEmpty())
                return false;

            return AddCard(card.Prompt, card.Answer, card.StackID);
        }

        public bool EditCard(Card editedCard)
        {
            var success = false;
            // INTERNAL LIST
            var card = _cards.FirstOrDefault(x => x.ID == editedCard.ID);
            if (card == null)
                return success;
            else
            {
                card.Prompt = editedCard.Prompt;
                card.Answer = editedCard.Answer;
                card.StackID = editedCard.StackID;
                success = true;
            }
            // DATABASE
            using var conn = MakeConnection;
            object[] param = { new{editedCard.Prompt, editedCard.Answer, editedCard.StackID, editedCard.ID } };
            var update = conn.Execute(UpdateSql, param) == 1;
            if (update && success)
            {
                Synced = true;
                return true;
            }
            else
                return false;
        }

        public bool DeleteCard(Card deleteMe) 
        {
            bool retValue;
            using var conn = MakeConnection;
            object[] param = [new {deleteMe.ID }];
            var deleted = conn.Execute(DeleteSql, param) == 1;
            if (deleted)
            {
                LoadAllRecords();
                retValue = true;
            }
            else
                retValue = false;

            return retValue;
            
        }

        public Card? GetCardById(int ID) => _cards.FirstOrDefault(x => x.ID == ID);
        public List<Card> GetAllCardsByStack(int stackId) => _cards.Where(x => x.StackID == stackId).ToList();


        private void LoadAllRecords()
        {            
            using var conn = MakeConnection;
            _cards = conn.Query<Card>(SelectAllSql).ToList();
            Synced = true;
        }

        public List<Card> GetAllCards()
        {
            List<Card> tmpCards;
            if (!Synced)
            {
                LoadAllRecords();
                tmpCards = _cards;
                Synced = true;
            }
            else
                tmpCards = _cards;
            
            return tmpCards;
        }

        private bool Synced { get; set; } = false;

        private SqlConnection MakeConnection
        {
            get
            {
                var conn = new SqlConnection(DataSource.Main);
                if (conn.State != System.Data.ConnectionState.Open)
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        AnsiConsole.WriteLine("Error problem opening connection to the database engine. CHeck to see if it running.");
                        throw;
                    }
                return conn;
            }
        }

        public int Count => _cards.Count;

        /// <summary>
        /// Gets the number of cards in the stack identified by the specified key.
        /// </summary>
        /// <param name="fkey">The unique identifier of the stack for which to count the cards.</param>
        /// <returns>The total number of cards in the specified stack.</returns>
        public int GetNumberCardsInStack(int fkey)
        {
            using var conn = MakeConnection;
            Object parm = new { fkey };

            var total = conn.ExecuteScalar<int>(CardsPerStackSql, parm);
            return total;
        }

        public bool CardExist(Card selCard)
        {
            var list = GetAllCardsByStack(selCard.StackID).Any(x => x.Prompt.Equals(selCard.Prompt) && x.Answer.Equals(selCard.Answer));
            return list;
            
        }
    }
}
