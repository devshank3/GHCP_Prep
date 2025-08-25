using System;
using System.Collections.Generic;
using Spectre.Console;

namespace CodeIsland_Game;

public class Location
{
    public string Name { get; }
    public string Description { get; set; }
    public List<string> AvailableItems { get; }
    public Dictionary<string, string> Exits { get; }
    public bool HasBeenVisited { get; set; }

    public Puzzle EntryPuzzle { get; private set; }
    public Chest Chest { get; private set; }

    public Location(string name, string description)
    {
        Name = name;
        Description = description;
        AvailableItems = new List<string>();
        Exits = new Dictionary<string, string>();
        HasBeenVisited = false;
    }

    public void SetEntryPuzzle(Puzzle puzzle)
    {
        EntryPuzzle = puzzle;
    }

    public void AddChest(Chest chest)
    {
        Chest = chest;
    }

    public void AddItem(string item)
    {
        AvailableItems.Add(item);
    }

    public bool RemoveItem(string item)
    {
        return AvailableItems.Remove(item);
    }

    public void AddExit(string direction, string locationName)
    {
        Exits[direction.ToLower()] = locationName;
    }
    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }

    public void Display()
    {
        var panel = new Panel(new Markup($"[bold yellow]{Name}[/]"))
        {
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1, 2, 1),
        };
        AnsiConsole.Write(panel);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[italic cyan]{Description}[/]");
        AnsiConsole.WriteLine();

        // Display available items
        if (AvailableItems.Count > 0)
        {
            AnsiConsole.MarkupLine("[bold green]You can see:[/]");
            var itemTable = new Table().Border(TableBorder.Rounded).BorderColor(Color.Green);
            itemTable.AddColumn("Items");

            foreach (var item in AvailableItems)
            {
                itemTable.AddRow($"[yellow]{item}[/]");
            }

            AnsiConsole.Write(itemTable);
            AnsiConsole.WriteLine();
        }

        // Display exits
        if (Exits.Count > 0)
        {
            AnsiConsole.MarkupLine("[bold blue]Exits:[/]");
            var exitTable = new Table().Border(TableBorder.Rounded).BorderColor(Color.Blue);
            exitTable.AddColumn("Direction");
            exitTable.AddColumn("Location");

            foreach (var exit in Exits)
            {
                exitTable.AddRow($"[cyan]{exit.Key.ToUpper()}[/]", $"[white]{exit.Value}[/]");
            }

            AnsiConsole.Write(exitTable);
        }
        
        
        // Display puzzle hint if present and not completed
        if (EntryPuzzle != null && !EntryPuzzle.IsCompleted)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold magenta]There appears to be a puzzle here...[/]");
        }
        
        // Display chest if present
        if (Chest != null && !Chest.IsOpen)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold cyan]You notice a {Chest.Name}.[/]");
        }
    }
}