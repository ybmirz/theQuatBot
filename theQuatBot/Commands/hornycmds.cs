using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace TheQuatBot.Commands
{
    //just a simple no horny cmds to add to the bot in a group
    [Group("stophorny")]
    [Description("the stop horny command, returns a random no horny pic")]
    [Aliases("sth")]

    public class hornycmds : BaseCommandModule
    {
        // cmnds here can be just executed with ;sth
        [GroupCommand()]
        public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Name of horny person")] DiscordMember member = null)
        {
            var rnd = new Random();
            var next = rnd.Next(0, 6);
            switch (next)
            {
                case 0:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await hornyJail(ctx);
                    return;
                case 1:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await whytfPatrick(ctx);
                    return;
                case 2:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await reportAllah(ctx);
                    return;
                case 3:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await antiHorny(ctx);
                    return;
                case 4:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await hornyLockedOn(ctx);
                    return;
                case 5:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await noHorny(ctx);
                    return;
                case 6:
                    if (member != null)
                    {
                        await ctx.RespondAsync($"{member.Mention}").ConfigureAwait(false);
                    }
                    await squidHorny(ctx);
                    return;
            }
        }

        [Command("go_horny_jail")] // case 0
        public async Task hornyJail(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder {
                Title = "Stop Horny",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/766322327246995496/20201011_142721.jpg?width=1046&height=545"
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("why_tf_patrick")] //case 1
        public async Task whytfPatrick(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Nigga,",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/766322326308126800/20201012_130122.jpg"
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("report_to_Allah")] //case 2
        public async Task reportAllah(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Allah,",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/766322326051487805/1602515388545.jpg?width=383&height=681"
            };
            await ctx.RespondAsync(embed:embed).ConfigureAwait(false);
        }

        [Command("anti_horny_spray")] //case 3
        public async Task antiHorny(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Fuck no,",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/766322327595778058/20201011_135222.jpg?width=479&height=681"
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("horny_mf_lock_on")] //case 4
        public async Task hornyLockedOn(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Roger that,",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/767041331909361674/-7ibdcq.jpg?width=749&height=680"
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("no_horny")] //case 5
        public async Task noHorny(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "No,",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/765360502351069204/20201011_135243.jpg"
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("horny_squidward")] //case 6
        public async Task squidHorny(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = "Interesting,",
                ImageUrl = "https://media.discordapp.net/attachments/764397392296017920/780366381979402240/20201123_105956.jpg?width=786&height=589"
            };
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

    }
}
