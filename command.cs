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
        var channel = (Context.Message.Author as IGuildUser)?.VoiceChannel;
        await channel.ModifyAsync(x => x.UserLimit = limit);
    }

    /// <summary>
    /// 人数制限解除
    /// </summary>
    /// <returns>Task</returns>
    [Command("unlimit")]
    public async Task userUnlimit()
    {
        var channel = (Context.Message.Author as IGuildUser)?.VoiceChannel;
        await channel.ModifyAsync(x => x.UserLimit = 99);
    }

    /// <summary>
    /// 名前の変更
    /// </summary>
    /// <param name="name">変更する名前</param>
    /// <returns>Task</returns>
    [Command("name")]
    public async Task nameChange(string name)
    {
        var channel = (Context.Message.Author as IGuildUser)?.VoiceChannel;
        await channel.ModifyAsync(x => x.Name = name);
    }
    [Command("help")]
    public async Task showHelp()
    {
        EmbedBuilder embed = new EmbedBuilder();
        embed.Title = "Helpコマンド";
        embed.Description = "!limit 人数 : 人数制限\n !unlimit : 人数制限解除\n !name チャンネル名 : チャンネル名変更\n !help : ヘルプコマンド";
        await this.Context.Channel.SendMessageAsync("helpコマンド", false, embed.Build());
    }
}