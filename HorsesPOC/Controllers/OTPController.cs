using HorsesPOC.Data;
using HorsesPOC.Services.OtpService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesPOC.Controllers
{
	[Authorize]
	[Route("otp")]
	public class OTPController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IOtpService _otpService;

		public OTPController(AppDbContext context, IOtpService otpService)
		{
			_context = context;
			_otpService = otpService;
		}

		[HttpPost("send")]
		[AllowAnonymous]
		public async Task<IActionResult> Send([FromBody] SendOtpDto dto, CancellationToken ct)
		{
			if (dto is null || string.IsNullOrWhiteSpace(dto.PhoneNumberE164) || !dto.PhoneNumberE164.StartsWith("+"))
				return BadRequest(new { message = "أدخل رقم بصيغة E.164 مثل +201234567890" });

			try
			{
				var trainee = await _context.Trainees.Where(t => t.PhoneNumber == dto.PhoneNumberE164).FirstOrDefaultAsync(ct);
				if (trainee != null) {
					await _otpService.CreateAndSendAsync(dto.PhoneNumberE164, ct);
					return Ok(new { message = "تم إرسال كود OTP على واتساب." });
				}
				else
				{
					return BadRequest(new { message = " هذا الرقم غير موجود" });
				}
				
			}
			catch (InvalidOperationException ex)
			{
				return StatusCode(429, new { message = ex.Message });
			}
			catch
			{
				return StatusCode(500, new { message = "فشل الإرسال." });
			}
		}

		[HttpPost("verify")]
		[AllowAnonymous]
		public async Task<IActionResult> Verify([FromBody] VerifyOtpDto dto, CancellationToken ct)
		{
			if (dto is null || string.IsNullOrWhiteSpace(dto.PhoneNumberE164) || string.IsNullOrWhiteSpace(dto.Code))
				return BadRequest(new { message = "بيانات غير مكتملة." });

			var now = DateTime.UtcNow;
			var row = await _context.OtpCodes
				.Where(x => x.PhoneNumber == dto.PhoneNumberE164 && !x.IsVerified && x.ExpiresAtUtc >= now)
				.OrderByDescending(x => x.CreatedAtUtc)
				.FirstOrDefaultAsync(ct);

			if (row is null)
				return BadRequest(new { message = "الكود منتهي أو غير موجود. اطلب كود جديد." });

			if (row.Attempts >= 5)
				return StatusCode(429, new { message = "تخطيت الحد المسموح للمحاولات." });

			row.Attempts++;

			if (!string.Equals(dto.Code, row.Code, StringComparison.Ordinal))
			{
				await _context.SaveChangesAsync(ct);
				return BadRequest(new { message = "الكود غير صحيح." });
			}

			row.IsVerified = true;
			await _context.SaveChangesAsync(ct);
			var trainee = await _context.Trainees.Where(t => t.PhoneNumber == dto.PhoneNumberE164).FirstOrDefaultAsync(ct);
			if (trainee == null)
				return Ok(new { valid = true, message = "تم التحقق بنجاح 🎉", qr_data = (string?)null });

			// ✅ Unified response format
			return Ok(new
			{
				valid = true,
				message = "تم التحقق بنجاح 🎉",
				qr_data = trainee.QRCode
			});
		}
	}

	public sealed class SendOtpDto { public string? PhoneNumberE164 { get; set; } }
	public sealed class VerifyOtpDto { public string? PhoneNumberE164 { get; set; } public string? Code { get; set; } }
}
