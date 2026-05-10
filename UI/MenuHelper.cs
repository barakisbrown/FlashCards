
using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

namespace UI;

public class MenuHelper
{
    private static CardController _cardController = new();
    private static StackController _stackController = new();
    
    public static SelectionPrompt<Stack> GetStackNamePrompt(List<Stack> stackList)
    {
        var prompt = new SelectionPrompt<Stack>()
            .Title("Select Stack Name to browse Flash Cards")
            .UseConverter(s => $"[bold]{s.Name}[/]")
            .AddChoices<Stack>(stackList);
        return prompt;
    }
    
    public static SelectionPrompt<Card> GetCardNamesPrompt(int stackID)
    {
        var list = _cardController.GetAllCardsByStack(stackID);
        list.Add(new DataLayer.Models.Card
        {
            Answer = "CANCEL",
            Prompt = "EDIT"
        });

        var title = $"{"  Prompt",-15} {" Answer"}";
        
        var prompt = new SelectionPrompt<Card>()
            .Title(title)
            .UseConverter(s => $"{s.Prompt,-15} {s.Answer}")
            .AddChoices<Card>(list);
        return prompt;
    }

    public static SelectionPrompt<Stack> GetStackListPrompt(List<Stack> stackList,string title)
    {
        var prompt = new SelectionPrompt<Stack>()
            .Title(title)
            .UseConverter(s => $"[bold]{s.Name}[/]")
            .AddChoices<Stack>(stackList);
        return prompt;
    }
}