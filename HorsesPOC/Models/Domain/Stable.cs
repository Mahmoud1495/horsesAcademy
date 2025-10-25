using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Hosting;

namespace HorsesPOC.Models.Domain
{
	public class Stable
	{
		public Guid ID { get; set; }
		public Guid OwnerId { get; set; }
		public string Name { get; set; }
		public string Location { get; set; }

		[ValidateNever]
		public virtual user Owner { get; set; }
		[ValidateNever]
		public virtual ICollection<Horse> Horses { get; set; }
		[ValidateNever]
		public virtual ICollection<Trainee> Trainees { get; set; }
	}
}
