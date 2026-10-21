using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Contexts
{
    public class VagtplanDbContext : DbContext
    {
        private readonly ILogger<VagtplanDbContext> _logger;

        public VagtplanDbContext(DbContextOptions<VagtplanDbContext> options, ILogger<VagtplanDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }

        public DbSet<PlanningPeriod> PlanningPeriods { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<StaffGroup> StaffGroups { get; set; }
        public DbSet<User> People { get; set; }
        public DbSet<Employment> Employments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Employment>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Ward)
                      .WithMany()
                      .HasForeignKey(e => e.WardId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.StaffGroup)
                      .WithMany()
                      .HasForeignKey(e => e.StaffGroupId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}