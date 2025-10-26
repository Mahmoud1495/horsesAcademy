using HorsesPOC.Enums;

namespace HorsesPOC.Models.Domain
{
	public class TrainingTracker
	{
		public Guid Id { get; set; }
		public Guid TraineeId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public int? ActualTrainingInMin { get; set; }

		public SessionStage Stage { get; set; }
	}
}
