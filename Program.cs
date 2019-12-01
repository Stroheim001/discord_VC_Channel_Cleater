using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

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
        public Bot()
        {
            discord = new DiscordSocketClient();

            discord.Log += log;

            var yaml = new YamlStream();
            yaml.Load(new StreamReader("data/data.yml", System.Text.Encoding.UTF8));

            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            var token = (YamlScalarNode)mapping.Children[new YamlScalarNode("token")];

            var channel_ = (YamlScalarNode)mapping.Children[new YamlScalarNode("channel")];

            channel = ulong.Parse(channel_.Value);

            var guild_ = (YamlScalarNode)mapping.Children[new YamlScalarNode("guild")];

            guild = ulong.Parse(guild_.Value);

            discord.LoginAsync(TokenType.Bot, token.Value);

            discord.UserVoiceStateUpdated += onUserJoined;
        }

        public async Task onUserJoined(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            SocketGuild socketguild = discord.GetGuild(guild);

            if (state2.VoiceChannel != null && state2.VoiceChannel.Id == channel)
            {
                var VC = await socketguild.CreateVoiceChannelAsync(user.Username + "のチャンネル");
                SocketGuildUser user_ = (SocketGuildUser)user;
                await user_.ModifyAsync(x => x.ChannelId = Optional.Create(VC.Id));
            }

            if (state1.VoiceChannel.Users.Count == 0 && state1.VoiceChannel.Id != channel)
            {
                SocketVoiceChannel old_VC = (SocketVoiceChannel)discord.GetChannel(state1.VoiceChannel.Id);
                await old_VC.DeleteAsync();
            }

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
