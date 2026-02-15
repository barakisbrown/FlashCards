using DataLayer.Controller;
using Spectre.Console;

namespace UI
{
    public class Menu(CardController card,StackController stack)
    {
        private readonly StackController _stack = stack;
        private readonly CardController _card = card;

        public void DisplayMenu()
        {
            AnsiConsole.WriteLine($"Number of FlashCards = {card.Count}");
            AnsiConsole.WriteLine($"Number of Stacks = {_stack.COUNT}");
        }
    }
}
