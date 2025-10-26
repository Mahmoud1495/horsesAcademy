using HorsesPOC.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HorsesPOC.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<user> users { get; set; }
		public DbSet<Stable> Stables { get; set; }
		public DbSet<Trainee> Trainees { get; set; }
		public DbSet<Horse> Horses { get; set; }
		public DbSet<TrainingTracker> TrainingTracker { get; set; }
	}
}
