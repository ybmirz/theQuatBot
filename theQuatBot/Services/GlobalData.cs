using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Timers;

namespace TheQuatBot.Services
{
    public class GlobalData
    {
        public static FirestoreDb database;
        public static DateTime startTime;
        public static Dictionary<ulong, Timer> timers = new Dictionary<ulong, Timer>();       
        public static Dictionary<ulong, CommandContext> CapReminderContext = new Dictionary<ulong, CommandContext>();
        public static DiscordChannel birthdayAnnounceChannel;
        public static DiscordClient globalClient;
    }
}
