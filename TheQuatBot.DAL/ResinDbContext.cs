using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using theQuatBot.DAL.Models;

namespace theQuatBot.DAL
{
    public class ResinDbContext : DbContext
    {
        public ResinDbContext(DbContextOptions<ResinDbContext> options) : base(options) { }
        public DbSet<ResinUser> users { get; set; } 
    }
}
