using System;
using Spectre.Console;

namespace CodeIsland_Game;

public abstract class Puzzle
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public bool IsCompleted { get; protected set; }
    
    public Puzzle(string name, string description)
    {
        Name = name;
        Description = description;
        IsCompleted = false;
    }
    
    public abstract bool Present();
    
    public void DisplayPuzzle()
    {
        var panel = new Panel(new Markup(Description))
        {
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1, 2, 1),
            BorderStyle = new Style(Color.Yellow),
            Header = new PanelHeader($"[bold]{Name}[/]")
        };
        AnsiConsole.Write(panel);
    }
}