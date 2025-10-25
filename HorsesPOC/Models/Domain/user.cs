using HorsesPOC.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HorsesPOC.Models.Domain
{
	public class user
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		[ValidateNever]
		public string Password { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string NID { get; set; } = string.Empty;
		public UserEnum UserType { get; set; }

	}
}
