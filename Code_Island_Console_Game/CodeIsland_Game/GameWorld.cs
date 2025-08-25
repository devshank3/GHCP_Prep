using System;
using System.Collections.Generic;
using Spectre.Console;

namespace CodeIsland_Game;

public class GameWorld
{
    public Dictionary<string, Location> Locations { get; }
    public string CurrentLocationName { get; private set; }
    
    public Guardian TempleGuardian { get; private set; }
    public bool IsGuardianDefeated => TempleGuardian?.IsDefeated ?? false;

    public bool GuardianEncountered { get; private set; }

    public GameWorld()
    {
        Locations = new Dictionary<string, Location>();
        InitializeWorld();
        CurrentLocationName = "Beach"; // Starting location
    }

    // Call this method when the player encounters the Guardian in your game logic
    public void EncounterGuardian()
    {
        GuardianEncountered = true;
    }

    // Add this method to fix the error
    public bool IsGuardianEncountered()
    {
        return GuardianEncountered;
    }
    private void InitializeWorld()
    {
        // Create Beach location
        var beach = new Location("Beach",
            "You stand on a pristine sandy beach. The waves crash gently on the shore. " +
            "The jungle lies to the north, and you can see the remains of a shipwreck to the south.");
        beach.AddItem("Coconut");
        beach.AddItem("Seashell");
        beach.AddExit("north", "Jungle");
        beach.AddExit("south", "Shipwreck");

        // Create Jungle location
        var jungle = new Location("Jungle",
            "Thick vegetation surrounds you. Strange noises echo from deep within the trees. " +
            "The beach is to the south, and an ancient temple entrance can be seen to the east.");
        jungle.AddItem("Map Fragment");
        jungle.AddItem("Healing Herb");
        jungle.AddExit("south", "Beach");
        jungle.AddExit("east", "Temple Entrance");

        // Create Shipwreck location
        var shipwreck = new Location("Shipwreck",
            "The remnants of a once-mighty vessel lie half-buried in the sand. " +
            "The hull is splintered and weathered by years of exposure to the elements.");
        shipwreck.AddItem("Rusty Key");
        shipwreck.AddItem("Old Compass");
        shipwreck.AddExit("north", "Beach");

        // Create Temple Entrance
        var templeEntrance = new Location("Temple Entrance",
            "Massive stone pillars mark the entrance to an ancient temple. " +
            "The entrance is covered in mysterious symbols and seems to require solving a riddle to enter.");
        templeEntrance.AddExit("west", "Jungle");
        templeEntrance.AddExit("north", "Temple"); // This will be locked initially

        // Create Temple
        var temple = new Location("Temple",
            "The inside of the temple is dimly lit by strange glowing crystals. " +
            "Ancient artifacts line the walls, and at the center stands the imposing Guardian of the Final Puzzle.");
        temple.AddItem("Crystal Shard");
        temple.AddExit("south", "Temple Entrance");

        // Add all locations to the dictionary
        Locations.Add("Beach", beach);
        Locations.Add("Jungle", jungle);
        Locations.Add("Shipwreck", shipwreck);
        Locations.Add("Temple Entrance", templeEntrance);
        Locations.Add("Temple", temple);

        // Add the temple entrance riddle puzzle
        var templeEnt = Locations["Temple Entrance"];
        templeEnt.SetEntryPuzzle(new RiddlePuzzle(
            "Ancient Temple Riddle",
            "The stone door is etched with a mysterious riddle:\n\n[italic yellow]\"What gets wet while drying?\"[/]",
            "towel"));

        // Add a treasure chest in the Temple
        var temp = Locations["Temple"];
        var chest = new Chest("Treasure Chest", new List<string> { "Golden Key", "Ancient Scroll", "Healing Potion" });
        temp.AddChest(chest);

        // Create the Guardian in the Temple
        TempleGuardian = new Guardian("Temple Guardian");
    }
    
