using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;

using VoiceChannelCreater;

public class CommandList : ModuleBase
{
    /// <summary>
    /// 人数制限
    /// </summary>
    /// <param name="limit">制限する人数</param>
    /// <returns>Task</returns>
    [Command("limit")]
    public async Task userLimit(int limit)
    {
        SocketVoiceChannel channel = (SocketVoiceChannel)Bot.discord.GetChannel(Bot.channel_list[Bot.discord.CurrentUser.Id]);
        await channel.ModifyAsync(x => x.UserLimit = limit);
    }

    /// <summary>
    /// 名前の変更
    /// </summary>
    /// <param name="name">変更する名前</param>
    /// <returns>Task</returns>
    [Command("name")]
    public async Task nameChange(string name)
    {
        SocketGuildChannel channel = (SocketGuildChannel)Bot.discord.GetChannel(Bot.channel_list[Bot.discord.CurrentUser.Id]);
        await channel.ModifyAsync(x => x.Name = name);
    }
}