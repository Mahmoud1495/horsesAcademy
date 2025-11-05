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
		public DbSet<OtpCode> OtpCodes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Horse → Stable
			modelBuilder.Entity<Horse>()
				.HasOne(h => h.Stable)
				.WithMany(s => s.Horses)
				.HasForeignKey(h => h.StableID)
				.OnDelete(DeleteBehavior.Restrict);  // prevents cascade delete

			// Trainee → Stable
			modelBuilder.Entity<Trainee>()
				.HasOne(t => t.Stable)
				.WithMany(s => s.Trainees)
				.HasForeignKey(t => t.StableID)
				.OnDelete(DeleteBehavior.Restrict);

			// TrainingTracker → Horse
			modelBuilder.Entity<TrainingTracker>()
				.HasOne(tt => tt.Horse)
				.WithMany(h => h.Sessions)
				.HasForeignKey(tt => tt.HorseId)
				.OnDelete(DeleteBehavior.Restrict); // <- key part

			// TrainingTracker → Trainee
			modelBuilder.Entity<TrainingTracker>()
				.HasOne(tt => tt.Trainee)
				.WithMany(t => t.Sessions)
				.HasForeignKey(tt => tt.TraineeId)
				.OnDelete(DeleteBehavior.Restrict); // <- key part
		}
	}
}
