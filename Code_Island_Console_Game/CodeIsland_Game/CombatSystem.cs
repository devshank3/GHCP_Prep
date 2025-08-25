using System;
using System.Collections.Generic;
using Spectre.Console;

namespace CodeIsland_Game;

public class CombatSystem
{
    private readonly Player player;
    private readonly Guardian guardian;
    private readonly Dictionary<string, int> itemEffects;

    public CombatSystem(Player player, Guardian guardian)
    {
        this.player = player;
        this.guardian = guardian;
        
        // Define item effects (healing amounts or damage bonuses)
        itemEffects = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Coconut", 20 },
            { "Healing Herb", 25 },
            { "Healing Potion", 50 },
            { "Crystal Shard", 15 },
            { "Ancient Scroll", 30 },
            { "Rusty Key", 5 },
            { "Seashell", 10 },
            { "Map Fragment", 5 },
            { "Old Compass", 5 }
        };
    }

    public bool RunCombat()
    {
        AnsiConsole.Clear();
        
        // Intro animation
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("red"))
            .Start("The Guardian awakens...", ctx => {
                Thread.Sleep(2000);
            });
        
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("BOSS FIGHT")
                .Color(Color.Red));
        
        AnsiConsole.MarkupLine("[bold yellow]The Temple Guardian stands before you, blocking the way to the treasure![/]");
        AnsiConsole.WriteLine();
        
        bool playerTurn = true;
        bool combatActive = true;
        bool playerVictory = false;
        
        while (combatActive)
        {
            // Display combat stats
            DisplayCombatStatus();
            
            if (playerTurn)
            {
                // Player's turn
                combatActive = PlayerTurn();
                if (!combatActive)
                {
                    playerVictory = guardian.IsDefeated;
                    break;
                }
            }
            else
            {
                // Guardian's turn
                GuardianTurn();
                if (player.Health <= 0)
                {
                    playerVictory = false;
                    break;
                }
            }
            
            // Switch turns
            playerTurn = !playerTurn;
            
            if (playerTurn)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Press any key for your next turn...[/]");
                Console.ReadKey(true);
                AnsiConsole.Clear();
            }
            else
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("red"))
                    .Start("Guardian is preparing to attack...", ctx => {
                        Thread.Sleep(1500);
                    });
                AnsiConsole.Clear();
            }
        }
        
        if (playerVictory)
        {
            DisplayVictory();
            return true;
        }
        else if (player.Health <= 0)
        {
            DisplayDefeat();
            return false;
        }
        
        // Player ran away
        return false;
    }
    
    private void DisplayCombatStatus()
    {
        // Display Guardian stats
        guardian.DisplayStatus();
        
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]VS[/]");
        AnsiConsole.WriteLine();
        
        // Display Player stats
        player.DisplayStatus();
        AnsiConsole.WriteLine();
    }
    
    private bool PlayerTurn()
    {
        // Player combat menu
        var actions = new List<string> { "Attack", "Use Item", "Run Away" };
        
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]What would you like to do?[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(actions));
        
        switch (action)
        {
            case "Attack":
                int damage = 10; // Base player damage
                guardian.TakeDamage(damage);
                AnsiConsole.MarkupLine($"[green]You attack the Guardian for [bold]{damage}[/] damage![/]");
                
                if (guardian.IsDefeated)
                {
                    return false; // Combat ends with guardian defeated
                }
                break;
                
            case "Use Item":
                if (UseItemMenu())
                {
                    // Item was used successfully
                }
                break;
                
            case "Run Away":
                if (AnsiConsole.Confirm("[yellow]Are you sure you want to flee from battle?[/]"))
                {
                    AnsiConsole.MarkupLine("[yellow]You retreat from the Guardian...[/]");
                    Thread.Sleep(1000);
                    return false; // Combat ends with player running away
                }
                break;
        }
        
        return true; // Combat continues
    }
    
    private void GuardianTurn()
    {
        int damage = guardian.Attack();
        player.Health -= damage;
        
        if (player.Health < 0)
            player.Health = 0;
        
        AnsiConsole.MarkupLine($"[red]The Guardian attacks you for [bold]{damage}[/] damage![/]");
    }
    
    private bool UseItemMenu()
    {
        if (player.Inventory.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]You have no items to use![/]");
            AnsiConsole.Markup("[blue]Press any key to continue...[/]");
            Console.ReadKey(true);
            return false;
        }
        
        var items = new List<string>(player.Inventory);
        items.Add("Cancel");
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Which item would you like to use?[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(items));
        
        if (choice == "Cancel")
        {
            return false;
        }
        
        // Use the selected item
        if (itemEffects.TryGetValue(choice, out int effect))
        {
            player.RemoveItem(choice);
            
            // Most items heal the player
            player.Health += effect;
            if (player.Health > player.MaxHealth)
                player.Health = player.MaxHealth;
                
            AnsiConsole.MarkupLine($"[green]You used [bold]{choice}[/] and recovered [bold]{effect}[/] health![/]");
            return true;
        }
        else
        {
            // Item not in effects dictionary
            player.RemoveItem(choice);
            AnsiConsole.MarkupLine($"[yellow]You used [bold]{choice}[/] but nothing happened...[/]");
            return true;
        }
    }
    
    private void DisplayVictory()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("VICTORY!")
                .Color(Color.Gold1));
        
        AnsiConsole.MarkupLine("[green]The Guardian falls to the ground, defeated![/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[gold1]A bright light reveals an ancient treasure behind where the Guardian stood![/]");
        AnsiConsole.WriteLine();
        
        AnsiConsole.Write(
            new FigletText("💎 TREASURE FOUND! 💎")
                .Centered()
                .Color(Color.Gold1));
            
        AnsiConsole.MarkupLine("[bold gold1]You have gained 100 points![/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[blue]Press any key to continue your adventure...[/]");
        Console.ReadKey(true);
    }
    
    private void DisplayDefeat()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("DEFEATED")
                .Color(Color.Red));
        
        AnsiConsole.MarkupLine("[red]You have been defeated by the Guardian...[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]Perhaps you should gather more items before facing this foe again.[/]");
        AnsiConsole.WriteLine();
        
        AnsiConsole.Markup("[blue]Press any key to continue...[/]");
        Console.ReadKey(true);
        
        // Reduce player's health but don't kill them
        player.Health = Math.Max(10, player.Health);
    }
}