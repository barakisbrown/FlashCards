using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

namespace UI
{
    public class MainMenu : IMenu
    {
        private readonly string _appName = "Flashcards App";
        private readonly string _appNote = "Table DEFAULT can not be deleted since it is a system table.";
        private readonly StackController _stack;
        private readonly CardController _card;

        public MainMenu(CardController card, StackController stack)
        {
            _card = card;
            _stack = stack;
        }

        public CardController Card { get => _card; init => _card = value; }
        public StackController Stack { get => _stack; init  => _stack = value; }
        public void DisplayMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText(_appName).Color(Color.Green));
                AnsiConsole.MarkupLine("[grey]{0}[/]", _appNote);
                AnsiConsole.WriteLine($"Number of FlashCards = {_card.Count}");
                AnsiConsole.WriteLine($"Number of Stacks = {_stack.COUNT}");
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .AddChoices(new[] {
                            "Study Notes Area",
                            "Manage FlashCards",                            
                            "Manage Stacks",
                            "List Cards",
                            "Exit"
                        }));

                switch (choice)
                {
                    case "Study Notes Area":
                        AnsiConsole.MarkupLine("[yellow]Study flow not implemented yet.[/]");
                        break;
                    case "Manage FlashCards":
                        new CardMenu(_card,_stack).DisplayMenu();
                        break;
                    case "Manage Stacks":
                        new StackMenu(_card,_stack).DisplayMenu();
                        break;
                    case "List Cards":
                        ListCards();
                        break;
                    case "Exit":
                        return;
                }

                AnsiConsole.MarkupLine("\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }
        public void LoadMenu(string menuName)
        {
            throw new NotImplementedException();
        }

        private void ListCards()
        {
            var cards = _card.GetAllCards();
            var stacks = _stack.GetAllStacks();
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
}
