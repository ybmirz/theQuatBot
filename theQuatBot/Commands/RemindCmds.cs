using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheQuatBot.Services;
using System.Timers;
using System.Threading;

namespace TheQuatBot.Commands
{
    [Group("reminder")] //reminder can be set by anyone in the server along with the reminder cancel can be by anyone [might wanna change later]
    [Description("Reminder commands to set (with overloads) and cancel reminders.")]
    public class RemindCmds : BaseCommandModule
    {
        List<Remindermodel> reminders = new List<Remindermodel>(); 
        [Command("set"), Description("Set Reminder using number of Seconds to wait from start time")]
        public async Task SetSeconds(CommandContext ctx, [Description("Seconds to the Reminder")] int scnds, [RemainingText, Description("The message to go with the reminder")] string msg = null)
        {
            var reminder = new Remindermodel(scnds, msg, ctx);
            reminder.Set();
            if (reminder.IsSet)
            { 
            await ctx.Channel.SendMessageAsync($"Reminder {reminder.GetHashCode()} has been set for task: `{reminder._msg}` -{ctx.User.Username} in **{scnds}** seconds").ConfigureAwait(false); //can change message here
            reminders.Add(reminder); } //adding into the list so it's recorded
            else
                await ctx.Channel.SendMessageAsync("Something went wrong").ConfigureAwait(false);
        }
        [Command("set"), Description("Set Reminder using number of minutes to wait from start time")]
        public async Task SetMinutes(CommandContext ctx, [Description("Minutes to the Reminder in the form [mm:ss]")] string minDef /*minute Default form*/, [RemainingText, Description("The message to be associated with the reminder")]string msg = null)
        {
            int count = 0;
            int initscnds = 0;
            foreach (char i in minDef)
            { 
                if (i == Char.Parse(":"))
                {
                    try
                    {
                        initscnds = int.Parse(minDef.Substring(count + 1)); 
                    }
                    catch (Exception e) {var tempmsg = await ctx.RespondAsync($"Something went wrong; {e.Message}").ConfigureAwait(false); //if it cant parse this means the command argument was inputted wrongly
                        Thread.Sleep(1500);
                        await ctx.Channel.DeleteMessageAsync(tempmsg).ConfigureAwait(false);
                        return; //exit function code block
                    }
                    break; //exits foreach loop cos no need to check next chars
                }
                count++; //down here because zero based substring cut
            }

            int min = int.Parse(minDef.Substring(0, count)); //getting the minute and parsing to int
            int scnds = initscnds + (min * 60); //adding the minutes into seconds

            var reminder = new Remindermodel(scnds, msg, ctx);
            reminder.Set();
            if (reminder.IsSet)
            {
                await ctx.Channel.SendMessageAsync($"Reminder {reminder.GetHashCode()} has been set for task: `{reminder._msg}` -{ctx.User.Username} in **{minDef}** minutes ({scnds} total seconds)").ConfigureAwait(false); //can change message here
                reminders.Add(reminder);
            } //adding into the list so it's recorded
            else
                await ctx.Channel.SendMessageAsync("Something went wrong").ConfigureAwait(false);
        }

        /* Things to add:
         * -manipulate message command so user can change message midway
         */

        // will be auto restarting for the test and see if the timer works with a running program,, but will change to jsut one event occurence once elapsed is triggered
        [Command("cancel"), Description("Cancel set timer that has been enabled")]
        public async Task cancel(CommandContext ctx, [Description("ID of the Reminder")] string ID) //error is being logged to console with InvalidOperationException
        {
            bool reminderFound = false;
            var now = DateTime.Now;
            foreach (Remindermodel reminder in reminders)
            {
                //add condition for ctx user as well to check if you are the one that did the reminder
                if (reminder.GetHashCode() == Int32.Parse(ID))
                {
                    reminderFound = true;
                    reminder.Cancel();
                    var remainingtime = now - reminder.startTime;
                    if (reminder.IsCancelled) //adbundant code but eh
                        await ctx.Channel.SendMessageAsync($"Reminder {reminder.GetHashCode()} by has been cancelled, with **{remainingtime.Minutes}:{remainingtime.Seconds}** Minute remaining, with message `{reminder._msg}` ").ConfigureAwait(false);
                    reminders.Remove(reminder); // shud add user who made the reminder here
                }
                // can have abundant code here where there's an else and stating it's false
            }
            if (!reminderFound)
            {
                await ctx.RespondAsync("Reminder ID not Found").ConfigureAwait(false);
            }

        }

       
    }
}
