using HorsesPOC.Data;
using HorsesPOC.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HorsesPOC.Services.OtpService
{
	public interface IOtpService
	{
		Task CreateAndSendAsync(string phoneE164, CancellationToken ct = default);
	}

	public sealed class OtpService : IOtpService
	{
		private readonly AppDbContext _db;
		private readonly IWhatsAppSender _wa;

		public OtpService(AppDbContext db, IWhatsAppSender wa)
		{
			_db = db;
			_wa = wa;
		}

		public async Task CreateAndSendAsync(string phoneE164, CancellationToken ct = default)
		{
			var oneMinuteAgo = DateTime.UtcNow.AddSeconds(-60);
			var recent = await _db.OtpCodes
				.AnyAsync(x => x.PhoneNumber == phoneE164 && x.CreatedAtUtc >= oneMinuteAgo, ct);
			if (recent) throw new InvalidOperationException("يرجى الانتظار دقيقة قبل طلب كود جديد.");

			var otp = GenerateOtp();

			var row = new OtpCode
			{
				PhoneNumber = phoneE164,
				Code = otp,
				CreatedAtUtc = DateTime.UtcNow,
				ExpiresAtUtc = DateTime.UtcNow.AddMinutes(3),
				Attempts = 0,
				IsVerified = false
			};

			_db.OtpCodes.Add(row);
			await _db.SaveChangesAsync(ct);

			await _wa.SendOtpAsync(phoneE164, otp, ct);
		}

		private static string GenerateOtp()
		{
			int n = RandomNumberGenerator.GetInt32(0, 1_000_000);
			return n.ToString("D6");
		}
	}
}