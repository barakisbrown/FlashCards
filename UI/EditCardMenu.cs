using DataLayer.Controller;
using Spectre.Console;

namespace UI;

public class EditCards(CardController cardController, StackController stackController)
{
    private readonly CardController? _cardController = cardController;
    private readonly StackController _stackController = stackController;

    public CardController? Card { get => _cardController; init => _cardController = value; }

    public StackController? Stack { get => _stackController; init => _stackController = value; }

    public void Begin()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("EDITING FLASHCARD INFORMATION");
            // DISPLAY STACK TO GET FLASHCARD
            // DISPLAY LIST OF CARDS FROM STACK TO EDIT
            // ASK USER WHICH OF THE PARTS TO EDIT
            var stackList = _stackController?.GetStackNames();
            AnsiConsole.WriteLine("Please Select Stack of Cards to find the Card that needs to be modified.");
            AnsiConsole.WriteLine();
            if (stackList != null)
            {
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
            }
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
                    AnsiConsole.WriteLine("Prompt will be modified");
                    var (newName, modified) = ChangePrompt(modify);
                    break;
                case 'a':
                case 'A':
                    AnsiConsole.WriteLine("Answer will be modified");
                    break;
                case 's':
                case 'S': AnsiConsole.WriteLine("Stack will be modified");
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

    private (string,bool) ChangePrompt(string oldName)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"OLD PROMPT = {oldName}");
        var newPrompt = new TextPrompt<string>("Enter new Prompt to be used or ENTER to skip changes.");
        while (true)
        {
            var choice = AnsiConsole.Prompt(newPrompt);
            if (string.IsNullOrEmpty(choice))
            {
                AnsiConsole.WriteLine("Prompt will not be modified. Keeping old Prompt");
                Thread.Sleep(4000);
                return (oldName, false);
            }
            else
            {
                var confirm = AnsiConsole.Confirm("The new Prompt will be ${choice} (Y/N)");
                if (confirm)
                {
                    AnsiConsole.WriteLine($"Prompt changed to {choice}");
                    Thread.Sleep(4000);
                    return (choice, true);
                }
                else
                    continue;
            }
        }
    }

    

}
