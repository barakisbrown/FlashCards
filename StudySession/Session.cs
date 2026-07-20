namespace StudySession;

using DataLayer.Controller;
using Spectre.Console;
public class Session(CardController _card,StackController stack)
{
    private const string StudySessionTItle = "StudySession-Menu.txt";
    private string[] _menu;

    private void LoadMenu()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        path += "//MENUS//";

        _menu = File.ReadAllLines(Path.Combine(path, StudySessionTItle));
    }

    public void DisplayMenu()
    {
        LoadMenu();
        while(true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Study Session Menu").Color(Color.Green));

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select an option").AddChoices(_menu));
            switch(choice)
            {
                case "NEW":
                case "HISTORY":
                    AnsiConsole.MarkupLineInterpolated($"[yellow]Not Implemented yet.[/]");
                    Console.ReadKey(true);
                    break;
                case "EXIT":
                    return;
            }
        }
    }

}
