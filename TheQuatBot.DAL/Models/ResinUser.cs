using System;
using System.Collections.Generic;
using System.Text;

namespace theQuatBot.DAL.Models
{
    public class ResinUser : Entity
    {
        public ulong DiscordId { get; set; }
        public int ResinAmnt { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
