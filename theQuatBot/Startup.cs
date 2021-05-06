
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Timers;
using TheQuatBot.Services;
using TheQuatBot.Services.API_Services;

namespace TheQuatBot
{
    public class Startup
    {
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
                aTimer.Elapsed += (source, e) => UpdateResinEvent(source, e, docRef);
                aTimer.AutoReset = true;
                aTimer.Start();
                Console.WriteLine($"Resin Timer for {resinUser.DiscordID} started!");
                GlobalData.timers.Add(resinUser.DiscordID, aTimer);
            }
        }
        
        // When the bot is closing up
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var timer in GlobalData.timers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            Console.WriteLine("Timer disposed!");
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

            resinUser.LastUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
            Console.WriteLine($"Resin for {resinUser.DiscordID} added! at {DateTime.UtcNow}");
            await docRef.SetAsync(resinUser);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}

