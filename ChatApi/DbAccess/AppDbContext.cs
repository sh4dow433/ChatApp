using ChatApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.DbAccess
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<FileRecord> Files { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UsersChats> UsersChats { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UsersChats>().HasKey(uc => new { uc.ChatId, uc.UserId });

            builder.Entity<UsersChats>()
                .HasOne<AppUser>(uc => uc.User)
                .WithMany(u => u.UsersChats)
                .HasForeignKey(uc => uc.UserId);
            builder.Entity<UsersChats>()
                .HasOne<Chat>(uc => uc.Chat)
                .WithMany(c => c.UsersChats)
                .HasForeignKey(uc => uc.ChatId);

            builder.Entity<FriendShip>().HasKey(fs => new { fs.UserId, fs.FriendId });

            builder.Entity<FriendShip>()
                .HasOne<AppUser>(fs => fs.User)
                .WithMany(u => u.FriendShips)
                .HasForeignKey(fs => fs.UserId);
            builder.Entity<FriendShip>()
                .HasOne<Friend>(fs => fs.Friend)
                .WithMany(f => f.FriendShips)
                .HasForeignKey(fs => fs.FriendId);


            base.OnModelCreating(builder);
        }
    }
}
