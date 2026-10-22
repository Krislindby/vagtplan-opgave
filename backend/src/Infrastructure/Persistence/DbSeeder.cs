using Domain.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence
{
    //Lægger lidt eksempeldata ind, så en nyklonet database ikke står helt tom.
    //Kører kun, hvis der ikke findes data i forvejen.
    public static class DbSeeder
    {
        public static void Seed(VagtplanDbContext context)
        {
            if (context.Wards.Any() || context.Users.Any() || context.PlanningPeriods.Any())
                return;

            var periods = new[]
            {
                new PlanningPeriod { Name = "Oktober 2026", StartDate = new DateOnly(2026, 10, 1), EndDate = new DateOnly(2026, 10, 31) },
                new PlanningPeriod { Name = "November 2026", StartDate = new DateOnly(2026, 11, 1), EndDate = new DateOnly(2026, 11, 30) },
                new PlanningPeriod { Name = "December 2026", StartDate = new DateOnly(2026, 12, 1), EndDate = new DateOnly(2026, 12, 31) },
            };

            var skadestue = new Ward { Name = "Skadestue", Color = "#e5484d" };
            var aeldre = new Ward { Name = "Ældremedicin", Color = "#f76808" };
            var neuro = new Ward { Name = "Neurologi", Color = "#0091ff" };

            var overlaege = new StaffGroup { Name = "Overlæge" };
            var afdelingslaege = new StaffGroup { Name = "Afdelingslæge" };
            var reservelaege = new StaffGroup { Name = "Reservelæge" };

            var admin = new User { Name = "Mette Holm", Email = "mette.holm@hospital.dk", IsAdmin = true, IsStaff = false };
            var emil = new User { Name = "Emil Hansen", Email = "emil.hansen@hospital.dk", IsAdmin = false, IsStaff = true };
            var sara = new User { Name = "Sara Lind", Email = "sara.lind@hospital.dk", IsAdmin = false, IsStaff = true };
            var jonas = new User { Name = "Jonas Berg", Email = "jonas.berg@hospital.dk", IsAdmin = false, IsStaff = true };
            var nadia = new User { Name = "Nadia Krogh", Email = "nadia.krogh@hospital.dk", IsAdmin = true, IsStaff = true };

            //Ingen af ansættelserne overlapper for den samme person.
            var employments = new[]
            {
                new Employment { User = emil, Ward = skadestue, StaffGroup = overlaege, HoursPerWeek = 37, StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 12, 31) },
                new Employment { User = sara, Ward = aeldre, StaffGroup = afdelingslaege, HoursPerWeek = 30, StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 11, 30) },
                new Employment { User = jonas, Ward = neuro, StaffGroup = reservelaege, HoursPerWeek = 37, StartDate = new DateOnly(2026, 10, 15), EndDate = new DateOnly(2027, 4, 30) },
                new Employment { User = nadia, Ward = skadestue, StaffGroup = afdelingslaege, HoursPerWeek = 24, StartDate = new DateOnly(2026, 12, 1), EndDate = new DateOnly(2027, 6, 30) },
            };

            context.PlanningPeriods.AddRange(periods);
            context.Wards.AddRange(skadestue, aeldre, neuro);
            context.StaffGroups.AddRange(overlaege, afdelingslaege, reservelaege);
            context.Users.AddRange(admin, emil, sara, jonas, nadia);
            context.Employments.AddRange(employments);

            context.SaveChanges();
        }
    }
}