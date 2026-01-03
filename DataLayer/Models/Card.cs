namespace DataLayer.Models
{
    /// <summary>
    /// FlashCard Entity
    /// </summary>
    /// <param name="CardID">ID OF THE CARD</param>
    /// <param name="Front">INFO ON FRONT</param>
    /// <param name="Back">INFO ON BACK</param>
    /// <param name="StackID">ID to the stack this card belongs too</param>
    public record class Card(int CardID,string? Front,string? Back,int? StackID);
}
