using DataLayer.Controller;
using DataLayer.Models;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace UI;

public class StackMenu : IMenu
{
    private readonly List<string> _illegalValues = ["DEFAULT", "QUIT"];
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
            AnsiConsole.Write(new FigletText("Stack Menu").Color(Color.Green));
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[b]NOTE: DEFAULT STACK CAN NOT BE REMOVED[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Number of Stacks created currently {Stack.COUNT}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Please select a choice").AddChoices(_menu));

            switch (choice)
            {
                case "Add":
                    AddNewStack();
                    Thread.Sleep(2000);
                    continue;
                case "Rename":
                case "Delete":
                    AnsiConsole.MarkupLine("[Yellow]Not Implemented Yet[/]");
                    Thread.Sleep(1000);
                    continue;
                case "View":                      
                    ShowStackCardCount();
                    AnsiConsole.Write("Press any key to return to menu.");
                    Console.ReadKey(true);
                    continue;
                case "Exit": break;
            }
            break;
        }
    }
    public void LoadMenu(string menuName)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "\\MENUS\\";         

        _menu = File.ReadAllLines(Path.Combine(path, menuName));
    }

    private void ShowStackCardCount()
    {
        List<CardsPerStackDTO> cardPerStacks = Stack.StackTotalCardView();

        // DISPLAY TABLE
        var table = new Table().RoundedBorder();
        table.AddColumn("Name");
        table.AddColumn("Number of Cards",col => col.RightAligned());

        foreach(var cps in cardPerStacks)
        {
            table.AddRow(cps.Name,cps.NumCards.ToString());
        }

        AnsiConsole.Write(table);
    }
    /// <summary>
    /// AddNewTable will ask the user for a new stak to be created.
    /// Note: Name of the Stacks are UNIQUE and DEFAULT can not be used either since it is used internally.
    /// </summary>
    private void AddNewStack()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Please enter a new Stack Name to be used");
            AnsiConsole.WriteLine("Stack Names need to be Unique and not called DEFAULT/default");
            AnsiConsole.WriteLine("TYPE QUIT/Quit TO EXIT");
            var name = AnsiConsole.Ask<string>("Stack Name => ");
            var contains = _illegalValues.FindIndex(value => value.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (contains == -1)
            {
                AnsiConsole.WriteLine("Press any key to return to stack menu.");
                Console.ReadKey(true);
                break;
            }
            // CHECK FOR EMPTY STRING / DUPLICATE / DEFAULT
            if (!Stack.AddStack(name))
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Name enteted must NOT be empty or already exist..Try again[/].");
                Console.WriteLine("Press any key to try again.");
                Console.ReadKey(true);
                continue;
            }            
            else
            {
                AnsiConsole.WriteLine($"{name} has been added to the stacks.");
                Thread.Sleep(2000);
                break;
            }
        }
        
    }
}