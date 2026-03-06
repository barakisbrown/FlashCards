using DataLayer.Controller;

namespace UI;

public interface IMenu
{
    public const string CardMenuTitle = "FlashCard-Menu.txt";
    public const string StackMenuTitle = "Stack-Menu.txt";
    public const string EditMenuTitle = "Edit-FlashCard-Menu.txt";
    public const string StudySessionTItle = "StudySession-Menu.txt";
    public CardController Card { get; init; }
    public StackController Stack { get; init; }
    public void DisplayMenu();

    public void LoadMenu(string menuName);
}