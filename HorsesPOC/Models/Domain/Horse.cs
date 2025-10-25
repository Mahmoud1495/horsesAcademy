using HorsesPOC.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HorsesPOC.Models.Domain
{
	

	public class Horse
	{
		public Guid ID { get; set; }
		public Guid StableID { get; set; }

		public string Name { get; set; }
		public float Weight { get; set; }
		public string Vaccine { get; set; }
		public int Age { get; set; }
		public DateTime BirthDate { get; set; }
		public float Height { get; set; }
		public string Gender { get; set; }

		public HorseCategory Category { get; set; }
		public string StatusAndCondition { get; set; }

		public TimeSpan? RideTimeStart { get; set; }
		public TimeSpan? RideTimeEnd { get; set; }

		[ValidateNever]
		public virtual Stable Stable { get; set; }
	}
}
