using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HorsesPOC.Services.OtpService
{
	public sealed class TwilioOptions
	{
		public string AccountSid { get; set; } = "";
		public string AuthToken { get; set; } = "";
		public string WhatsAppFrom { get; set; } = "";
		public string ContentSid { get; set; } = "";
	}

	public interface IWhatsAppSender
	{
		Task SendOtpAsync(string toE164, string otp, CancellationToken ct = default);
	}

	public sealed class TwilioWhatsAppSender : IWhatsAppSender
	{
		private readonly TwilioOptions _opt;

		public TwilioWhatsAppSender(IOptions<TwilioOptions> opt)
		{
			_opt = opt.Value;
			TwilioClient.Init(_opt.AccountSid, _opt.AuthToken);
		}

		public async Task SendOtpAsync(string toE164, string otp, CancellationToken ct = default)
		{

			await MessageResource.CreateAsync(
				from: new Twilio.Types.PhoneNumber(_opt.WhatsAppFrom),
				to: new Twilio.Types.PhoneNumber($"whatsapp:{toE164}"),
				contentSid: _opt.ContentSid,
				contentVariables: $"{{\"1\":\"{otp}\"}}"
			);
		}
	}
}