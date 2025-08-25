using Spectre.Console;


namespace CodeIsland_Game;
public static class Program
{
    public static void Main(string[] args)
    {
        // Display welcome banner
        DisplayWelcomeBanner();

        // Create player
        var player = CreatePlayer();

        // Create game world
        var gameWorld = new GameWorld();

        // Game loop
        RunGameLoop(player, gameWorld);
    }

    private static void RunGameLoop(Player player, GameWorld gameWorld)
    {
        bool gameRunning = true;
        bool victoryAchieved = false;

        while (gameRunning)
        {
            Console.Clear();
            
            // Check if player is facing the Guardian
            if (gameWorld.IsPlayerFacingGuardian())
            {
                RunCombat(player, gameWorld);
                continue;
            }
            
            // Check if player has won the game
            if (gameWorld.IsGuardianDefeated && !victoryAchieved)
            {
                DisplayVictoryScreen(player);
                victoryAchieved = true;
            }
            
            // Display current location
            gameWorld.DisplayCurrentLocation();
            
            // Display player status
            AnsiConsole.WriteLine();
            player.DisplayStatus();
            
            // Show available actions
            AnsiConsole.WriteLine();
            
            // Build the list of available actions based on current location state
            var actions = new List<string> {
                "Look around",
                "Navigate",
                "Check inventory",
                "Quit game"
            };
            
            // Add "Pick up item" if there are items in the location
            var currentLocation = gameWorld.GetCurrentLocation();
            if (currentLocation.AvailableItems.Count > 0)
            {
                actions.Insert(2, "Pick up item");
            }
            
            // Add "Solve puzzle" if there's an unsolved puzzle in this location
            string targetLocation = null;
            foreach (var exit in currentLocation.Exits)
            {
                if (gameWorld.HasPuzzleInLocation(exit.Value))
                {
                    targetLocation = exit.Value;
                    actions.Insert(2, "Solve puzzle");
                    break;
                }
            }
            
            // Add "Open chest" if there's a chest in this location
            if (gameWorld.HasChestInCurrentLocation())
            {
                actions.Insert(2, "Open chest");
            }
            
            // Add "Fight Guardian" if player has encountered the Guardian
            if (gameWorld.IsGuardianEncountered() && !gameWorld.IsGuardianDefeated)
            {
                actions.Insert(2, "Fight Guardian");
            }
            
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]What would you like to do?[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Yellow))
                    .AddChoices(actions));
            
            Console.Clear();
            switch (action)
            {
                case "Look around":
                    gameWorld.DisplayCurrentLocation();
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[blue]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    break;
                
                case "Navigate":
                    gameWorld.ShowNavigationMenu(player);
                    break;
                
                case "Pick up item":
                    PickUpItemMenu(player, gameWorld);
                    break;
                
                case "Check inventory":
                    Console.Clear();
                    player.DisplayStatus();
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[blue]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    break;
                
                case "Quit game":
                    if (ConfirmQuit())
                    {
                        gameRunning = false;
                    }
                    break;
                
                case "Solve puzzle":
                    if (targetLocation != null)
                    {
                        if (gameWorld.SolvePuzzle(targetLocation))
                        {
                            player.Score += 25; // Bonus points for solving a puzzle
                            AnsiConsole.MarkupLine("[green]You gained 25 points for solving the puzzle![/]");
                        }
                    }
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[blue]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    break;
                    
                case "Open chest":
                    gameWorld.OpenChestInCurrentLocation(player);
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[blue]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    break;
                
                case "Fight Guardian":
                    RunCombat(player, gameWorld);
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[blue]Press any key to continue...[/]");
                    Console.ReadKey(true);
                    break;
            }
        }
        
        // End game message
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Game Over")
                .Color(Color.Red));
        AnsiConsole.MarkupLine($"[yellow]Thanks for playing, {player.Name}! Your final score: {player.Score}[/]");
        AnsiConsole.Markup("[blue]Press any key to exit...[/]");
        Console.ReadKey();
    }

    private static bool ConfirmQuit()
    {
        return AnsiConsole.Confirm("[yellow]Are you sure you want to quit the game?[/]");
    }

    private static void PickUpItemMenu(Player player, GameWorld gameWorld)
    {
        var currentLocation = gameWorld.GetCurrentLocation();
        
        if (currentLocation.AvailableItems.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no items to pick up here![/]");
            AnsiConsole.Markup("[blue]Press any key to continue...[/]");
            Console.ReadKey(true);
            return;
        }
        
        var choices = new List<string>(currentLocation.AvailableItems);
        choices.Add("Cancel");
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]What would you like to pick up?[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));
        
        if (choice == "Cancel")
        {
            return;
        }
        
        if (gameWorld.PickUpItem(choice, player))
        {
            AnsiConsole.MarkupLine($"[green]You picked up the [bold]{choice}[/].[/]");
            player.Score += 10; // Reward for picking up items
            AnsiConsole.MarkupLine("[green]You gained 10 points![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to pick up the item.[/]");
        }
        
        AnsiConsole.Markup("[blue]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    private static void DisplayWelcomeBanner()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Code Island")
                .Color(Color.Yellow));

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold cyan]The Lost Treasure of Code Island[/]");
        AnsiConsole.MarkupLine("[italic]A Text Adventure[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.Markup("[yellow]Press any key to begin your adventure...[/]");
        Console.ReadKey(true);
    }

    private static Player CreatePlayer()
    {
        Console.Clear();
        var name = AnsiConsole.Ask<string>("[green]What is your name, adventurer?[/]");

        var player = new Player(name);

        // Create a more visually appealing welcome
        AnsiConsole.Clear();
        
        // Display animated welcome text
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("yellow"))
            .Start("Entering the island...", ctx => {
                Thread.Sleep(1500);
            });
        
        // Character introduction with styled panel
        var welcomePanel = new Panel($"[bold yellow]Welcome to Code Island, [underline]{name}[/]![/]")
        {
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1, 2, 1),
            BorderStyle = new Style(Color.Gold1)
        };
        AnsiConsole.Write(welcomePanel);
        
        AnsiConsole.WriteLine();
        
        // Story introduction with visual styling
        var storyPanel = new Panel(new Markup(
            "[italic]You wake up on a mysterious beach, the waves crashing against the shore.\n" +
            "The sun beats down on your face as you try to remember how you got here...\n\n" +
            "Searching your pockets, you find a tattered note that reads:[/]"))
        {
            Border = BoxBorder.Rounded,
            Expand = true,
            BorderStyle = Style.Parse("blue dim")
        };
        AnsiConsole.Write(storyPanel);
        
        // Make the note stand out with a different panel style
        AnsiConsole.WriteLine();
        var notePanel = new Panel(
            new Markup("[bold yellow]\"Beware the Guardian, solve the Riddles,\nand the Treasure shall be yours.\"[/]"))
        {
            Border = BoxBorder.Heavy,
            Expand = false,
            BorderStyle = Style.Parse("red"),
            Padding = new Padding(2, 1, 2, 1)
        };
        notePanel.Header = new PanelHeader("[dim]Ancient Note[/]");
        AnsiConsole.Write(notePanel);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Your adventure begins now. Good luck![/]");
        AnsiConsole.WriteLine();
        
        var prompt = new Panel("[blue]Press any key to continue...[/]")
        {
            Border = BoxBorder.None
        };
        AnsiConsole.Write(prompt);
        Console.ReadKey(true);

        return player;
    }

    private static void RunCombat(Player player, GameWorld gameWorld)
    {
        var combatSystem = new CombatSystem(player, gameWorld.TempleGuardian);
        
        bool victory = combatSystem.RunCombat();
        
        if (victory)
        {
            gameWorld.AwardTreasure(player);
        }
    }

    private static void DisplayVictoryScreen(Player player)
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("YOU WIN!")
                .Centered()
                .Color(Color.Gold1));
                
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[gold1]THE LOST TREASURE OF CODE ISLAND[/]").RuleStyle("gold1").Centered());
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine($"[bold gold1]Congratulations, {player.Name}![/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]You have defeated the Guardian and claimed the legendary treasure of Code Island![/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold cyan]Final Score: {player.Score}[/]");
        
        // Display a fancy treasure chest
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(@"[gold1]       _.-'_.-'_.-'[/]");
        AnsiConsole.MarkupLine(@"[gold1]    .-'_.-'_.-'_.-'[/]");
        AnsiConsole.MarkupLine(@"[gold1]   (_.-'_.-'_.-'_.-[/]");
        AnsiConsole.MarkupLine(@"[gold1]   |               |[/]");
        AnsiConsole.MarkupLine(@"[gold1]   |  💎   💎   💎  |[/]");
        AnsiConsole.MarkupLine(@"[gold1]   |               |[/]");
        AnsiConsole.MarkupLine(@"[gold1]   |_______________|[/]");
        
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[blue]Press any key to continue your adventure...[/]");
        Console.ReadKey(true);
    }

}
