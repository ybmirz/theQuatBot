using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TheQuatBot.Commands
{
    [Group("reminder")]
    [Description("Reminder commands to set (with overloads) and cancel reminders.")]
    public class RemindCmds : BaseCommandModule
    {
        // setting variables to be passed thru the entire command group
        private Timer _timer = new Timer();
        private DiscordMember _member;
        private CommandContext _ctx;
        private string _msg;
        private DateTime startTime;
        [Command("set"),Description("Set and Start Timer Reminder with allocated Interval timing")]
        public async Task Set(CommandContext ctx,[Description("number of seconds to be reminded with")] int scnds, [RemainingText, Description("The message to go with the reminder")] string str = null)
        {
            if (!(_timer.Enabled))
            {
                _timer.Interval = scnds * 1000; //setting interval timer with the seconds passed thru the command
                _ctx = ctx;
                _member = ctx.Member;
                _msg = str;
                _timer.Elapsed += OnTimedEvent;
                await ctx.RespondAsync($"Reminder for {_member.Mention}, in {scnds} Seconds, with msg \"{_msg}\"").ConfigureAwait(false);
                startTime = DateTime.Now; //setting a datetime datatype when timer start
                _timer.Start();
            }
            else
            {
                await ctx.RespondAsync($"A reminder for user {ctx.User} in channel {ctx.Channel.Mention} already exists.").ConfigureAwait(false);
            }
        }
        // will be auto restarting for the test and see if the timer works with a running program,, but will change to jsut one event occurence once elapsed is triggered
        [Command("cancel"), Description("Cancel set timer that has been enabled")]
        public async Task cancel(CommandContext ctx)
        {
            if (ctx.User == _member)
            {
                _timer.Stop();
                TimeSpan DurationRemaining = DateTime.Now - startTime;
                await ctx.RespondAsync($"Reminder with message \"{_msg}\" for {_ctx.User.Mention} has been cancelled with {DurationRemaining.Hours}H {DurationRemaining.Minutes}M {DurationRemaining.Seconds}S Remaining").ConfigureAwait(false);
            }
            else { await ctx.RespondAsync("You have not set a reminder in this channel").ConfigureAwait(false); }
        }

        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            await _ctx.Channel.SendMessageAsync($"Reminder for: {_ctx.User.Mention} \"{_msg}\" ").ConfigureAwait(false);
            _timer.Stop();
        }

       
    }
}
