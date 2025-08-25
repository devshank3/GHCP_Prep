using System;
using Spectre.Console;

namespace CodeIsland_Game;

public class Guardian
{
    public string Name { get; }
    public int Health { get; private set; }
    public int MaxHealth { get; }
    public bool IsDefeated => Health <= 0;
    private readonly Random random = new Random();
    
    public Guardian(string name, int health = 50)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
    }
    
    public int Attack()
    {
        // Random damage between 5-15
        return random.Next(5, 16);
    }
    
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
    }
    
    public void DisplayStatus()
    {
        var panel = new Panel($"[bold red]{Name}[/]")
        {
            Border = BoxBorder.Heavy,
            Padding = new Padding(1, 0),
            BorderStyle = new Style(Color.Red)
        };
        AnsiConsole.Write(panel);

        // Health bar
        AnsiConsole.Write(new BarChart()
            .Width(60)
            .AddItem("Health", Health, Health > 25 ? Color.Red : Color.DarkRed));
    }
}