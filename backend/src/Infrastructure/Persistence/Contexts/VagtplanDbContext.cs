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
        public DbSet<Person> People { get; set; }
        public DbSet<Employment> Employments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Employment>(entity =>
            {
                entity.HasOne(e => e.Person)
                      .WithMany()
                      .HasForeignKey(e => e.PersonId)
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