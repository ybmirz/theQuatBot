using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Google.Cloud.Firestore;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TheQuatBot.Services;
using TheQuatBot.Services.API_Services;

namespace TheQuatBot.Commands
{
    [Group("resin")]
    [Description("Your own resin counter for genshin right from the bot!")]
    public class ResinCmnds : BaseCommandModule
    {
        [GroupCommand]
        [Description("Returns the resin amount you have currently and updates the database")]        
        public async Task DefaultCmnd(CommandContext ctx)
        {
            DocumentReference docref = GlobalData.database.Collection("Resin").Document(ctx.User.Id.ToString());
            DocumentSnapshot docSnap = await docref.GetSnapshotAsync().ConfigureAwait(false);

            if (docSnap.Exists)
            {
                var resinUser = docSnap.ConvertTo<ResinModel>();
                string msg = "Go grind ya khara!";
                if (resinUser.ResinAmnt < 20)
                    msg = "Go cry ya khara!";
                   
                TimeSpan timeToCap = TimeSpan.FromSeconds(((160 - resinUser.ResinAmnt)*8) * 60); // Remaining time for it to reach cap
                DateTime capTime = DateTime.UtcNow + timeToCap;
                await ctx.Channel.SendMessageAsync($"You currently have **{resinUser.ResinAmnt}**/160 resin! " + msg + $"\nTime to cap: `{timeToCap.Hours} Hours {timeToCap.Minutes} Minutes {timeToCap.Seconds} Seconds`" +
                    $"\nWill Cap At `{capTime.ToUniversalTime():f} UTC`" +
                    $"\nRemind when resin caps: `{resinUser.CapReminder}`").ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync("It seems you're not in the database, please do `q!resin set [amount]` first and try again.").ConfigureAwait(false);
            }
        }

        [Command("set")]
        [Description("Sets your resin amount and updates to the database // Will create new Resin user if you're not already in the database")]
        public async Task UpdateResin(CommandContext ctx, int ResinAmount)
        {
            DocumentReference docRef = GlobalData.database.Collection("Resin").Document(ctx.User.Id.ToString());
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync().ConfigureAwait(false);
            int resin = 0;
            ResinModel resinModel;

            // Removes/Restarts an already existing timer
            if (GlobalData.timers.ContainsKey(ctx.User.Id))
            {
                GlobalData.timers[ctx.User.Id].Stop();
                GlobalData.timers[ctx.User.Id].Dispose();
                GlobalData.timers.Remove(ctx.User.Id);
            }

            if (ResinAmount > 160)
                resin = 160;
            else if (ResinAmount < 0)
                resin = 0;
            else
                resin = ResinAmount;           

                if (docSnap.Exists) // Snap exists meaning that we just need to count
                {
                    var resinUser = docSnap.ConvertTo<ResinModel>();
                    resinUser.LastUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
                    resinUser.ResinAmnt = resin;

                    resinModel = resinUser;
                    resin = resinUser.ResinAmnt;
                    await docRef.SetAsync(resinUser).ConfigureAwait(false);
                }
                else
                {
                    var resinUser = new ResinModel()
                    {
                        DiscordID = ctx.User.Id,
                        guildId = ctx.Guild.Id,
                        ResinAmnt = resin,
                        LastUpdated = Timestamp.FromDateTime(DateTime.UtcNow)
                    };
                    resinModel = resinUser;
                    await docRef.CreateAsync(resinUser);
                    resin = resinUser.ResinAmnt;
                }

            // Creates new timer to keep adding resin
            var aTimer = new Timer(480000);
            aTimer.Elapsed += (source, e) => UpdateResinEvent(source, e, docRef);
            aTimer.AutoReset = true;
            aTimer.Start();
            Console.WriteLine($"Resin Timer for {resinModel.DiscordID} re-started!");
            GlobalData.timers.Add(resinModel.DiscordID, aTimer); // Adds to dicionary with uID so no duplicates 

            await ctx.RespondAsync($"Resin amount has been updated! **{resin}** resin.").ConfigureAwait(false);
        }

