using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TheQuatBot.Services
{
    //bot services that we might need to use
    public class BotServices
    {
        //quickly send embeds without new 
        public static async Task SendEmbedAsync(CommandContext ctx,[Description("Title of Embed")]string Title,[Description ("Description of Embed")] string message, EmbedType type = EmbedType.Default)
        {
            var prefix = string.Empty;
            DiscordColor color;
            switch (type)
            {
                case EmbedType.Good:
                    color = DiscordColor.Green;
                    break;

                case EmbedType.Warning:
                    prefix = ":warning: ";
                    color = DiscordColor.Yellow;
                    break;

                case EmbedType.Missing:
                    prefix = ":mag: ";
                    color = DiscordColor.Wheat;
                    break;

                case EmbedType.Error:
                    prefix = ":no_entry: ";
                    color = DiscordColor.Red;
                    break;

                default:
                    color = new DiscordColor("#00FF7F");
                    break;
            }

            var output = new DiscordEmbedBuilder()
                .WithTitle(Title)
                .WithDescription(prefix + message)
                .WithColor(color);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
        //simple null checking of string input
        public static bool CheckUserInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }
        //string comparison for a "Next" message or a specified keyword
        public static async Task<InteractivityResult<DiscordMessage>> GetUserInteractivity(CommandContext ctx, string keyword, int seconds)
        {
            return await ctx.Client.GetInteractivity()
                .WaitForMessageAsync(
                    m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase),
                    TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        }
        //remove message just as a better syntax
        public static async Task RemoveMessage(DiscordMessage message)
        {
            await message.DeleteAsync().ConfigureAwait(false);
        }

    }
    public enum EmbedType
    {
        Default,
        Good,
        Warning,
        Missing,
        Error
    }
}
