using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace TheQuatBot.Services.API_Services
{
    [FirestoreData]
    public partial class ResinModel
    {
        [FirestoreProperty]
        public ulong DiscordID { get; set; }
        [FirestoreProperty]
        public int ResinAmnt { get; set; }
        [FirestoreProperty]
        public Timestamp LastUpdated { get; set; }
        [FirestoreProperty]
        public ulong guildId { get; set; }
        [FirestoreProperty]
        public bool CapReminder { get; set; } = false;
    }
}
