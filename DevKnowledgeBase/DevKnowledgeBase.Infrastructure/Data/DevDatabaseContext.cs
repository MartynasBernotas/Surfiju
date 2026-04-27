using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Domain.Enums;
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
        public virtual DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Note>()
                .ToTable("Notes");

            modelBuilder.Entity<Note>()
                .HasKey(n => n.Id);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Camp>()
                .HasOne(t => t.Organizer)
                .WithMany(u => u.OrganizedCamps)
                .HasForeignKey(t => t.OrganizerId)
                .IsRequired();

            modelBuilder.Entity<Camp>()
                .HasMany(c => c.Bookings)
                .WithOne(b => b.Camp)
                .HasForeignKey(b => b.CampId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasDefaultValue(BookingStatus.Pending)
                .HasConversion<string>();

            modelBuilder.Entity<Booking>()
                .Property(b => b.PaymentIntentId)
                .IsRequired(false);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CancellationReason)
                .IsRequired(false);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CancelledAt)
                .IsRequired(false);

            modelBuilder.Entity<Camp>().UseXminAsConcurrencyToken();

            modelBuilder.Entity<CampMember>()
                .HasOne(cm => cm.Camp)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.CampId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CampMember>()
                .HasOne(cm => cm.User)
                .WithMany()
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
