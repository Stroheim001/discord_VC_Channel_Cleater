using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace VoiceChannelCreater
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.keep().GetAwaiter().GetResult();
        }
    }
    class Bot
    {
        public static DiscordSocketClient discord;

        private ulong channel;

        private List<ulong> non_erase_channel;

        private ulong guild;

        private char prefix = '!';

        private static CommandService commands;
        private static IServiceProvider services;

        public static Dictionary<ulong, ulong> channel_list;
        public Bot()
        {
            channel_list = new Dictionary<ulong, ulong>();

            discord = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            discord.Log += log;

            var yaml = new YamlStream();
            yaml.Load(new StreamReader("data/data.yml", System.Text.Encoding.UTF8));

            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            var token = (YamlScalarNode)mapping.Children[new YamlScalarNode("token")];

            var channel_ = (YamlScalarNode)mapping.Children[new YamlScalarNode("channel")];

            channel = ulong.Parse(channel_.Value);

            var guild_ = (YamlScalarNode)mapping.Children[new YamlScalarNode("guild")];

            guild = ulong.Parse(guild_.Value);

            non_erase_channel = new List<ulong>();

            non_erase_channel.Add(649456868141563944);
            non_erase_channel.Add(649457149239623690);
            non_erase_channel.Add(649457299886309377);
            non_erase_channel.Add(649462142487101461);
            non_erase_channel.Add(649474051982622720);

            commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            discord.LoginAsync(TokenType.Bot, token.Value);

            discord.UserVoiceStateUpdated += onUserVCJoined;

            discord.MessageReceived += onMessageRecieved;
        }

        public async Task onUserVCJoined(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            SocketGuild socketguild = discord.GetGuild(guild);

            deleteChannel(user, state1, state2);

            if (state2.VoiceChannel != null && state2.VoiceChannel.Id == channel)
            {
                var VC = await socketguild.CreateVoiceChannelAsync(user.Username + "のチャンネル");
                channel_list.Add(discord.CurrentUser.Id, VC.Id);
                SocketGuildUser user_ = (SocketGuildUser)user;
                await user_.ModifyAsync(x => x.ChannelId = Optional.Create(VC.Id));
            }

        }

        async public void deleteChannel(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            if (state1.VoiceChannel != null && state1.VoiceChannel.Users.Count == 0 && state1.VoiceChannel.Id != channel)
            {
                SocketVoiceChannel old_VC = (SocketVoiceChannel)discord.GetChannel(state1.VoiceChannel.Id);

                foreach (var tmp in non_erase_channel)
                {
                    if (tmp == old_VC.Id) return;
                }

                if (channel_list.Remove(old_VC.Id))
                {
                    await old_VC.DeleteAsync();
                }
                else
                {
                    channel_list.Clear();
                    await old_VC.DeleteAsync();
                }
            }

        }

        public async Task onMessageRecieved(SocketMessage message_)
        {
            var message = message_ as SocketUserMessage;
            int argPos = 0;

            if (!(message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }
            var context = new CommandContext(discord, message);

            var result = await commands.ExecuteAsync(context, argPos, services);

        }
        public async Task keep()
        {
            await discord.StartAsync();
            await Task.Delay(-1);
        }
        public Task log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
