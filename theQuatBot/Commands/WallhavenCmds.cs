using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheQuatBot.Services;

namespace TheQuatBot.Commands
{
    [Group("wallhaven")]
    [Aliases("wall")]
    public class WallhavenCmds : BaseCommandModule
    {
        [Command("search"), Description("Searches using the Wallhaven API wallpaper with a search tag.")]
        public async Task SearchWithPage(CommandContext ctx, int pageNum, [Description("The tag to search with"), RemainingText] string tag)
        {
            try
            {
                List<Page> pages = new List<Page>();
                var wallpapers = getPagedWallpapers(tag, pageNum);
                if (wallpapers.Data.Length > 0)
                {
                    var interactivity = ctx.Client.GetInteractivity();
                    var emojis = new PaginationEmojis()
                    {
                        Left = DiscordEmoji.FromName(ctx.Client, ":arrow_backward:"),
                        Right = DiscordEmoji.FromName(ctx.Client, ":arrow_forward:"),
                        SkipLeft = null,
                        SkipRight = null
                    };
                    int count = 0;
                    foreach (var wallpaper in wallpapers.Data)
                    {
                        count += 1;
                        string desc =
                            $"**Category** = {wallpaper.Category}\n" +
                            $"**Purity** = {wallpaper.Purity}\n" +
                            $"**Resolution** = {wallpaper.Resolution}\n" +
                            $"**Ratio** = {wallpaper.Ratio}\n" +
                            $"*[Original Link]({wallpaper.Url})*\n";
                            var embed = new DiscordEmbedBuilder()
                            .WithAuthor("theQuatBot", null, "https://media.discordapp.net/attachments/764513313907408926/807343859432423434/voldigoad.PNG")
                            .WithTitle($"**Wallpaper #{count}/{wallpapers.Data.Length}**")
                            .WithDescription(desc)
                            .WithImageUrl(wallpaper.Thumbs.Original)
                            .WithColor(DiscordColor.Cyan)
                            .WithFooter($"Requested by {ctx.User.Username}")
                            .WithTimestamp(DateTime.Now);
                        var page = new Page("Press :stop_button: to stop interacting", embed);
                        pages.Add(page);
                    }
                    await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, emojis);
                }
                else
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle("Error 404 : Search Not Found")
                        .WithDescription("There seems to be no more Wallpapers :pepehands: | Try the first page?")
                        .WithTimestamp(DateTime.Now)
                        .WithColor(DiscordColor.Red);
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            { await ctx.RespondAsync($"CmdExecuted with Exception: {e.GetType()} | {e.Message} | {e.StackTrace}"); }
        }
        [Command("search"), Description("Searches using the Wallhaven API wallpaper with a search tag.")]
        public async Task Search(CommandContext ctx,[Description("The tag to search with"), RemainingText]string tag)
        {
                List<Page> pages = new List<Page>();
                var wallpapers = getDefaultWallpapers(tag);
                if (wallpapers.Data.Length > 0)
                {
                    var interactivity = ctx.Client.GetInteractivity();
                    var emojis = new PaginationEmojis()
                    {
                        Left = DiscordEmoji.FromName(ctx.Client, ":arrow_backward:"),
                        Right = DiscordEmoji.FromName(ctx.Client, ":arrow_forward:"),
                        SkipLeft = null,
                        SkipRight = null
                    };
                    int count = 0;
                    foreach (var wallpaper in wallpapers.Data)
                    {
                        count += 1;
                        string desc =
                            $"**Category** = {wallpaper.Category}\n" +
                            $"**Purity** = {wallpaper.Purity}\n" +
                            $"**Resolution** = {wallpaper.Resolution}\n" +
                            $"**Ratio** = {wallpaper.Ratio}\n" +
                            $"*[Original Link]({wallpaper.Url})*\n";
                        var embed = new DiscordEmbedBuilder()
                            .WithAuthor("theQuatBot", null, "https://media.discordapp.net/attachments/764513313907408926/807343859432423434/voldigoad.PNG")
                            .WithTitle($"**Wallpaper #{count}/{wallpapers.Data.Length}**")
                            .WithDescription(desc)
                            .WithImageUrl(wallpaper.Thumbs.Original)
                            .WithColor(DiscordColor.Cyan)
                            .WithFooter($"Requested by {ctx.User.Username}")
                            .WithTimestamp(DateTime.Now);
                        var page = new Page("Press :stop_button: to stop interacting", embed);
                        pages.Add(page);
                    }
                    await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, emojis);
                }
                else
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle("Error 404 : Search Not Found")
                        .WithDescription("There seems to be no more Wallpapers :pepehands:")
                        .WithTimestamp(DateTime.Now)
                        .WithColor(DiscordColor.Red);
                    await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
                }
            
        }

        private WallpaperModel getDefaultWallpapers(string tag)
        {
            WallhavenAPI api = new WallhavenAPI();
            var wallpapers = api.searchTag(tag);
            return wallpapers;
        }
        private WallpaperModel getPagedWallpapers(string tag, int page)
        {
            WallhavenAPI api = new WallhavenAPI();
            var wallpapers = api.searchTagWithPage(tag, page);
            return wallpapers;
        }
    }
}
