using DataLayer;
using DataLayer.Controller;
using DataLayer.Models;
using Spectre.Console;

var db = new DbSetup();

var table = new CardController();

Console.WriteLine($"There are {table.Count} records in the card table");


AnsiConsole.WriteLine("Begin UI Application Here");
Console.ReadKey();