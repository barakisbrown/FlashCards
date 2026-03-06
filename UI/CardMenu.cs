using DataLayer.Controller;
using Spectre.Console;

namespace UI;

public class CardMenu : IMenu
{
    private readonly CardController _cardController;
    private readonly StackController _stackController;
    private string[] _menu = [];

    public CardMenu(CardController cardController, StackController stackController)
    {
        _cardController = cardController;
        _stackController = stackController;
        LoadMenu(IMenu.CardMenuTitle);
    }
    
    public CardController Card { get => _cardController; init  => _cardController = value; }
    public StackController Stack { get => _stackController; init   => _stackController = value; }
    public void DisplayMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Manage Card Menu").Color(Color.Green));
            AnsiConsole.WriteLine($"Number of Cards created currently {Card.Count}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Please select a choice").AddChoices(_menu));

            switch (choice)
            {
                case "View" :
                case "Add"  :
                case "Edit" :
                case "Delete" : AnsiConsole.WriteLine("Not Implemented Yet"); break;
                case "Exit" : break;
            }
            break;
        }
    }
    public void LoadMenu(string menuName)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;

        _menu = File.ReadAllLines(Path.Combine(path, menuName));
    }
}