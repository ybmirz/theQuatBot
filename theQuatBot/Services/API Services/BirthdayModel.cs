using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace TheQuatBot.Services.API_Services
{
    [FirestoreData]
    public partial class BirthdayModel
    {
        [FirestoreProperty]
        public ulong DiscordID { get; set; }
        [FirestoreProperty]
        public Timestamp BirthDate { get; set; }
        [FirestoreProperty]
        public string username { get; set; }
        [FirestoreProperty]
        public string nickname { get; set; }
        [FirestoreProperty]
        public string message { get; set; }
        [FirestoreProperty]
        public int birthYear { get; set; } = 0;
    }
}