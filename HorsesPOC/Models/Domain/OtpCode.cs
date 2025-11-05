using HorsesPOC.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace HorsesPOC.Models.Domain
{
	

	public class OtpCode
	{
		public int Id { get; set; }

		[Required, MaxLength(20)]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required, MaxLength(6)]
		public string Code { get; set; } = string.Empty;

		public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

		public DateTime ExpiresAtUtc { get; set; } /*= DateTime.UtcNow.AddMinutes(3);*/

		public int Attempts { get; set; } = 0;

		public bool IsVerified { get; set; } = false;
	}
}
