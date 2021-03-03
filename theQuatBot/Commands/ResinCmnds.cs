using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Threading.Tasks;
using theQuatBot.DAL;
using theQuatBot.DAL.Models;

namespace TheQuatBot.Commands
{
    [Group("resin")]
    [Description("Your own resin counter for genshin right from the bot!")]
    public class ResinCmnds : BaseCommandModule
    {
        private readonly DbContextOptions<ResinDbContext> _options;
        public ResinCmnds(DbContextOptions<ResinDbContext> options)
        {
            _options = options; 
        }

        [GroupCommand]
        [Description("Returns the resin amount you have currently (thru math) and updates the database")]
        public async Task DefaultCmnd(CommandContext ctx)
        {
            using var _context = new ResinDbContext(_options);
            bool found = false;
            var currentUsers = await _context.users.ToListAsync().ConfigureAwait(false);
            int previousResin, newResin;
            foreach (var user in currentUsers) //safer to do this way rather than the List.Find
            {
                if (user.DiscordId == ctx.User.Id)
                {
                    found = true;
                    previousResin = user.ResinAmnt;
                    TimeSpan passedTime = DateTime.Now - user.LastUpdated;
                    newResin = (int)(passedTime.TotalMinutes / 8) + previousResin;
                    if (newResin > 160)
                    {
                        newResin = 160;
                    }

                    user.ResinAmnt = newResin;
                    user.LastUpdated = DateTime.Now;
                    _context.users.Update(user);
                    TimeSpan timeToCap = new TimeSpan(0,minutes: (160 - newResin) * 8,0);
                    await ctx.Channel.SendMessageAsync($"You currently have **{newResin}**/160 resin! Go grind ya khara! Time to cap: {timeToCap.Hours} Hours {timeToCap.Minutes} Minutes {timeToCap.Seconds} Seconds").ConfigureAwait(false);
                }
            }

            if (currentUsers.Count == 0 || found == false)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Error 817CH: Resin User Does Not Exist",
                    Description = $"Have you used q!resin set [ResinAmnt] yet? Make sure to set your resin to update for the counter",
                    Color = DiscordColor.DarkRed // red
                };
                await ctx.RespondAsync(embed).ConfigureAwait(false);
            }
           await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        [Command("set")]
        [Description("Sets your resin amount and updates to the database // Will create new Resin user if you're not already in the database")]
        public async Task UpdateResin(CommandContext ctx, int ResinAmount)
        {
            using var _context = new ResinDbContext(_options);
            bool found = false;
            var currentUsers = await _context.users.ToListAsync().ConfigureAwait(false);
            foreach (var user in currentUsers)
            {
                if (user.DiscordId == ctx.User.Id)
                {
                    found = true;
                    if (ResinAmount > 160)
                    {
                        ResinAmount = 160;
                    }

                    user.ResinAmnt = ResinAmount;
                    user.LastUpdated = DateTime.Now;
                    _context.users.Update(user);
                    await ctx.RespondAsync("Resin amount has been updated!").ConfigureAwait(false);
                }
            }

            if (currentUsers.Count == 0 || found == false)
            {
                await _context.users.AddAsync(new ResinUser { DiscordId = ctx.User.Id, ResinAmnt = ResinAmount, LastUpdated = DateTime.Now }).ConfigureAwait(false);
                var msg = await ctx.Channel.SendMessageAsync("New Resin User added!").ConfigureAwait(false);
            }
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        [Command("removeAcc")]
        public async Task RemoveUser(CommandContext ctx, ulong discordId)
        {
            bool found = false;
            using var _context = new ResinDbContext(_options);
            var users = await _context.users.ToListAsync().ConfigureAwait(false);
            foreach (var user in users)
            {
                if (user.DiscordId == discordId)
                {
                    found = true;
                    _context.users.Remove(user);
                    await ctx.RespondAsync("User found and deleted!").ConfigureAwait(false);
                }
            }

            if (found == false || users.Count == 0)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Error 817CH: A resin user with that ID does not exist",
                    Description = $"Rechecked the user Id inputted or no users exist!",
                    Color = DiscordColor.DarkRed // red
                };
                await ctx.RespondAsync(embed).ConfigureAwait(false);
            }
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        [Command("userList")]
        public async Task UserList(CommandContext ctx)
        {
            using var _context = new ResinDbContext(_options);
            var users = await _context.users.ToListAsync().ConfigureAwait(false);
            if (users.Count == 0)
            {
                await ctx.RespondAsync("There's no resin users in the database!").ConfigureAwait(false);
                return;
            }

            StringBuilder sb = new StringBuilder();
            int count= 0;
            foreach (var user in users)
            {
                count++;
                var discordUser = await ctx.Client.GetUserAsync(user.DiscordId);
                sb.Append($"{count}. Name: {discordUser.Username} [DiscordId: {user.DiscordId}] .ResinAmount:{user.ResinAmnt} ({user.LastUpdated})\n");
            }
            await ctx.Channel.SendMessageAsync(Formatter.BlockCode(sb.ToString(), "css")).ConfigureAwait(false);
        }
    }
}
