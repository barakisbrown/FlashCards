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

    public CardController Card { get => _cardController; init => _cardController = value; }
    public StackController Stack { get => _stackController; init => _stackController = value; }
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
                case "View":
                    ListAllCards();
                    AnsiConsole.WriteLine("Press any key to return to menu.");
                    Console.ReadKey(true);
                    continue;
                case "Add":
                    AddFlashCard();
                    continue;
                case "Edit":
                    EditFlashCards();
                    continue;
                case "Delete":
                    AnsiConsole.MarkupLine("[Yellow]Not Implemented Yet[/]");
                    Thread.Sleep(1000); continue;
                case "Exit": break;
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

    private void ListAllCards()
    {
        var cards = Card.GetAllCards();
        var stacks = Stack.GetAllStacks();
        var stackLookup = stacks.ToDictionary(s => s.ID, s => s.Name);

        var table = new Table().AddColumns("Prompt", "Answer", "Stack");
        foreach (var c in cards)
        {
            var stackName = stackLookup.GetValueOrDefault(c.StackID, "DEFAULT");
            table.AddRow(c.Prompt ?? string.Empty, c.Answer ?? string.Empty, stackName);
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Allows the user to enter a new FlashCard into the system. Stack defaults to DEFAULT.
    /// 
    /// </summary>
    private void AddFlashCard()
    {
        var stackList = _stackController.GetAllStacks();

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("ADDING NEW FLASHCARDS");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("To create a new FlashCard, you will need to enter a prompt followed by the answer.");
            AnsiConsole.WriteLine("You can assign to a Stack here unless you want it to be attached the DEFAULT stack.");
            AnsiConsole.WriteLine("You can change the stacks name of the flashcard later if needed.");

            var cancel = new TextPrompt<string>("Prompt => (HIT RETURN TO EXIT)").AllowEmpty();

            var prompt = AnsiConsole.Prompt(cancel);
            if (string.IsNullOrEmpty(prompt))
            {
                break;
            }
            var answer = AnsiConsole.Ask<string>("Answer => ");
            // DOH - Need to confirm if prompt and answer is correct
            Card newCard = new Card { Prompt = prompt, Answer = answer, StackID = 0 };
            // ASSIGN IT TO THE PROPER STACK EVEN DEFAULT
            var stack = new SelectionPrompt<Stack>()
           .Title("Select Stack Name to Add Card")
           .UseConverter(s => $"[bold]{s.Name}[/]")
           .AddChoices<Stack>(stackList);

            var selStack = AnsiConsole.Prompt(stack);
            newCard.StackID = selStack.ID;
            // CHECK FOR DUPLICATE CARD FROM THE SAME STACK AND ASK THE USER IF HE WANTS 
            // TO STILL KEEP IT OR DISCARD
            if (Card.CardExist(newCard))
            {
                var confirmCard = AnsiConsole.Confirm("This card already exist in this stack. Do you still want to add it anyway?");
                if (!confirmCard)
                    continue;
            }


            var (success, another) = AddCard(newCard);
            if (success && another)
            {
                if (success == true && another == true)
                    continue;
                else
                    break;
            }
            else
            {
                break;
            }
        }
    }

    private void EditFlashCards()
    {
        while(true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("EDITING FLASHCARD INFORMATION");
            // DISPLAY STACK TO GET FLASHCARD
            // DISPLAY LIST OF CARDS FROM STACK TO EDIT
            // ASK USER WHICH OF THE PARTS TO EDIT
            var stackList = _stackController.GetStackNames();
            AnsiConsole.WriteLine("Please Select Stack of Cards to find the Card that needs to be modified.");
            AnsiConsole.WriteLine();
            var choice = AnsiConsole.Prompt(MenuHelper.GetStackNamePrompt(stackList));
            if (choice.ID == 0)
            {
                AnsiConsole.WriteLine("Thank you. No Changed will be made. Exiting to Menu.");
                Thread.Sleep(4000);
                break;
            }

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Select FlashCard from {choice.Name} stack to modify");
            var modify = AnsiConsole.Prompt(MenuHelper.GetCardNamesPrompt(choice.ID));
            if (modify.ID == 0)
            {
                AnsiConsole.WriteLine("Thank you. No changed will be made. Exiting to Menu.");
                Thread.Sleep(4000);
                break;
            }
            AnsiConsole.WriteLine("Card being Modified");
            AnsiConsole.WriteLine($"Prompt => {modify.Prompt}");
            AnsiConsole.WriteLine($"Answer => {modify.Answer}");
            AnsiConsole.WriteLine($"StackName = {choice.Name}");
            // ASK FOR WHICH PART OF THE CARD TO BE MODIFIED 
            // OPTIONS ARE (P)rompt, (A)nswer, (S)tackname, (x)exit(no change made)
            var nodifyOption = new TextPrompt<Char>("Which option do you want to modify")
                .AddChoice('p')
                .AddChoice('a')
                .AddChoice('s')
                .AddChoice('x');
            var selection = AnsiConsole.Prompt(nodifyOption);
            switch (selection)
            {
                case 'p':
                case 'P':
                    AnsiConsole.WriteLine("Prompt will be modiified");
                    break;
                case 'a':
                case 'A':
                    AnsiConsole.WriteLine("Answer will be modiified");
                    break;
                case 's':
                case 'S': AnsiConsole.WriteLine("Stack will be modiified");
                    break;
                case 'x':AnsiConsole.WriteLine("No changes made.  Exiting.");
                    break;
            }
            // Will Add More
            AnsiConsole.WriteLine();
            AnsiConsole.Write("Press Any Key to Exit.");
            Console.ReadKey(true);
            break;
        }
    }

    private string ChangePrompt(string oldName)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"OLD PROMPT = {oldName}");
        throw new NotImplementedException();
    }



    /// <summary>
    /// Attempts to add a new flashcard to the system and prompts the user to add another card if the operation
    /// succeeds.
    /// </summary>
    /// <remarks>If the card is added successfully, the method prompts the user to confirm whether to add
    /// another card. If the addition fails, an error message is displayed and the method returns (<see
    /// langword="false"/>, <see langword="false"/>).</remarks>
    /// <param name="newCard">The flashcard to add to the system. Cannot be null.</param>
    /// <returns>A tuple indicating the result of the operation. The first value is <see langword="true"/> if the card was added
    /// successfully; otherwise, <see langword="false"/>. The second value is <see langword="true"/> if the user chooses
    /// to add another card; otherwise, <see langword="false"/>.</returns>
    private (bool, bool) AddCard(Card newCard)
    {
        var (success,message) = _cardController.AddCard(newCard);
        if (success)
        {
            var name = _stackController.GetStackNameById(newCard.StackID);
            AnsiConsole.WriteLine($"Congrats .. Successfully added a new flashcard into the system to the {name} stack.");
            var another = AnsiConsole.Confirm("Do you want to add an another FlashCard? (Y/N) ");
            if (another)
            {
                return (true, true);
            }
            else
            {
                AnsiConsole.WriteLine("Redirecting to the Card Menu.");
                Thread.Sleep(3000);
                return (true, false);
            }
        }
        else
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error Adding a new FlashCard. Check with Administrator.[/]");
            AnsiConsole.MarkupLineInterpolated($"Exception Used => {message}");
            Thread.Sleep(4000);
            return (false, false);
        }
    }
}