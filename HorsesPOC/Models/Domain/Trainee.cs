using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HorsesPOC.Models.Domain
{
	public class Trainee
	{
		public Guid ID { get; set; }
		public Guid StableID { get; set; }

		public string Name { get; set; }
		public int Age { get; set; }
		public string PhoneNumber { get; set; }
		[ValidateNever]
		public string QRCode { get; set; }

		[ValidateNever]
		public virtual Stable Stable { get; set; }
		[ValidateNever]
		public virtual ICollection<TrainingTracker> Sessions { get; set; }
	}
}
