using DataLayer.Controller;
using DataLayer.Models;
using Microsoft.IdentityModel.Tokens;
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
                    RenameStack();
                    Thread.Sleep(2000);
                    continue;
                case "Delete":
                    DeleteStack();
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
    /// Note: Name of the Stacks are UNIQUE. DEFAULT can not be used for a stack since it is used internally
    /// Note: User enters quit and it will return then back to the exit.
    /// </summary>
    private void AddNewStack()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Please enter a new Stack Name to be used");
            AnsiConsole.WriteLine("Stack Names need to be Unique and can not be called either the following: DEFAULT/default");
            AnsiConsole.WriteLine("OR TYPE QUIT/Quit TO exit back to to the menu.");
            var name = AnsiConsole.Ask<string>("Stack Name => ");
            //  CHECK FOR DUPLICATE ENTRIES SINCE ALL ENTRIES ARE UNIQUE.  Values Default/Quit/Exit can not be used.
            //  Quit takes the user out of here and back to the menu.
            if (name.ToLower().Contains("quit") || name.ToLower().Contains("exit"))
            {
                AnsiConsole.WriteLine("Return back to the Stack Menu.");
                AnsiConsole.WriteLine("Press any key.");
                Console.ReadKey(true);
                break;
            }
            var (added, unique) = _stack.AddStack(name);

            if (unique)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Stack Name {name} exist. Please enter another one.[/]");
                Thread.Sleep(3000);
                continue;
            }
            if (added)
            {
                AnsiConsole.MarkupLineInterpolated($"[bold]Name ${name} has beeen successfully added to the system.[/]");
                Thread.Sleep(3000);
                continue;
            }
        }                
    }

    private void RenameStack() {

        bool changed = false;
        while(!changed)
        {
            
            var stackList = _stack.GetAllStacks();
            stackList.Add(new DataLayer.Models.Stack { Name = "Return to Menu. No Change Made." });


            AnsiConsole.Clear();
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("RENAME A STACK NAME");
            AnsiConsole.WriteLine();

            var prompt = new SelectionPrompt<Stack>()
                .Title("Select Stack Name to Change")
                .UseConverter(s => $"[bold]{s.Name}[/]")
                .AddChoices<Stack>(stackList);

            var choice = AnsiConsole.Prompt(prompt);

            if (choice.ID == 0)
                break;

            while (true)
            {
                var oldName = choice.Name.Trim();
                AnsiConsole.MarkupInterpolated($"Old Stack Name => {oldName}");
                var newName = AnsiConsole.Ask<string>("New Stack Name(DEFAULT NOT ALLOWED)=> ");

                if (newName.Contains("DEFAULT"))
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]ERROR:Name can not be called DEFAULT. Please try again.[/]");
                    AnsiConsole.WriteLine();
                    continue;
                }

                if (string.Equals(oldName,newName,StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]Error:Name can not be the same. Please try again.[/]");
                    AnsiConsole.WriteLine();
                    continue;
                }

                var confirmString = "Old Stack Name => " + oldName + "New Stack Name => " + newName;
                
                AnsiConsole.WriteLine(confirmString);
                bool confirm = AnsiConsole.Confirm("Is this Correct?");
                if (confirm)
                {
                    // Write to the backend
                    if (_stack.EditStack(oldName, newName))
                    {
                        AnsiConsole.MarkupLineInterpolated($"[bold]Succesfully changed {oldName} TO {newName}[/].");
                        AnsiConsole.WriteLine("Redirecting back to previous menu.");
                        changed = true;
                        break;
                    }
                    else
                    {
                        AnsiConsole.MarkupLineInterpolated($"[red]Changed was not completed.[/]");
                        AnsiConsole.WriteLine("Redirecting back to previous menu.");
                        changed = true;
                        break;
                    }
                }
                else
                {
                    break;
                }

            }
        }
    }
    /// <summary>
    /// Deletes a selected stack and all associated flashcards after user confirmation.
    /// </summary>
    /// <remarks>This operation is irreversible. Deleting a stack will permanently remove all flashcards
    /// contained within it. The user is prompted to confirm the deletion before any data is removed.</remarks>
    private void DeleteStack() 
    {
        bool done = false;
        while (done == false)
        {
            var stackList = _stack.GetAllStacks();
            stackList.Add(new DataLayer.Models.Stack { Name = "Return to Menu. No Change Made." });


            AnsiConsole.Clear();
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("DELETE STACK");
            AnsiConsole.MarkupLineInterpolated($"[bold]This deletes all flashcards that are stored in this stack also.[/]");
            AnsiConsole.MarkupLineInterpolated($"[bold]Please be careful. Once done it can not be undone.[/]");
            AnsiConsole.WriteLine();

            var prompt = new SelectionPrompt<Stack>()
                .Title("Select Stack Name to Delete")
                .UseConverter(s => $"[bold]{s.Name}[/]")
                .AddChoices<Stack>(stackList);

            var choice = AnsiConsole.Prompt(prompt);

            if (choice.ID == 0)
            {
                done = true;
                continue;
            }

            // Fetch Number of Records this stack has assigned to it.
            int total = _card.GetNumberCardsInStack(choice.ID);

            AnsiConsole.MarkupLineInterpolated($"[red]Deleting Stack Named = {choice.Name} and #{total} flashcards[/]");
            AnsiConsole.MarkupLineInterpolated($"[red]Please be careful. This is permanent.[/]");

            bool confirm = AnsiConsole.Confirm("Are you sure?");

            if (confirm)
            {
                if (Stack.DeleteStack(choice.Name))
                {
                    AnsiConsole.MarkupLineInterpolated($"[red]Stack {choice.Name} and #{total} flashcards have been deleted from the system.[/]");
                    done = true;
                    break;
                }                
            }
            else
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Aborting. No changes has been made.[/]");
                Thread.Sleep(3000);
                continue;
            }
        }
    }
}