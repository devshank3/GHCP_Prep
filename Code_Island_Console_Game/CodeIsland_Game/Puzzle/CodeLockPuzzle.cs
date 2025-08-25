using System;
using Spectre.Console;

namespace CodeIsland_Game;

public class CodeLockPuzzle : Puzzle
{
    private string CorrectCode { get; }
    private int AttemptsLeft { get; set; }
    
    public CodeLockPuzzle(string name, string description)
        : base(name, description)
    {
        // Generate a random 4-digit code
        Random random = new Random();
        CorrectCode = random.Next(1000, 10000).ToString();
        AttemptsLeft = 3;
    }
    
    public override bool Present()
    {
        if (IsCompleted)
        {
            AnsiConsole.MarkupLine("[green]The chest is already unlocked.[/]");
            return true;
        }
        
        DisplayPuzzle();
        
        AnsiConsole.MarkupLine($"[yellow]You have [bold]{AttemptsLeft}[/] attempts remaining.[/]");
        
        // Provide a clue after the first failed attempt
        if (AttemptsLeft < 3)
        {
            AnsiConsole.MarkupLine($"[dim]Hint: The first digit is {CorrectCode[0]}[/]");
        }
        
        AnsiConsole.WriteLine();
        var codeGuess = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Enter the 4-digit code:[/]")
                .PromptStyle("green")
                .Validate(code => 
                {
                    if (code.Length != 4 || !int.TryParse(code, out _))
                        return ValidationResult.Error("[red]Please enter exactly 4 digits[/]");
                    return ValidationResult.Success();
                }));
        
        if (codeGuess == CorrectCode)
        {
            AnsiConsole.MarkupLine("[green]Success! The chest unlocks with a click.[/]");
            IsCompleted = true;
            return true;
        }
        else
        {
            AttemptsLeft--;
            
            // Give feedback on the guess
            string feedback = GetFeedback(codeGuess);
            AnsiConsole.MarkupLine($"[red]Incorrect code. {feedback}[/]");
            
            if (AttemptsLeft <= 0)
            {
                AnsiConsole.MarkupLine("[red]You've used all your attempts. The lock resets.[/]");
                // Reset attempts but don't change the code
                AttemptsLeft = 3;
            }
            
            return false;
        }
    }
    
    private string GetFeedback(string guess)
    {
        int correctDigits = 0;
        int correctPositions = 0;
        
        for (int i = 0; i < 4; i++)
        {
            if (i < guess.Length && guess[i] == CorrectCode[i])
            {
                correctPositions++;
            }
            else if (i < guess.Length && CorrectCode.Contains(guess[i]))
            {
                correctDigits++;
            }
        }
        
        return $"You have {correctPositions} correct digits in the right position and {correctDigits} correct digits in the wrong position.";
    }
}