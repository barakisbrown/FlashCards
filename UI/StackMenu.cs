using DataLayer.Controller;
using Spectre.Console;

namespace UI;

public class StackMenu : IMenu
{
    private readonly CardController _card;
    private readonly StackController _stack;
    private string[] _menu = [];

    public StackMenu(CardController card, StackController stack)
    {
        _card = card;
        _stack = stack;
        LoadMenu(IMenu.StackMenuTitle);
    }
    
    public CardController Card { get => _card; init => _card = value; }
    public StackController Stack { get => _stack; init => _stack = value; }
    public void DisplayMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Manage Stack Menu").Color(Color.Green));
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]NOTE: DEFAULT STACK CAN NOT BE REMOVED[/bold]");
            AnsiConsole.WriteLine($"Number of Stacks created currently {Stack.COUNT}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Please select a choice").AddChoices(_menu));

            switch (choice)
            {
                case "Add":
                case "Rename":
                case "Delete":
                case "View": AnsiConsole.WriteLine("Not Implemented Yet"); break;
                case "Exit": break;
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