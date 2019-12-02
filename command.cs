using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;

using VoiceChannelCreater;

public class Dice : ModuleBase
{

    [Command("limit")]
    public async Task userLimit(int limit)
    {
        SocketVoiceChannel channel = (SocketVoiceChannel)Bot.discord.GetChannel(Bot.channel_list[Bot.discord.CurrentUser.Id]);
        await channel.ModifyAsync(x => x.UserLimit = limit);
    }
    [Command("name")]
    public async Task nameChange(string name)
    {
        SocketGuildChannel channel = (SocketGuildChannel)Bot.discord.GetChannel(Bot.channel_list[Bot.discord.CurrentUser.Id]);
        await channel.ModifyAsync(x => x.Name = name);
    }
}