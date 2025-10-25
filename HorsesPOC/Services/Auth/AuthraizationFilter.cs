using HorsesPOC.Enums;
using System.Security.Claims;

namespace HorsesPOC.Services.Auth
{
	public class AuthraizationFilter
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthraizationFilter(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public string GetCurrentUserId()
		{
			return _httpContextAccessor.HttpContext?.User?
				.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		public string GetCurrentUserType()
		{
			return _httpContextAccessor.HttpContext?.User?
				.FindFirst("UserType")?.Value;
		}

		public string GetStableID()
		{
			return _httpContextAccessor.HttpContext?.User?
				.FindFirst("StableID")?.Value;
		}


		public bool Can(string Controller)
		{
			var userType = GetCurrentUserType();
			if (userType == "Admin" && Controller != "Home")
			{
				return true;
			}
			else if (userType == "Admin" && Controller == "Home")
			{
				return false;
			}
			else
			{
				if (Controller == "Horses" 
					|| Controller == "Trainees" || Controller == "Home")
					return true;
				else
					return false;
			}
		}
	}
}
