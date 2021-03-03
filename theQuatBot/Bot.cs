using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TheQuatBot.Commands;

namespace TheQuatBot
{
    public class Bot
    {
        private EventId BotEventId = new EventId(420,"theQuatBot");
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public Bot(IServiceProvider services)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json")) // config json containing token and prefix is put in the debug folder
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Warning
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            //logging for client during runtime
            Client.GuildAvailable += Client_GuildConnected;
            Client.ClientErrored += client_ClientError;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false, // to avoid confusion
                EnableMentionPrefix = false,
                EnableDefaultHelp = true, // make your own help in abit
                DmHelp = false,
                CaseSensitive = false,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //implementing command logs in runtime
            Commands.CommandExecuted += Command_CommandExecuted;
            Commands.CommandErrored += Command_CommandError;

            Commands.RegisterCommands<RemindCmds>();
            Commands.RegisterCommands<TryCommands>();
            Commands.RegisterCommands<hornycmds>();
            Commands.RegisterCommands<WallhavenCmds>();
            Commands.RegisterCommands<ResinCmnds>();

            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromSeconds(60)
            });
             Client.ConnectAsync();
        }

        //client is ready
        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            Globals.startTime = DateTime.Now;
            client.Logger.LogInformation(BotEventId, $"This bot is ready now bitches Shard: {client.ShardId}");
            return Task.CompletedTask;
        }

        //guild connect log
        private Task Client_GuildConnected(DiscordClient sender, GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, $"theQuatBot is now connected to {e.Guild.Name}({e.Guild.Id})");
            //read the prefixes json and then read (not needed right now, as only one main prefix at hand)
            return Task.CompletedTask;
        }

        //client error exception logging
        private Task client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(BotEventId, $"big oopsie [Type: {e.Exception.GetType()}] [{e.Exception.Message}]");
            return Task.CompletedTask;
        }

        //command executed log
        private Task Command_CommandExecuted(CommandsNextExtension cnext, CommandExecutionEventArgs e)
        {
            cnext.Client.Logger.LogInformation(BotEventId, $"Hoe {e.Context.User.Username} successfully executed {e.Command.QualifiedName}");
            return Task.CompletedTask;
        }

        //command error log
        private async Task Command_CommandError(CommandsNextExtension cnext, CommandErrorEventArgs e)
        {
            cnext.Client.Logger.LogWarning(BotEventId, $"{e.Context.User.Username} tried to do'{e.Context.Message}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"} | {e.Exception.StackTrace}");

            //if it's lack of perms
            if (e.Exception is ChecksFailedException ex)
            { 
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                // wrapping into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Error 69: Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color =  DiscordColor.DarkRed // red
                };
                var msg = await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
                await Task.Delay(4000);
                await msg.DeleteAsync().ConfigureAwait(false);
            }

            //if it's due to wrong argument implementation
            if (e.Exception is System.ArgumentException overload) 
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":x:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Error 53X: Invalid Argument Placement",
                    Description = $"{emoji} Arguments error; Please check q!help <cmd> for proper usage",
                    Color = DiscordColor.DarkRed
                };
                var msg = await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
                await Task.Delay(4000);
                await msg.DeleteAsync().ConfigureAwait(false);
            }

            //if it's due to unknown cmd
            if (e.Exception is CommandNotFoundException ntfound)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":x:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Error 420: Command Not Found",
                    Description = $"{emoji} command does not exist in context [lmao] try q!help",
                    Color = DiscordColor.DarkRed
                };
                var msg = await e.Context.RespondAsync("",embed: embed).ConfigureAwait(false);
                await Task.Delay(4000);
                await msg.DeleteAsync().ConfigureAwait(false);
            }
        }
    }
}
