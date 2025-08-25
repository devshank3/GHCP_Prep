using System;
using Spectre.Console;

namespace CodeIsland_Game;

public class Player
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public List<string> Inventory { get; set; }
        public int Score { get; set; }

        public Player(string name, int health = 100)
        {
            Name = name;
            Health = health;
            MaxHealth = health;
            Inventory = new List<string>();
            Score = 0;
        }

        public void AddItem(string item)
        {
            Inventory.Add(item);
            AnsiConsole.MarkupLine($"[green]Added [bold]{item}[/] to inventory[/]");
        }

        public bool RemoveItem(string item)
        {
            bool removed = Inventory.Remove(item);
            if (removed)
            {
                AnsiConsole.MarkupLine($"[yellow]Removed [bold]{item}[/] from inventory[/]");
            }
            return removed;
        }

        public bool HasItem(string item)
        {
            return Inventory.Contains(item);
        }

        public void DisplayStatus()
        {
            var panel = new Panel($"[bold]{Name}[/] | [blue]Score:[/] {Score}")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0)
            };
            AnsiConsole.Write(panel);

            // Health bar
            AnsiConsole.Write(new BarChart()
                .Width(60)
                .AddItem("Health", Health, Health > 50 ? Color.Green : Health > 25 ? Color.Yellow : Color.Red));

            // Inventory
            if (Inventory.Count == 0)
            {
                var emptyPanel = new Panel(new Markup("[italic]Empty[/]"))
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader("[bold]Inventory[/]")
                };
                AnsiConsole.Write(emptyPanel);
            }
            else
            {
                var table = new Table().Border(TableBorder.Rounded);
                table.AddColumn("Items");
                
                foreach (var item in Inventory)
                {
                    table.AddRow($"[yellow]{item}[/]");
                }
                
                var inventoryPanel = new Panel(table)
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader("[bold]Inventory[/]")
                };
                AnsiConsole.Write(inventoryPanel);
            }
        }
    }
