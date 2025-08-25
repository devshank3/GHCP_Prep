using System;
using System.Collections.Generic;
using Spectre.Console;

namespace CodeIsland_Game;

public class Chest
{
    public string Name { get; }
    public List<string> Contents { get; private set; }
    public CodeLockPuzzle Lock { get; private set; }
    public bool IsOpen { get; private set; }
    
    public Chest(string name, List<string> contents)
    {
        Name = name;
        Contents = contents;
        Lock = new CodeLockPuzzle("Locked Chest", 
            "The chest is secured with a 4-digit combination lock.\nYou need to enter the correct code to open it.");
        IsOpen = false;
    }
    
    public List<string> Open()
    {
        if (IsOpen)
        {
            AnsiConsole.MarkupLine("[yellow]The chest is already open.[/]");
            return new List<string>();
        }
        
        if (Lock.Present())
        {
            IsOpen = true;
            
            var items = new List<string>(Contents);
            Contents.Clear();
            
            AnsiConsole.MarkupLine("[green]You open the chest and find:[/]");
            var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Green);
            table.AddColumn("Items");
            
            foreach (var item in items)
            {
                table.AddRow($"[yellow]{item}[/]");
            }
            
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            
            return items;
        }
        
        return new List<string>();
    }
}