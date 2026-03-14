using DataLayer.Controller;
using DataLayer.Models;
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
            AnsiConsole.Write(new FigletText("Flash Card Menu").Color(Color.Green));
            AnsiConsole.WriteLine($"Number of Cards created currently {Card.Count}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select an option").AddChoices(_menu));

            switch (choice)
            {
                case "View" :
                    ListAllCards();
                    AnsiConsole.WriteLine("Press any key to return to menu.");
                    Console.ReadKey(true);
                    continue;
                case "Add"  :
                case "Edit" :
                case "Delete" : 
                    AnsiConsole.MarkupLine("[Yellow]Not Implemented Yet[/]");
                    Thread.Sleep(1000); continue;
                case "Exit" : break;
            }
            break;
        }
    }
    public void LoadMenu(string menuName)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        path += "//MENUS//";

        _menu = File.ReadAllLines(Path.Combine(path, menuName));
    }

    public void ListAllCards()
    {
        var cards = Card.GetAllCards();
        var stacks = Stack.GetAllStacks();
        var stackLookup = stacks.ToDictionary(s => s.ID, s => s.Name);

        var table = new Table().AddColumns("Prompt", "Answer", "Stack");
        foreach (var c in cards)
        {
            var stackName = stackLookup.ContainsKey(c.StackID) ? stackLookup[c.StackID] : "DEFAULT";
            table.AddRow(c.Prompt ?? string.Empty, c.Answer ?? string.Empty, stackName);
        }

        AnsiConsole.Write(table);
    }
}