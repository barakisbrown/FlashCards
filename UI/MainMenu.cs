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
        private string[] _menu = [];

        public MainMenu(CardController card, StackController stack)
        {
            _card = card;
            _stack = stack;
            LoadMenu(IMenu.MainMenuTitle);

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

                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .Title("Select an option:").AddChoices(_menu));

                 switch (choice)
                {
                    case "Study Sessions":
                        AnsiConsole.MarkupLine("[yellow]Study flow not implemented yet.[/]");
                        Thread.Sleep(1000);
                        continue;
                    case "FlashCards":
                        new CardMenu(_card,_stack).DisplayMenu();
                        continue;
                    case "Stacks":
                        new StackMenu(_card,_stack).DisplayMenu();
                        continue;
                    case "List-Cards":
                        ListCards();
                        AnsiConsole.WriteLine("Press any key to return to menu.");
                        Console.ReadKey(true);
                        continue;
                    case "Exit":
                        break;
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
