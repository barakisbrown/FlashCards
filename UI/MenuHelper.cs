
using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

namespace UI;

public static class MenuHelper
{
    private readonly static CardController CardController = new();
    private static StackController _stackController = new();
    
    public static SelectionPrompt<Stack> GetStackNamePrompt(List<Stack>? stackList)
    {
        const string title = $"  STACK";
        var prompt = new SelectionPrompt<Stack>()
            .Title(title)
            .UseConverter(s => $"[bold]{s.Name}[/]")
            .AddChoices<Stack>(stackList);
        return prompt;
    }
    
    public static SelectionPrompt<Card> GetCardNamesPrompt(int stackID)
    {
        var list = CardController.GetAllCardsByStack(stackID);
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