    public bool HasPuzzleInLocation(string locationName)
    {
        return Locations.ContainsKey(locationName) && 
            Locations[locationName].EntryPuzzle != null && 
            !Locations[locationName].EntryPuzzle.IsCompleted;
    }

    public bool HasChestInCurrentLocation()
    {
        var location = GetCurrentLocation();
        return location.Chest != null && !location.Chest.IsOpen;
    }

    public bool SolvePuzzle(string locationName)
    {
        if (Locations.ContainsKey(locationName) && Locations[locationName].EntryPuzzle != null)
        {
            return Locations[locationName].EntryPuzzle.Present();
        }
        return false;
    }

    public List<string> OpenChestInCurrentLocation(Player player)
    {
        var location = GetCurrentLocation();
        if (location.Chest != null)
        {
            var items = location.Chest.Open();
            foreach (var item in items)
            {
                player.AddItem(item);
                player.Score += 15; // Bonus points for chest items
            }
            return items;
        }
        return new List<string>();
    }

    public Location GetCurrentLocation()
    {
        return Locations[CurrentLocationName];
    }
    
    public bool MovePlayer(string direction)
{
    direction = direction.ToLower();
    var currentLocation = GetCurrentLocation();
    
    if (currentLocation.Exits.ContainsKey(direction))
    {
        string targetLocationName = currentLocation.Exits[direction];
        
        // Check if the target location has a puzzle that needs to be solved
        if (HasPuzzleInLocation(targetLocationName))
        {
            AnsiConsole.MarkupLine($"[yellow]The way to {targetLocationName} is blocked by a puzzle.[/]");
            AnsiConsole.WriteLine();
            
            if (SolvePuzzle(targetLocationName))
            {
                CurrentLocationName = targetLocationName;
                return true;
            }
            return false;
        }
        
        CurrentLocationName = targetLocationName;
        return true;
    }
    
    return false;
}
    
    public void DisplayCurrentLocation()
    {
        var location = GetCurrentLocation();
        
        // Mark as visited
        if (!location.HasBeenVisited)
        {
            location.HasBeenVisited = true;
        }
        
        location.Display();
    }
    
    public bool PickUpItem(string itemName, Player player)
    {
        var location = GetCurrentLocation();
        var itemIndex = location.AvailableItems.FindIndex(i => i.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        
        if (itemIndex >= 0)
        {
            var item = location.AvailableItems[itemIndex];
            location.AvailableItems.RemoveAt(itemIndex);
            player.AddItem(item);
            return true;
        }
        
        return false;
    }

    public void ShowNavigationMenu(Player player)
    {
        var currentLocation = GetCurrentLocation();
        
        if (currentLocation.Exits.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no exits from this location![/]");
            return;
        }
        
        var choices = new List<string>();
        foreach (var exit in currentLocation.Exits)
        {
            choices.Add($"Go {exit.Key.ToUpper()} to {exit.Value}");
        }
        choices.Add("Stay here");
        
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Where would you like to go?[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Yellow))
                .AddChoices(choices));
        
        if (choice == "Stay here")
        {
            AnsiConsole.MarkupLine("[yellow]You decide to stay where you are.[/]");
            return;
        }
        
        // Extract the direction from the choice
        var direction = choice.Split(' ')[1].ToLower();
        if (MovePlayer(direction))
        {
            Console.Clear();
            DisplayCurrentLocation();
        }
    }

    // Add this method to check if player is facing the guardian
    public bool IsPlayerFacingGuardian()
    {
        return CurrentLocationName == "Temple" && !IsGuardianDefeated;
    }

    // Add this method to handle combat rewards
    public void AwardTreasure(Player player)
    {
        player.Score += 100;
        player.AddItem("Ancient Treasure");
        
        // Update temple description to reflect defeated guardian
        var temple = Locations["Temple"];
        temple.UpdateDescription("The inside of the temple is dimly lit by strange glowing crystals. " +
                               "Ancient artifacts line the walls. The Guardian has been defeated, " +
                               "and a glowing treasure chest sits at the center of the room.");
    }
}