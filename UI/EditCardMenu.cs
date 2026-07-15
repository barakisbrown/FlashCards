using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

namespace UI;

public class EditCards(CardController cardController, StackController stackController)
{
    private readonly CardController? _cardController = cardController;
    private readonly StackController _stackController = stackController;

    public CardController? Card { get => _cardController; init => _cardController = value; }

    public StackController? Stack { get => _stackController; init => _stackController = value; }

    private enum Option { PROMPT = 'p', ANSWER = 'a', STACK = 's', WRITE='w', EXIT = 'x' };
    
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
                EditLoop(choice);                              
            }
            // Will Add More
            AnsiConsole.WriteLine();
            AnsiConsole.Write("Press Any Key to Exit.");
            Console.ReadKey(true);
            break;
        }
        
    }

    private void EditLoop(Stack whichStack)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Select FlashCard from {whichStack.Name} stack to modify");
        var modify = AnsiConsole.Prompt(MenuHelper.GetCardNamesPrompt(whichStack.ID));
        if (modify.ID == 0)
        {
            AnsiConsole.WriteLine("Thank you. No changed will be made. Exiting to Menu.");
            Thread.Sleep(4000);
            return;
        }
        AnsiConsole.WriteLine("Card being Modified");
        AnsiConsole.WriteLine($"Prompt => {modify.Prompt}");
        AnsiConsole.WriteLine($"Answer => {modify.Answer}");
        AnsiConsole.WriteLine($"StackName = {whichStack.Name}");
        while (true)
        {
            // ASK FOR WHICH PART OF THE CARD TO BE MODIFIED 
            // OPTIONS ARE (P)rompt, (A)nswer, (S)tackname, (x)exit(no change made), (W)rite Changes
            var nodifyOption = new TextPrompt<Char>("Which option do you want to modify")
                .AddChoice('p')
                .AddChoice('a')
                .AddChoice('s')
                .AddChoice('w')
                .AddChoice('x');

            var selection = AnsiConsole.Prompt(nodifyOption);
            switch (selection)
            {
                case (char)Option.PROMPT:
                    AnsiConsole.WriteLine("Prompt being changed.");
                    modify = Modify(modify, Option.PROMPT);
                    break;
                case (char)Option.ANSWER:
                    AnsiConsole.WriteLine("Answer being changed.");
                    modify = Modify(modify, Option.ANSWER);
                    break;
                case (char)Option.STACK:
                    AnsiConsole.WriteLine("Stack Name being changed.");
                    modify = Modify(modify, Option.STACK);
                    break;
                case (char)Option.WRITE:
                    var writeConfirm = AnsiConsole.Confirm("Saving Changes Warning..Do you wish to save these changes.");
                    if (writeConfirm)
                    {
                        AnsiConsole.WriteLine("Chnages being saved to disk.");
                        // Write(modify)
                    }else
                        AnsiConsole.WriteLine("Changes have not been saved to disk.");
                    break;
                case (char)Option.EXIT:
                    AnsiConsole.WriteLine("No changes had been made. Exiting to Menu.");
                    return;
            }
        }
    }

    private Card Modify(Card _card, Option which)
    {
        var displayOption = "User chose: ";
        if (which == Option.PROMPT)
            displayOption += "Prompt";
        else if (which == Option.ANSWER)
            displayOption += "Answer";
        else if (which == Option.STACK)
            displayOption += "Stack Name";

        AnsiConsole.WriteLine(displayOption);

        return _card;
    }

    /// <summary>
    /// This actually does the Updat in the backend.
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    private bool Write(Card _card)
    {
        return true;
    }
}
