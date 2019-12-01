using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

public class Dice : ModuleBase
{

    [Command("limit")]
    public async Task userLimit()
    {
        Console.WriteLine("temp");
    }
    [Command("name")]
    public async Task nameChange(string name)
    {
        Console.WriteLine(name);
    }
}