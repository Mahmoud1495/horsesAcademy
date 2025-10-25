using HorsesPOC.Data;
using HorsesPOC.Enums;
using HorsesPOC.Models.Domain;
using HorsesPOC.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HorsesPOC.Controllers
{
	[Authorize]
	public class UsersController : Controller
	{
		private readonly AppDbContext _context;
		private AuthraizationFilter _filter;

		public UsersController(AppDbContext context, AuthraizationFilter filter)
		{
			_context = context;
			_filter = filter;
		}

		// ✅ This runs before any action method
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!_filter.Can("Users"))
			{
				// Return a 403 Forbidden or redirect to AccessDenied
				context.Result = new ForbidResult();
				// Or: context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}

			base.OnActionExecuting(context);
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.users.ToListAsync());
		}

		public async Task<IActionResult> Details(Guid id)
		{
			var user = await _context.users.FindAsync(id);
			if (user == null)
				return NotFound();

			return View(user);
		}

		public IActionResult Create()
		{
			ViewBag.UserTypes = new SelectList(Enum.GetValues(typeof(UserEnum)));
			return View(new user());
		}

		[HttpPost]
		public async Task<IActionResult> Create(user user, string confirmPassword)
		{
			if (ModelState.IsValid)
			{
				user.Id = Guid.NewGuid();
				user.Password = TripleDES.Encrypt(user.Password, false);
				_context.users.Add(user);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewBag.UserTypes = new SelectList(Enum.GetValues(typeof(UserEnum)));
			return View(user);
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var user = await _context.users.FindAsync(id);
			if (user == null)
				return NotFound();

			ViewBag.UserTypes = new SelectList(Enum.GetValues(typeof(UserEnum)), user.UserType);
			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(user user)
		{
			if (ModelState.IsValid)
			{
				var existingUser = await _context.users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
				if (existingUser == null)
					return NotFound();

				// Re-encrypt only if password has changed
				if (user.Password == null) 
				{
					user.Password = existingUser.Password;
				}
				else if (user.Password != existingUser.Password)
				{
					user.Password = TripleDES.Encrypt(user.Password, false);
				}

					_context.users.Update(user);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewBag.UserTypes = new SelectList(Enum.GetValues(typeof(UserEnum)), user.UserType);
			return View(user);
		}

		public async Task<IActionResult> Delete(Guid id)
		{
			var user = await _context.users.FindAsync(id);
			if (user == null)
				return NotFound();

			return View(user);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var user = await _context.users.FindAsync(id);
			_context.users.Remove(user);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
