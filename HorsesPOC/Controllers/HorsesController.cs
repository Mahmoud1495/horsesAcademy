using HorsesPOC.Data;
using HorsesPOC.Models.Domain;
using HorsesPOC.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HorsesPOC.Controllers
{
	[Authorize]
	public class HorsesController : Controller
	{
		private readonly AppDbContext _context;
		private AuthraizationFilter _filter;

		public HorsesController(AppDbContext context, AuthraizationFilter filter)
		{
			_context = context;
			_filter = filter;

		}

		// ✅ This runs before any action method
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!_filter.Can("Horses"))
			{
				// Return a 403 Forbidden or redirect to AccessDenied
				context.Result = new ForbidResult();
				// Or: context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}

			base.OnActionExecuting(context);
		}


		public async Task<IActionResult> Index(Guid? StableID)
		{
			var Horses = _context.Horses.AsQueryable();

			if (StableID.HasValue)
				return View(await _context.Horses.Where(t => t.StableID == StableID).Include(s => s.Stable).ToListAsync());
			else
				return View(await _context.Horses.Include(s => s.Stable).ToListAsync());

		}

		[HttpGet]
		public IActionResult Create(Guid? StableID)
		{
			var horse = new Horse();
			if (StableID.HasValue)
				horse.StableID = StableID.Value;

			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();

			//ViewBag.Stables = new SelectList(_context.Stables, "ID", "Name");
			ViewBag.GenderList = new List<SelectListItem>
{
	new SelectListItem { Value = "Male", Text = "Male" },
	new SelectListItem { Value = "Female", Text = "Female" }
};

			return View(horse);
		}

		[HttpPost]
		public async Task<IActionResult> Create(Horse horse)
		{
			if (ModelState.IsValid)
			{
				// Generate unique ID
				horse.ID = Guid.NewGuid();
				// Create the folder if it doesn't exist
				var qrFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrhorses");
				if (!Directory.Exists(qrFolder))
					Directory.CreateDirectory(qrFolder);

				string qrContent = $"horse:{horse.ID}";

				// Generate QR code
				using (var qrGenerator = new QRCoder.QRCodeGenerator())
				{
					var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCoder.QRCodeGenerator.ECCLevel.Q);
					var qrCode = new QRCoder.QRCode(qrCodeData);
					using (var qrBitmap = qrCode.GetGraphic(20))
					{
						var qrFileName = $"{horse.ID}.png";
						var qrFilePath = Path.Combine(qrFolder, qrFileName);
						qrBitmap.Save(qrFilePath, System.Drawing.Imaging.ImageFormat.Png);

						// Store relative path for later display
						horse.QRCode = $"/qrhorses/{qrFileName}";
					}
				}

				_context.Horses.Add(horse);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();
			ViewBag.GenderList = new List<SelectListItem>
{
	new SelectListItem { Value = "Male", Text = "Male" },
	new SelectListItem { Value = "Female", Text = "Female" }
};
			return View(horse);
		}

		public async Task<IActionResult> Edit(Guid id)
		{

			var horse = await _context.Horses.FindAsync(id);
			if (horse == null)
				return NotFound();

			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();
			ViewBag.GenderList = new List<SelectListItem>
{
	new SelectListItem { Value = "Male", Text = "Male" },
	new SelectListItem { Value = "Female", Text = "Female" }
};

			return View(horse);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Horse horse)
		{

			if (ModelState.IsValid)
			{
				var existhorse = await _context.Horses.AsNoTracking().FirstOrDefaultAsync(u => u.ID == horse.ID);
				if (existhorse == null)
					return NotFound();

				horse.QRCode = existhorse.QRCode;
				_context.Update(horse);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();
			ViewBag.GenderList = new List<SelectListItem>
{
	new SelectListItem { Value = "Male", Text = "Male" },
	new SelectListItem { Value = "Female", Text = "Female" }
};
			return View(horse);
		}

		public async Task<IActionResult> Delete(Guid id)
		{
			var horse = await _context.Horses.FindAsync(id);
			if (horse == null)
				return NotFound();

			return View(horse);
		}

		[HttpPost, ActionName("DeleteConfirmed")]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var horse = await _context.Horses.FindAsync(id);
			_context.Horses.Remove(horse);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(Guid id)
		{
			var horse = await _context.Horses
				.Include(s => s.Stable)
				.FirstOrDefaultAsync(s => s.ID == id);
			if (horse == null)
				return NotFound();

			return View(horse);
		}
	}
}
