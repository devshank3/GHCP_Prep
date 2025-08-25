using System;
using Spectre.Console;

namespace CodeIsland_Game;

public class RiddlePuzzle : Puzzle
{
    private string CorrectAnswer { get; }
    
    public RiddlePuzzle(string name, string description, string correctAnswer)
        : base(name, description)
    {
        CorrectAnswer = correctAnswer.ToLower().Trim();
    }
    
    public override bool Present()
    {
        if (IsCompleted)
        {
            AnsiConsole.MarkupLine("[green]You've already solved this riddle.[/]");
            return true;
        }
        
        DisplayPuzzle();
        
        AnsiConsole.WriteLine();
        var answer = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Your answer:[/]")
                .PromptStyle("green"));
        
        if (answer.ToLower().Trim() == CorrectAnswer)
        {
            AnsiConsole.MarkupLine("[green]Correct! The door unlocks with a satisfying click.[/]");
            IsCompleted = true;
            return true;
        }
        else
        {
            AnsiConsole.MarkupLine("[red]That's not right. The door remains locked.[/]");
            return false;
        }
    }
}