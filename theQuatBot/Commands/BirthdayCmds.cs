using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheQuatBot.Commands
{
    [Group("birthday")]
    [Description("Birthday command for the boys to set and remind them")] //pretty much just reading and writing to a json file
    public class BirthdayCmds : BaseCommandModule
    {
        [Command("set"), Description("Set up a birthday value")]
        public async Task set(CommandContext ctx, [Description("Day and Month of Birthday and Year [dd-month-yyyy]")] string dayMonthyear ) // will set a user's one who did the command to a date for the user's bday
        {
            
        }
    }
}
