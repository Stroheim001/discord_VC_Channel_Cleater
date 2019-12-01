using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using YamlDotNet.RepresentationModel;

using Microsoft.Extensions.DependencyInjection;

namespace ban_account_bot
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
        private DiscordSocketClient discord;

        private ulong channel;

        private ulong guild;

        private char prefix = '!';

        private static CommandService commands;
        private static IServiceProvider services;
        public Bot()
        {
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

            commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            discord.LoginAsync(TokenType.Bot, token.Value);

            discord.UserVoiceStateUpdated += onUserVCJoined;

            discord.MessageReceived += onMessageRecieved;
        }

        public async Task onUserVCJoined(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            SocketGuild socketguild = discord.GetGuild(guild);

            if (state2.VoiceChannel != null && state2.VoiceChannel.Id == channel)
            {
                var VC = await socketguild.CreateVoiceChannelAsync(user.Username + "のチャンネル");
                SocketGuildUser user_ = (SocketGuildUser)user;
                await user_.ModifyAsync(x => x.ChannelId = Optional.Create(VC.Id));
            }

            if (state1.VoiceChannel != null && state1.VoiceChannel.Users.Count == 0 && state1.VoiceChannel.Id != channel)
            {
                SocketVoiceChannel old_VC = (SocketVoiceChannel)discord.GetChannel(state1.VoiceChannel.Id);
                await old_VC.DeleteAsync();
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
