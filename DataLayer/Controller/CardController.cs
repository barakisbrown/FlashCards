using Dapper;
using DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace DataLayer.Controller
{
    public class CardController
    {
        private static readonly DbUser dataSource = Configuration.GetUserSecretsConnStrings();
        private readonly string selectAllSql = "SELECT * FROM dbo.Cards";
        private readonly string insertSQL = "INSERT INTO dbo.Cards(Prompt, Answer, StackID) VALUES(@PROMPT, @ANSWER, @CARDID)";
        private readonly string updateSQL = "Update dbo.cards SET Prompt = @Prompt, Answer = @Answer, StackID = @StackID" +
                " WHERE dbo.cards.ID = @ID";
        private readonly string deleteSQL = "DELETE FROM dbo.Cards WHERE dbo.Cards.ID = @ID";
        private readonly string cardsPerStackSQL = "SELECT COUNT(*) FROM Cards WHERE Cards.StackID = @fkey";
        // FIX CONNECTION STRING ISSUES
        private List<Card> cards = [];
        private bool synced = false;
        public CardController()
        {
            LoadAllRecords();
        }

        public bool AddCard(string? Prompt,string? Answer,int? CardID = 1)
        {
            // CHECK IF EITHER STRING IS NULL OR EMPTY
            if (Prompt.IsNullOrEmpty() || Answer.IsNullOrEmpty())
                return false;            
            using var conn = MakeConnection;
            object[] param = { new { Prompt, Answer, CardID } };
            bool success = conn.Execute(insertSQL, param) == 1;
            if (success)
            {
                synced = false;
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
            
            var conn = MakeConnection;
            object[] parm = { new { card.Prompt, card.Answer, card.StackID } };
            bool success = conn.Execute(insertSQL, parm) == 1;
            if (success)
            {
                SYNCED = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EditCard(Card editedCard)
        {
            bool success = false;
            // INTERNAL LIST
            var card = cards.FirstOrDefault(x => x.ID == editedCard.ID);
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
            Object[] param = { new{editedCard.Prompt, editedCard.Answer, editedCard.StackID, editedCard.ID } };
            bool update = conn.Execute(updateSQL, param) == 1;
            if (update && success)
            {
                SYNCED = true;
                return true;
            }
            else
                return false;
        }

        public bool DeleteCard(Card deleteMe) 
        {
            bool retValue;
            using var conn = MakeConnection;
            Object[] param = [new {deleteMe.ID }];
            bool deleted = conn.Execute(deleteSQL, param) == 1;
            if (deleted)
            {
                LoadAllRecords();
                retValue = true;
            }
            else
                retValue = false;

            return retValue;
            
        }

        public Card? GetCardByID(int ID) => cards.FirstOrDefault(x => x.ID == ID);
        public List<Card> GetAllCardsByStack(int stackID) => cards.Where(x => x.StackID == stackID).ToList();


        private void LoadAllRecords()
        {            
            using var conn = MakeConnection;
            cards = conn.Query<Card>(selectAllSql).ToList();
            SYNCED = true;
        }

        public List<Card> GetAllCards()
        {
            List<Card> tmpCards;
            if (!SYNCED)
            {
                LoadAllRecords();
                tmpCards = cards;
                SYNCED = true;
            }
            else
                tmpCards = cards;
            
            return tmpCards;
        }

        private bool SYNCED { get => synced; set => synced = value; }

        private SqlConnection MakeConnection
        {
            get
            {
                var conn = new SqlConnection(dataSource.Main);
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

        public int Count => cards.Count;

        /// <summary>
        /// Gets the number of cards in the stack identified by the specified key.
        /// </summary>
        /// <param name="fkey">The unique identifier of the stack for which to count the cards.</param>
        /// <returns>The total number of cards in the specified stack.</returns>
        public int GetNumberCardsInStack(int fkey)
        {
            using var conn = MakeConnection;
            Object parm = new { fkey };

            var total = conn.ExecuteScalar<int>(cardsPerStackSQL, parm);
            return total;
        }
    }
}
