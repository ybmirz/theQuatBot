using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TheQuatBot.Services;
using TheQuatBot.Services.API_Services;

namespace TheQuatBot.Commands
{
    [Group("birthday")]
    [Aliases("bb", "bday")]
    [Description("Birthday set up, so the bot can remind us of a birthday coming up")]
    public class BirthdayCmds : BaseCommandModule
    {
        [GroupCommand, Description("Returns back with the stored birthdate, if it does exist. If not, it prompts to register to be reminded of it!")]
        public async Task birthdate(CommandContext ctx)
        {
            DocumentReference docRef = GlobalData.database.Collection("Birthdays").Document(ctx.User.Id.ToString());
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            if (docSnap.Exists)
            {
                var birthday = docSnap.ConvertTo<BirthdayModel>();
                if (birthday.birthYear == 0) // Meaning no birthyear is set
                    await ctx.RespondAsync($"Your birthday is on `{birthday.BirthDate.ToDateTime().ToLocalTime():M}` (UTC {TimeZoneInfo.Local.BaseUtcOffset}). Birthday message is set to `{birthday.message}`.").ConfigureAwait(false);
                else
                {
                    var actualBirthdate = new DateTime(birthday.birthYear, birthday.BirthDate.ToDateTime().ToLocalTime().Month, birthday.BirthDate.ToDateTime().ToLocalTime().Day);
                    await ctx.RespondAsync($"Your birthday is on `{actualBirthdate:D}` (UTC {TimeZoneInfo.Local.BaseUtcOffset}). You'll be turning **{DateTime.UtcNow.Year - birthday.birthYear}** this year! Birthday message is set to `{birthday.message}`.").ConfigureAwait(false);
                }
            }
            else
            { await ctx.RespondAsync($"You seem to not have your birthday set! You can set it using `q!birthday set [mm-dd]/[dd-mmm]`").ConfigureAwait(false); }
        }

        [Command("set"), Description("Registers your birth date with the form [mm-dd] ie: 12-06 or 06-dec. Bot will ping you at the start of the birthday (GMT +3) along with a custom message you can set")]
        public async Task set(CommandContext ctx, string birthDateStr,[RemainingText] string message = "You're closer to death now bud!")
        {
            // Birth date time object with this year as the year            
            DateTime birthDate;
            if (DateTime.TryParse(birthDateStr, out birthDate))
            {
                DocumentReference docRef = GlobalData.database.Collection("Birthdays").Document(ctx.User.Id.ToString());
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();                
                if (docSnap.Exists)
                {
                    var birthday = docSnap.ConvertTo<BirthdayModel>();
                    birthday.nickname = ctx.Member.Nickname;
                    birthday.username = ctx.User.Username + "#" + ctx.User.Discriminator;
                    birthday.BirthDate = Timestamp.FromDateTime(new DateTime(DateTime.UtcNow.Year, birthDate.Month, birthDate.Day,0,0,0, DateTimeKind.Utc));
                    birthday.message = message;
                    birthday.birthYear = 0;
                    if (birthDate.Year != DateTime.UtcNow.Year)
                        birthday.birthYear = birthDate.Year;
                    await docRef.SetAsync(birthday).ConfigureAwait(false);
                }
                else
                {
                    var birthday = new BirthdayModel()
                    {
                        DiscordID = ctx.User.Id,
                        BirthDate = Timestamp.FromDateTime(new DateTime(DateTime.UtcNow.Year, birthDate.Month, birthDate.Day, 0, 0, 0, DateTimeKind.Utc)),
                        nickname = ctx.Member.Nickname,
                        username = ctx.User.Username + "#" + ctx.User.Discriminator,
                        message = message
                    };
                    if (birthDate.Year != DateTime.UtcNow.Year)
                        birthday.birthYear = birthDate.Year;
                    await docRef.CreateAsync(birthday).ConfigureAwait(false);
                }                
                await ctx.RespondAsync($"Your birthdate has been set! `{birthDate.ToLocalTime():M}` (UTC {TimeZoneInfo.Local.BaseUtcOffset}) with message: `{message}`").ConfigureAwait(false);                
            }
            else
            {
                await ctx.RespondAsync($"The input seems to be wrong, please try again. Could not parse: `{birthDateStr}`").ConfigureAwait(false);
                return;
            }
        }

