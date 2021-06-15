
using DSharpPlus.Entities;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Timers;
using TheQuatBot.Commands;
using TheQuatBot.Services;
using TheQuatBot.Services.API_Services;

namespace TheQuatBot
{
    public class Startup
    {
        private static Clock _c;
        public void ConfigureServices(IServiceCollection services)
        {
            // connecting to the firestore database
            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Resources/walroos-bot-data-firebase-adminsdk.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            GlobalData.database = FirestoreDb.Create("walroos-bot-data");
            Console.WriteLine("Database connected! Project id:{0}", GlobalData.database.ProjectId);

            CurrentDomain_ProcessStart();
            var serviceProvider = services.BuildServiceProvider();
            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }

        // When the bot is starting up
        static async void CurrentDomain_ProcessStart()
        {
            Query allResinUsers = GlobalData.database.Collection("Resin");
            QuerySnapshot rsinUserSnaps = await allResinUsers.GetSnapshotAsync().ConfigureAwait(false);
            // Creates a new timer for each resinUser saved and adds to dicionary
            foreach (DocumentSnapshot docSnap in rsinUserSnaps)
            {
                var docRef = docSnap.Reference;
                var resinUser = docSnap.ConvertTo<ResinModel>();
                var aTimer = new Timer(480000);
                aTimer.Elapsed += (source, e) => ResinCmnds.UpdateResinEvent(source, e, docRef);
                aTimer.AutoReset = true;
                aTimer.Start();
                Console.WriteLine($"Resin Timer for {resinUser.DiscordID} started!");
                GlobalData.timers.Add(resinUser.DiscordID, aTimer);
            }

            // Start Birthday Clock
            _c = new Clock();
            _c.NewDay += C_NewDay;
        }

        private async static void C_NewDay(object sender, EventArgs e)
        {
            Console.WriteLine("Daily birthday check!");
            Query birthdayRefs = GlobalData.database.Collection("Birthdays");
            QuerySnapshot birthdaySnaps = await birthdayRefs.GetSnapshotAsync();
            foreach (DocumentSnapshot birthdaySnap in birthdaySnaps)
            {
                var birthday = birthdaySnap.ConvertTo<BirthdayModel>();
                if (birthday.BirthDate.ToDateTime().Date == DateTime.UtcNow.Date) // It's someone's birthday today!
                {
                    Console.WriteLine($"It's {birthday.username}'s Birthday today!");
                    var user = await GlobalData.globalClient.GetUserAsync(birthday.DiscordID).ConfigureAwait(false);
                    var msg = " ";
                    if (birthday.birthYear != 0)
                        msg = $" They're now **{DateTime.UtcNow.Year - birthday.birthYear}** years old! \n";
                    await GlobalData.birthdayAnnounceChannel.SendMessageAsync($"Hey @everyone, it's {user.Mention}'s birthday today!\n{msg}Special message: `{birthday.message}` :partying_face: :tada: :confetti_ball: :tada:").ConfigureAwait(false);
                }

                // Checks to see if need to update the year on the birtdate
                var localBirthDate = birthday.BirthDate.ToDateTime().ToLocalTime();
                if (DateTime.Now.Year > localBirthDate.Year)
                {
                    birthday.BirthDate = Timestamp.FromDateTime(new DateTime(DateTime.UtcNow.Year, localBirthDate.Month,localBirthDate.Day,0,0,0,DateTimeKind.Utc));
                    var birthdayRef = birthdaySnap.Reference;
                    await birthdayRef.SetAsync(birthday);                    
                    Console.WriteLine("Birthdate's year has been updated!");
                }
            }
        }

        // When the bot is closing up
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var timer in GlobalData.timers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            _c.Dispose();
            Console.WriteLine("Timer(s) disposed!");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}

