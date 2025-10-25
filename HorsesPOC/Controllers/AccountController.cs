
using HorsesPOC.Data;
using HorsesPOC.Models.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HorsesPOC.Controllers
{
	public class AccountController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;

		public AccountController(ILogger<HomeController> logger, AppDbContext context)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Login(string returnUrl = "/")
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(user model, string returnUrl = "/")
		{
			var user = _context.users.SingleOrDefault(u => u.UserName == model.UserName 
			&& u.Password == Services.Auth.TripleDES.Encrypt(model.Password, false)  );
			if (user == null)
			{
				ModelState.AddModelError("", "Invalid username or password");
				return View(model);
			}

			// Create claims
			var claims = new List<Claim>
{
	new Claim(ClaimTypes.Name, user.UserName),
	new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // user ID
    new Claim("UserType", user.UserType.ToString() ?? ""),               // e.g., "Owner", "Trainer", "Admin"
};
			var StableID = Guid.NewGuid();
			if (user.UserType == Enums.UserEnum.StableAdmin)
			{
				StableID = _context.Stables.FirstOrDefault(s => s.OwnerId == user.Id).ID;
				// Add StableID only if it exists
				if (StableID != null)
				{
					claims.Add(new Claim("StableID", StableID.ToString()));
				}
			}


			// Create identity and principal
			var identity = new ClaimsIdentity(claims, "login");
			var principal = new ClaimsPrincipal(identity);

			// Sign in
			await HttpContext.SignInAsync(principal);
			if(user.UserType == Enums.UserEnum.Admin) { return RedirectToAction("Index", "StablesControllers"); }
			else
				return Redirect(returnUrl);
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Account");
		}

		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