        [Command("list"), Description("Lists out the birthdays that are stored in the database")]
        public async Task list(CommandContext ctx)
        {
            Query query = GlobalData.database.Collection("Birthdays");
            QuerySnapshot querySnaps = await query.GetSnapshotAsync().ConfigureAwait(false);
            if (querySnaps.Count < 1)
            {
                await ctx.RespondAsync($"It seems that there's no birthdays in the database").ConfigureAwait(false);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Birthdays in {ctx.Guild.Name}\n");            
            foreach (var snap in querySnaps)
            {
                var birthday = snap.ConvertTo<BirthdayModel>();
                var nick = "";
                if (!string.IsNullOrEmpty(birthday.nickname))
                    nick = "- Nickname: "+birthday.nickname;
                sb.AppendLine($"• {birthday.BirthDate.ToDateTime():MMM}-{birthday.BirthDate.ToDateTime():dd}: {birthday.DiscordID} {birthday.username} {nick}\n");
            }
            await ctx.Channel.SendMessageAsync(Formatter.BlockCode(sb.ToString())).ConfigureAwait(false);
        }

        [Command("channel"), Description("Sets the channel to send announcements, if not specified, will be the channel the command is called in.")]
        public async Task channelSet(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel == null)
                GlobalData.birthdayAnnounceChannel = ctx.Channel;
            else
                GlobalData.birthdayAnnounceChannel = channel;
            await ctx.RespondAsync($"Birthdays will now be announced in {GlobalData.birthdayAnnounceChannel.Mention}.").ConfigureAwait(false);
        }

        [Command("remove"), Description("Remove your birthday in the database")]
        public async Task removeBirthday(CommandContext ctx)
        {
            DocumentReference docRef = GlobalData.database.Collection("Birthdays").Document(ctx.User.Id.ToString());
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            if (docSnap.Exists)
            {
                await docRef.DeleteAsync().ConfigureAwait(false);
                await ctx.RespondAsync("Your birthday data has been deleted!").ConfigureAwait(false);
            }
            else
                await ctx.RespondAsync($"It seems your birthday was never in the database to begin with!").ConfigureAwait(false);
         }

        [Command("upcoming"), Description("Returns back the upcoming birthdays from the current date")]
        [Aliases("recent")]
        public async Task upcoming(CommandContext ctx)
        {
            try {
                // Basically using an anchor onto a collection of birthday's dates lmao and ordering the list and then checking each position, giving back the closest ones
                List<BirthdayModel> birthdays = new List<BirthdayModel>();
                Query birthdaysGreaterThanNow = GlobalData.database.Collection("Birthdays").WhereGreaterThan("BirthDate", Timestamp.FromDateTime(DateTime.UtcNow));
                Query birthdaysLessThanNow = GlobalData.database.Collection("Birthdays").WhereLessThan("BirthDate", Timestamp.FromDateTime(DateTime.UtcNow));
                var birthdaysGreaterThanNowSnap = await birthdaysGreaterThanNow.GetSnapshotAsync();
                var birthdaysLessThanNowSnap = await birthdaysLessThanNow.GetSnapshotAsync();

                if (birthdaysGreaterThanNowSnap.Count < 1 || birthdaysLessThanNowSnap.Count < 1)
                {
                    await ctx.RespondAsync($"It seems that there's no birthdays in the database").ConfigureAwait(false);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Recent and upcoming birthdays:\n");
                var birthdaysGreaterThan = birthdaysGreaterThanNowSnap.Select(x => x.ConvertTo<BirthdayModel>());
                var birthdaysLessThan = birthdaysLessThanNowSnap.Select(x => x.ConvertTo<BirthdayModel>());
                birthdaysGreaterThan = birthdaysGreaterThan.OrderBy(x => x.BirthDate);
                birthdaysLessThan = birthdaysLessThan.OrderBy(x => x.BirthDate);

                var userGreater = await ctx.Guild.GetMemberAsync(birthdaysGreaterThan.ElementAt(0).DiscordID);
                sb.AppendLine($"**•** `{birthdaysGreaterThan.ElementAt(0).BirthDate.ToDateTime():MMM}-{birthdaysGreaterThan.ElementAt(0).BirthDate.ToDateTime():dd}`: **{userGreater.Nickname}** ({birthdaysGreaterThan.ElementAt(0).username})");

                var userLess = await ctx.Guild.GetMemberAsync(birthdaysLessThan.ElementAt(birthdaysLessThan.Count()).DiscordID);
                sb.AppendLine($"**•** `{birthdaysLessThan.ElementAt(birthdaysLessThan.Count()).BirthDate.ToDateTime():MMM}-{birthdaysLessThan.ElementAt(birthdaysLessThan.Count()).BirthDate.ToDateTime():dd}`: **{userLess.Nickname}** ({birthdaysLessThan.ElementAt(birthdaysLessThan.Count()).username})");

                await ctx.RespondAsync(sb.ToString()).ConfigureAwait(false);
            }
            catch {
                await ctx.RespondAsync("Oh no an error was thrown, fuck this command, dont bother lmao, i gave up if it's still giving errors").ConfigureAwait(false);
            }
        }
    }
}
