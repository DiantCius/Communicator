using Microsoft.EntityFrameworkCore;
using Server.Domain;

namespace Server.Infrastructure
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {       
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ChildPerson> ChildPersons { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatPerson> ChatPersons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChildPerson>()
                .HasKey(cp => new { cp.ChildId, cp.PersonId });

            modelBuilder.Entity<ChildPerson>()
                .HasOne(cp => cp.Child)
                .WithMany(c=> c.ChildPersons)
                .HasForeignKey(cp => cp.ChildId);

            modelBuilder.Entity<ChildPerson>()
                .HasOne(cp => cp.Person)
                .WithMany(p => p.ChildPersons)
                .HasForeignKey(cp => cp.PersonId);

            modelBuilder.Entity<ChatPerson>()
            .HasKey(cp => new { cp.ChatId, cp.PersonId });

            modelBuilder.Entity<ChatPerson>()
                .HasOne(cp => cp.Chat)
                .WithMany(c => c.ChatPersons)
                .HasForeignKey(cp => cp.ChatId);

            modelBuilder.Entity<ChatPerson>()
                .HasOne(cp => cp.Person)
                .WithMany(p => p.ChatPersons)
                .HasForeignKey(cp => cp.PersonId);
        }
    }
}
