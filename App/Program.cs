using DataLayer;
using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

var db = new DbSetup();

var table = new CardController();
var card = table.GetCardByID(6);

Console.WriteLine($"There are {table.Count} records in the card table");
bool success = table.DeleteCard(card);
Console.WriteLine($"Was record deleted? {success}");
Console.WriteLine($"There are {table.Count} records in the card table");


AnsiConsole.WriteLine("Begin UI Application Here");
Console.ReadKey();