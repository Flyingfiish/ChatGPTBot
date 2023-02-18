using ChatGPTBot.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTBot.Db
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<MessageResponse> MessageResponses { get; set; }

        public ApplicationContext()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Host=94.198.218.176;Database=default_db;Username=gen_user;Password=lcmt9sc3q5");

            //optionsBuilder.UseSqlServer(@"Server=ELDAR-DESKTOP;Database=ChatGPTBotDB;Trusted_Connection=True;Trust Server Certificate=true");
        }
    }
}
