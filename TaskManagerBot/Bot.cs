using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
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
        public DiscordClient Client { get; private set; }
        private InteractivityExtension Interactivity { get; set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            //logging for client during runtime
            Client.GuildAvailable += client_Guildavailable;
            Client.ClientErrored += client_ClientError;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false, // to avoid confusion
                EnableMentionPrefix = false,
                EnableDefaultHelp = true, // make your own help in abit
                DmHelp = false,
                CaseSensitive = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //implementing command logs in runtime
            Commands.CommandExecuted += Command_CommandExecuted;
            Commands.CommandErrored += Command_CommandError;

            Commands.RegisterCommands<TryCommands>();
            Commands.RegisterCommands<hornycmds>();

            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromSeconds(30)
            });

            // when a message is created in the guild connected
            Client.MessageCreated += msgCreated;

            await Client.ConnectAsync();


            await Task.Delay(-1);
        }
        //log msgs to console
        private Task msgCreated(MessageCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "TheQuatBot", $"{e.Author.Username} has typed \"{e.Message.Content}\" with ID: {e.Message.Id} in {e.Message.Channel}", DateTime.Now);
            return Task.CompletedTask;
        }

        //client is ready
        private Task OnClientReady(ReadyEventArgs e)
        {
            Globals.startTime = DateTime.Now;
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "TheQuatBot", "Ready for action bitches.", DateTime.Now);
            return Task.CompletedTask;
        }

        //guild connect log
        private Task client_Guildavailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "TheQuatBot", $"This Bitch is connected to {e.Guild.Name}.", DateTime.Now);
            return Task.CompletedTask;
        }

        //client error exception logging
        private Task client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "TheQuatBot", $"big oopsie [Type: {e.Exception.GetType()}] [{e.Exception.Message}]", DateTime.Now);
            return Task.CompletedTask;
        }

        //command executed log
        
        private Task Command_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "TheQuatBot", $"Hoe {e.Context.User.Username} successfully executed {e.Command.Name}", DateTime.Now);
            return Task.CompletedTask;
        }


        //command error log
        private async Task Command_CommandError(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "TheQuatBot", $"{e.Context.User.Username} tried to do'{e.Context.Message}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

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
                await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
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
                await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
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
                await e.Context.RespondAsync("",embed: embed).ConfigureAwait(false);
            }
        }
    }
}
