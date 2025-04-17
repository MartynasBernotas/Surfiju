using DevKnowledgeBase.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevKnowledgeBase.Infrastructure.Data
{
    public class DevDatabaseContext : IdentityDbContext<User>
    {
        public DevDatabaseContext() { }
        public DevDatabaseContext(DbContextOptions<DevDatabaseContext> options) : base(options) { }
        
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Camp> Camps { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<CampMember> CampMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Note>()
                .ToTable("Notes");

            modelBuilder.Entity<Note>()
                .HasKey(n => n.Id);

            modelBuilder.Entity<Camp>()
                .HasOne(t => t.Organizer)
                .WithMany(u => u.OrganizedCamps)
                .HasForeignKey(t => t.OrganizerId)
                .IsRequired();
        }
    }
}
