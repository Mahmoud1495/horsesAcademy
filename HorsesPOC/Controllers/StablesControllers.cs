using HorsesPOC.Data;
using HorsesPOC.Models.Domain;
using HorsesPOC.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HorsesPOC.Controllers
{
	[Authorize]
	public class StablesControllers : Controller
	{
		private readonly AppDbContext _context;
		private AuthraizationFilter _filter;

		public StablesControllers(AppDbContext context, AuthraizationFilter filter)
		{
			_context = context;
			_filter = filter;
		}


		// ✅ This runs before any action method
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!_filter.Can("Stables"))
			{
				// Return a 403 Forbidden or redirect to AccessDenied
				context.Result = new ForbidResult();
				// Or: context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}

			base.OnActionExecuting(context);
		}
		public async Task<IActionResult> Index()
		{
			return View(await _context.Stables.Include(s => s.Horses)
									   .Include(s => s.Trainees)
									   .Include(s => s.Owner).ToListAsync());
		}

		public async Task<IActionResult> Create()
		{
			await PopulateOwnersDropdown();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Stable stable)
		{
			if (ModelState.IsValid)
			{
				_context.Stables.Add(stable);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			await PopulateOwnersDropdown();
			return View(stable);
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var stable = await _context.Stables.FindAsync(id);
			if (stable == null)
				return NotFound();

			await PopulateOwnersDropdown();

			return View(stable);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Stable stable)
		{
			if (ModelState.IsValid)
			{
				_context.Update(stable);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			await PopulateOwnersDropdown();
			return View(stable);
		}

		public async Task<IActionResult> Delete(Guid id)
		{
			var stable = await _context.Stables.FindAsync(id);
			if (stable == null)
				return NotFound();

			return View(stable);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var stable = await _context.Stables.FindAsync(id);
			_context.Stables.Remove(stable);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(Guid id)
		{
			var stable = await _context.Stables
									   .Include(s => s.Horses)
									   .Include(s => s.Trainees)
									   .Include(s => s.Owner)
									   .FirstOrDefaultAsync(s => s.ID == id);
			if (stable == null)
				return NotFound();

			return View(stable);
		}

		private async Task PopulateOwnersDropdown()
		{

			var users = await _context.users.Where(u => u.UserType == Enums.UserEnum.StableAdmin &&!_context.Stables.Any(s => s.OwnerId == u.Id)).ToListAsync();

			ViewBag.Owners = new SelectList(users, "Id", "Name"); 
		}
	}
}
