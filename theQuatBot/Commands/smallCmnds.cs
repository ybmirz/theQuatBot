using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheQuatBot.Services;

namespace TheQuatBot.Commands
{
    //trying to set up globals in c#
    public static class Globals
    {
        public static DateTime startTime;
    }
    

    public class TryCommands : BaseCommandModule
    {
        
        [Command("ping"), Description("Why tf do you want to ping me")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var botMsg = await ctx.Channel.SendMessageAsync(":chart_with_downwards_trend: Pinging ");
            var ping = ctx.Client.Ping;

            //a rnadom msg to be sent
            var rnd = new Random();
            var nxt = rnd.Next(0, 5);

            switch (nxt)
            {
                case 0:
                    var emoji2 = DiscordEmoji.FromName(ctx.Client, ":middle_finger:");
                    await ctx.Channel.SendMessageAsync($"dont fucking ping me again, {ctx.Member.Mention} bitch {emoji2} ({ping} ms)").ConfigureAwait(false);
                    return;
                case 1:
                    var emoji = DiscordEmoji.FromName(ctx.Client, ":ok_hand:");
                    await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}, your mom gay {emoji} ({ping} ms)").ConfigureAwait(false);
                    return;
                case 2:
                    var emoji3 = DiscordEmoji.FromName(ctx.Client, ":yooooslim:");
                    await ctx.Channel.SendMessageAsync($"hmm? you're approaching me? {ctx.Member.Mention} {emoji3} ({ping} ms)").ConfigureAwait(false);
                    return;
                case 3:
                    var emoji4 = DiscordEmoji.FromName(ctx.Client, ":middle_finger:");
                    await ctx.Channel.SendMessageAsync($"dont fucking ping me again, {ctx.Member.Mention} bitch {emoji4} ({ping} ms)").ConfigureAwait(false);
                    return;
                case 4:
                    var emoji5 = DiscordEmoji.FromName(ctx.Client, ":ok_hand:");
                    await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}, your mom gay {emoji5} ({ping} ms)").ConfigureAwait(false);
                    return;
                case 5:
                    var emoji6 = DiscordEmoji.FromName(ctx.Client, ":yooooslim:");
                    await ctx.Channel.SendMessageAsync($"hmm? you're approaching me? {ctx.Member.Mention} {emoji6} ({ping} ms)").ConfigureAwait(false);
                    return;
            }
        }

        [Command("slap"), Description("Simple slap command, returns a slap img")]
        public async Task Slap(CommandContext ctx, [Description("who you'd like to slap ID or discord mention")] DiscordMember user, [Description("[Optional] Message to be sent with the slap (with _ as spaces)")] [RemainingText] string msg = null)
        {
            DiscordMember member = user; //will output executor if code doesnt work
            await ctx.TriggerTypingAsync();

            //adding if to check if the executor inputted proper nickname
            if (member == null)
            {
                //another embed to say sth went wrong in the name
                var emoji = DiscordEmoji.FromName(ctx.Client, ":x:");
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Error 64Y: Invalid Name Argument",
                    Description = $"{emoji} Nickname not found. gay.",
                    Color = DiscordColor.Red
                };
                await ctx.RespondAsync("", embed: embed).ConfigureAwait(false);
            }
            else //proper nick then
            {
                //embed image of slapping
                var embed = new DiscordEmbedBuilder
                {
                    ImageUrl = "https://www.telegraph.co.uk/multimedia/archive/02509/pow_2509607b.jpg"
                };

                if (msg != null)
                {
                    await ctx.Channel.SendMessageAsync($"{msg}{member.Mention}", embed: embed).ConfigureAwait(false);
                }
                else { await ctx.Channel.SendMessageAsync($"{member.Mention}", embed: embed); }
            }
        }

        [Command("summon"), Description("Summons unholy and holy things and people")]
        public async Task whois(CommandContext ctx,[RemainingText] [Description("Current arguments possible: [owner] [fion] [milo] [macy] [lil_pip]")] string args)
        {
            switch (args)
            {
                case "owner":
                    await ctx.Channel.SendMessageAsync($"the owner of this hell hole is {ctx.Guild.Owner}").ConfigureAwait(false);
                    return;
                case "fion":
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "you shall not fucking pass",
                        Description = "why tf do you want to summon her gay ass nigga",
                        Color = DiscordColor.DarkRed
                    };
                    await ctx.Channel.SendMessageAsync("", embed: embed.Build()).ConfigureAwait(false);
                    return;
                case "milo":
                    await ctx.Channel.SendMessageAsync("https://instagram.fdoh2-2.fna.fbcdn.net/v/t51.2885-15/e35/s1080x1080/120201701_630234864525298_5766933638114481231_n.jpg?_nc_ht=instagram.fdoh2-2.fna.fbcdn.net&_nc_cat=110&_nc_ohc=p6xXIDMa6aYAX9bI8kj&_nc_tp=15&oh=2e0a3bd6d9cfbd8cfcf0da14cfe493da&oe=5FBCDBBE").ConfigureAwait(false);
                    return;
                case "macy":
                    await ctx.Channel.SendMessageAsync("https://instagram.fdoh2-2.fna.fbcdn.net/v/t51.2885-15/e35/s1080x1080/79313488_2477708415845742_7451836767828133691_n.jpg?_nc_ht=instagram.fdoh2-2.fna.fbcdn.net&_nc_cat=106&_nc_ohc=ZIRurSoN2F0AX-DNMCC&_nc_tp=15&oh=f147e23f0e916753b0012fe8ceaef922&oe=5FBC1DFB").ConfigureAwait(false);
                    return;
                case "lil pip":
                    await ctx.Channel.SendMessageAsync("https://media.discordapp.net/attachments/764513313907408926/768863292701147216/20201022_184434.jpg?width=511&height=682").ConfigureAwait(false);
                    return;
                case "nigger":
                    await ctx.Channel.SendMessageAsync("https://media.discordapp.net/attachments/695909498430160999/768901166037205063/nah.context_20201020_151908_0.jpg");
                    return;
                case "apip bin laden":
                    await ctx.Channel.SendMessageAsync("https://media.discordapp.net/attachments/695909498430160999/780362908491710464/osama_bin_pip.png?width=1056&height=589");
                    return;
            }
        }

        [Command("annoy"), Description("A cmd to annoy the fuk out of someone by mass pinging, also it returns one less then wanted so like just remember that im not bothered to fix it")]
        public async Task annoy(CommandContext ctx, [Description("The member you wanna annoy [UID of Discord User or @[user]]")] DiscordMember member, [Description("The amount of times you wanna ping em [+1]")] int num = 0, [RemainingText,Description("Optional msg to be added with the ping [RemainingText]")] string msg = null)
        {
            //DiscordMember member = null;
            if (num != 0) //makes sure a number is inputted
            {
                var users = new Dictionary<ulong, DiscordMember>(ctx.Guild.Members);
                await ctx.TriggerTypingAsync();
                //foreach (KeyValuePair<ulong, DiscordMember> member1 in users)
                //{
                //    if (member1.Value.Nickname != null)
                //    {
                //        if (string.Compare(Strings.Left(member1.Value.Nickname.ToLower(), 3), Strings.Left(name.ToLower(), 3)) == 0)
                //        {
                //            member = member1.Value; //change the output you want here
                //        }
                //    }
                //}

                if (member == null)
                {
                    var emoji = DiscordEmoji.FromName(ctx.Client, ":x:");
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Error 64Y: Invalid Name Argument",
                        Description = $"{emoji} Name you inputted does not exist.",
                        Color = DiscordColor.Red
                    };
                    await ctx.RespondAsync("", embed: embed).ConfigureAwait(false);
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append($"{member.Mention}");
                    if (msg != null) { sb.Append($" {msg}"); }
                    await ctx.TriggerTypingAsync();
                    await ctx.Message.DeleteAsync();
                    for (int i = 0; i < num; i++)
                    {
                        await ctx.Channel.SendMessageAsync(sb.ToString()).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":x:");
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Error UR64Y: Invalid Num Argument",
                    Description = $"{emoji} bitch you forgot to input how many times, how did you forget to do sth so simple holy fuck dumbass",
                    Color = DiscordColor.Red
                };
                await ctx.RespondAsync("", embed: embed).ConfigureAwait(false);
            }
        }

        [Command("avatar"), Description("Simple cmd to show the avatar of a user")]
        [Aliases("av")]
        public async Task avatar(CommandContext ctx, DiscordMember member, string pass = null)
        {
            var foot = new DiscordEmbedBuilder.EmbedFooter { };
            if (pass == "n1c3")
            {
                foot = new DiscordEmbedBuilder.EmbedFooter { Text = "oml she's such a queen, bootiful" };
                await ctx.Message.DeleteAsync().ConfigureAwait(false);
            }
            else
            {
                foot = new DiscordEmbedBuilder.EmbedFooter { Text = "ew such a gay ass avatar" };
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = member.Username,
                ImageUrl = member.AvatarUrl,
                Color = DiscordColor.Cyan,
                Footer = foot
            };
            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("pain"), Description("What even is the purpose of life")]
        public async Task pain(CommandContext ctx)
        {
            var pepesad = DiscordEmoji.FromName(ctx.Client, ":PepeHands:");

            await ctx.Channel.SendMessageAsync($"toblerone this toblerone that {pepesad}").ConfigureAwait(false);
            Thread.Sleep(750);
            await ctx.Channel.SendMessageAsync("all i want is not to be 'lone").ConfigureAwait(false);
        }

        [Command("say1"), Description("The bot will repeat whatever you say, and deletes the cmnd msg [Usage: q!say1 why_tf_is_@_gay [userID]]")]
        public async Task say1(CommandContext ctx, [Description("The msg you want the bot to repeat [with _ as your space]")] string str, [Description("[Optional]: to mention a member, use @ in the string")] string name = null)
        {
            //troll say cmd lmao
            await ctx.TriggerTypingAsync();
            await ctx.Channel.SendMessageAsync("bitch sike, nigga ythink ill follow wat you say huh? fuck no :middle_finger:").ConfigureAwait(false);
        }

        [Command("say"), Description("The bot will repeat whatever you say, and deletes the cmnd msg [Usage: q!say [what you want to say]] if there is an @, reply with a username you'd like to mention")]
        [RequireRoles(RoleCheckMode.Any, "bot dictator")]
        public async Task say(CommandContext ctx, [RemainingText] [Description("The msg you want the bot to repeat")] string str)
        {
            if (str != null)
            {
                await ctx.TriggerTypingAsync();
                await ctx.Message.DeleteAsync();
                await ctx.Channel.SendMessageAsync(str).ConfigureAwait(false);
            }
            else
            {
                var botmsg = await ctx.Channel.SendMessageAsync("What do you want me to say goddamnit :face_with_symbols_over_mouth:");
                Thread.Sleep(2000);
                await ctx.Channel.DeleteMessageAsync(botmsg);
            }
        }

        [Command("sayto"), Description("The bot will repeat what you say but to a specified channel")]
        [RequireRoles(RoleCheckMode.Any, "bot dictator")]
        public async Task sayto(CommandContext ctx, [Description("The channel you want to send the msg to")]DiscordChannel chnl, [RemainingText] [Description("Message to be repeated")]string str = null)
        {
            if (str != null)
            {
                await ctx.Message.DeleteAsync();
                await ctx.Client.SendMessageAsync(chnl, str).ConfigureAwait(false);
            }
            else
            {
                var botmsg = await ctx.Channel.SendMessageAsync( "What do you want me to say goddamnit :face_with_symbols_over_mouth:");
                Thread.Sleep(2000);
                await ctx.Channel.DeleteMessageAsync(botmsg);
            }

        }

        [Command("8ball"), Description("Asking the magic 8ball life questions [Not Responsible for any improper decisions taken from this]")]
        [Aliases("8")]
        public async Task ball(CommandContext ctx,[RemainingText] string args = null)
        {
            var sb = new StringBuilder();
            var embed = new DiscordEmbedBuilder();

            var replies = new List<string>();

            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("unclear");

            embed.WithColor(DiscordColor.Green);
            embed.Title = "Welcome to 8-ball (where all life decisions are answered)";

            sb.AppendLine($"{ctx.User.Username},");

            if (args == null)
            {
                sb.AppendLine("Sorry mate, you got no life questions that needs decision making");
            }
            else
            {
                var answer = replies[new Random().Next(replies.Count - 1)];

                sb.AppendLine($"You asked: [**{args}**]....");
                sb.AppendLine();
                sb.AppendLine($"....your answer is [**{answer}**]");
                switch (answer)
                {
                    case "yes":
                        {
                            embed.WithColor(DiscordColor.Cyan);
                            break;
                        }
                    case "no":
                        {
                            embed.WithColor(DiscordColor.Red);
                            break;
                        }
                    case "maybe":
                        {
                            embed.WithColor(DiscordColor.Azure);
                            break;
                        }
                    case "hazzzzy....":
                        {
                            embed.WithColor(DiscordColor.Gray);
                            break;
                        }
                }
            }
            embed.Description = sb.ToString();
            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);

        }

        [Command("rps"), Description("Rock Paper Scissors Simple Game")]
        public async Task rps(CommandContext ctx, [RemainingText,Description("Input your choice here [rock/paper/scissors]")] string args = null)
        {
            if (args == null) {
                var embed = new DiscordEmbedBuilder();
                StringBuilder sb = new StringBuilder();

                embed.Title = "Rock|Paper|Scissors Game";
                embed.Color = DiscordColor.Cyan;
                sb.AppendLine("Welcome to RPS");
                sb.AppendLine("Do q!rps [choice of **rock**,**paper**,**scissors**] to start the rps game");
                sb.AppendLine("Make sure to type in arguments correctly. Have fun !!");

                embed.Description = sb.ToString();
                await ctx.Channel.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
            } else
            {
                //getting the comp's choice
                string[] choices = new string[3] { "rock", "paper", "scissors" };
                if (choices.Contains($"{args.ToLower()}") == false) {
                    var emoji = DiscordEmoji.FromName(ctx.Client, ":x:");

                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Error 53X: Invalid Argument Exception",
                        Description = $"{emoji} Arguments error; Choice input does not exist. [Might be a typo noob]",
                        Color = DiscordColor.DarkRed
                    };
                    await ctx.RespondAsync("", embed: embed).ConfigureAwait(false);
                    return;
                }

                Random rnd = new Random();
                int n = rnd.Next(0, 2);
                //the resulting win
                string[] resultStr = new string[3] { $"**{ctx.User.Username}** has won"/* 0 */, "It's a **tie**" /* 1 */ , "Haha loser, I won" /* 2 */};
                //emded for the winning result
                var resultEmbed = new DiscordEmbedBuilder { Title = "RPS Results" };
                StringBuilder resultSb = new StringBuilder();

                resultSb.AppendLine($"User has chosen **{args}**");
                resultSb.AppendLine();
                resultSb.AppendLine($"Bot has chosen **{choices[n]}**");
                resultSb.AppendLine();
                resultSb.AppendLine(resultStr[rpsFight(args, choices[n])]);

                resultEmbed.Description = resultSb.ToString();


                    switch (rpsFight(args, choices[n]))
                    {
                        case 0:
                            resultEmbed.Color = DiscordColor.Green;
                            break;
                        case 1:
                            resultEmbed.Color = DiscordColor.Cyan;
                            break;
                        case 2:
                            resultEmbed.Color = DiscordColor.Red;
                            break;
                    }

                await ctx.Channel.SendMessageAsync(embed: resultEmbed.Build()).ConfigureAwait(false);
            }

        }

        //rps fucntion returns 0 [Player Won] 1[tie] 2 [Player lost]
        private int rpsFight(string userChoice, string compChoice) //param: usrChoice of type string 
        {
            userChoice = userChoice.ToUpper();
            compChoice = compChoice.ToUpper();

            if (userChoice == "ROCK" && compChoice == "SCISSORS")
            {
                return 0;
            }
            else if (userChoice == "ROCK" && compChoice == "PAPER")
            {
                return 2;
            }
            else if (userChoice == "PAPER" && compChoice == "ROCK")
            {
                return 0;
            }
            else if (userChoice == "PAPER" && compChoice == "SCISSORS")
            {
                return 2;
            }
            else if (userChoice == "SCISSORS" && compChoice == "ROCK")
            {
                return 2;
            }
            else if (userChoice == "SCISSORS" && compChoice == "PAPER")
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }


        [Command("kanye"), Description("Gets a random kanye quote")]
        public async Task kanye(CommandContext ctx)
        {
            string quote = KanyeService.GetQuote();
            var embedThumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://i.pinimg.com/originals/c1/3b/38/c13b38fb73c16f27463f39e4dd6e9d15.jpg", Width = 150, Height =150 };
            var embedFooter = new DiscordEmbedBuilder.EmbedFooter { Text = "Taken from api.kanye.rest" };
            var color = DiscordColor.Brown;

            var embed = new DiscordEmbedBuilder
            {
                Thumbnail = embedThumbnail,
                Footer = embedFooter,
                Color = color,
                Title = "yeezy once said",
                Description = quote
            };

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("uptime"), Description("Returns uptime of the bot")]
        public async Task uptime(CommandContext ctx)
        {
            var timeSpan = DateTime.Now - Globals.startTime;
            var emoji = DiscordEmoji.FromName(ctx.Client,":gem:");
            var embed = new DiscordEmbedBuilder
            {
                Title = "Uptime",
                Color = DiscordColor.DarkGreen,
                Description = $"{timeSpan.Days} Days {timeSpan.Hours} Hours {timeSpan.Minutes} Minutes { timeSpan.Seconds } Seconds"
            }.WithFooter($"Ping {ctx.Client.Ping} ms {emoji}");

            await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
        }

        //set up purge command
        [Command("zahando"), Description("Purge above specified amount of messages")]
        [Aliases("purge")]
        public async Task Purge(CommandContext ctx, [Description("Number of message to delete not including command msg")] int num)
        {
            IEnumerable<DiscordMessage> msgsToDel = await ctx.Channel.GetMessagesAsync(num + 1);
            await ctx.Channel.DeleteMessagesAsync(msgsToDel).ConfigureAwait(false);
        }

        [Command("choose"), Description("Gives you a somewhat random choice between the words you specify (separated by spaces)")]
        public async Task Random(CommandContext ctx, [RemainingText, Description("The word choices arguments separated by spaces.")]string input)
        {
            Random rnd = new Random();
            var inputs = input.Split(" ");
            await ctx.Channel.SendMessageAsync($"I have chosen **{inputs[rnd.Next(0, inputs.Length)]}**.").ConfigureAwait(false);
        }
    }
}






