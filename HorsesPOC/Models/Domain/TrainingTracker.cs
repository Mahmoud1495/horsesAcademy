using HorsesPOC.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HorsesPOC.Models.Domain
{
	public class TrainingTracker
	{
		public Guid Id { get; set; }
		public Guid TraineeId { get; set; }
		public Guid HorseId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public int? ActualTrainingInMin { get; set; }

		public SessionStage Stage { get; set; }

		[ValidateNever]
		public virtual Horse Horse { get; set; }

		[ValidateNever]
		public virtual Trainee Trainee { get; set; }
	}
}
