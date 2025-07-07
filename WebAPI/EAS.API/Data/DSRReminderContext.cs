using EAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EAS.API.Data;

public class DSRReminderContext(DbContextOptions<DSRReminderContext> options)
    : DbContext(options)
{
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Attendance>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();
    }
}