        [Command("userList"), Description("Returns back the resin users, their current resin amount, and last updated, from users in the server.")]
        public async Task UserList(CommandContext ctx)
        {
            Query allResinUsers = GlobalData.database.Collection("Resin").WhereEqualTo("guildId", ctx.Guild.Id);
            QuerySnapshot rsinUserSnaps = await allResinUsers.GetSnapshotAsync().ConfigureAwait(false);
            if (rsinUserSnaps.Count < 1)
            {
                await ctx.RespondAsync("There's no resin users in the database!").ConfigureAwait(false);
                return;
            }
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var resinSnap in rsinUserSnaps)
            {
                var resinUser = resinSnap.ConvertTo<ResinModel>();
                count++;
                var discordUser = await ctx.Client.GetUserAsync(resinUser.DiscordID);
                sb.AppendLine($"{count}) Name: {discordUser.Username}#{discordUser.Discriminator}\n   [DiscordId: {resinUser.DiscordID}]\n   ResinAmount:{resinUser.ResinAmnt} ({resinUser.LastUpdated.ToDateTime():f})\n   " +
                    $"Cap Reminder: {resinUser.CapReminder}\n");
            }
            await ctx.Channel.SendMessageAsync(Formatter.BlockCode(sb.ToString(), "ml")).ConfigureAwait(false);
        }

        [Command("remind"), Description("Sets whether the user wants to be reminded when their resin is capped. Call to enable or disable")]
        public async Task remind(CommandContext ctx)
        {
            DocumentReference docRef = GlobalData.database.Collection("Resin").Document(ctx.User.Id.ToString());
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync().ConfigureAwait(false);

            if (docSnap.Exists)
            {
                var resinUser = docSnap.ConvertTo<ResinModel>();
                resinUser.CapReminder = !resinUser.CapReminder;
                await docRef.SetAsync(resinUser).ConfigureAwait(false);
                if (resinUser.CapReminder)
                {
                    GlobalData.CapReminderContext.Add(ctx.User.Id, ctx);
                    await ctx.RespondAsync($"You will now **be reminded** when your resin caps.").ConfigureAwait(false);
                }
                else
                {
                    GlobalData.CapReminderContext.Remove(ctx.User.Id);
                    await ctx.RespondAsync($"You will now **not be reminded** when your resin caps.").ConfigureAwait(false);
                }
            }
            else
            { await ctx.RespondAsync("It seems you're not in the database, please do `q!resin set [amount]` first and try again.").ConfigureAwait(false); }
        }

        private static async Task NotifyUser(ulong userId)
        {
            var ctx = GlobalData.CapReminderContext[userId];
            await ctx.RespondAsync($"{ctx.User.Mention}, your resin has capped! Go use it before it overflows!").ConfigureAwait(false);
        }

        private static async void UpdateResinEvent(object source, ElapsedEventArgs e, DocumentReference docRef)
        {
            var snap = await docRef.GetSnapshotAsync().ConfigureAwait(false);
            var resinUser = snap.ConvertTo<ResinModel>();

            resinUser.ResinAmnt++;
            if (resinUser.ResinAmnt > 160)
                resinUser.ResinAmnt = 160;
            if (resinUser.ResinAmnt < 0)
                resinUser.ResinAmnt = 0;

            // Notifies the user when their resin caps, and if they choose to be reminded or not.
            if (resinUser.ResinAmnt == 160)
            {
                if (resinUser.CapReminder)
                {
                    await NotifyUser(resinUser.DiscordID);
                }
            }

            resinUser.LastUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
            Console.WriteLine($"Resin for {resinUser.DiscordID} added! at {DateTime.UtcNow}");
            await docRef.SetAsync(resinUser);
        }
    }
}
