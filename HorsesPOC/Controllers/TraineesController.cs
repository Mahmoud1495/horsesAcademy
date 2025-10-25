using HorsesPOC.Data;
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
	public class TraineesController : Controller
	{
		private readonly AppDbContext _context;
		private AuthraizationFilter _filter;

		public TraineesController(AppDbContext context, AuthraizationFilter filter)
		{
			_context = context;
			_filter = filter;
		}

		// ✅ This runs before any action method
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!_filter.Can("Trainees"))
			{
				// Return a 403 Forbidden or redirect to AccessDenied
				context.Result = new ForbidResult();
				// Or: context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}

			base.OnActionExecuting(context);
		}

		

		public async Task<IActionResult> Index(Guid? StableID)
		{
			var trainees = _context.Trainees.Include(s => s.Stable).AsQueryable();

			if (StableID.HasValue)
				trainees = trainees.Where(t => t.StableID == StableID);

			return View(await trainees.ToListAsync());
		}

		[HttpGet]
		public IActionResult Create(Guid? StableID)
		{
			
			var trainee = new Trainee();
			if (StableID.HasValue)
				trainee.StableID = StableID.Value;

			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();
			return View(trainee);
		}

		[HttpPost]
		public async Task<IActionResult> Create(Trainee trainee)
		{
			if (ModelState.IsValid)
			{
				// Generate unique ID
				trainee.ID = Guid.NewGuid();

				// Create the folder if it doesn't exist
				var qrFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");
				if (!Directory.Exists(qrFolder))
					Directory.CreateDirectory(qrFolder);

				// Use trainee ID or other info for QR content
				//string qrContent = $"Trainee:{trainee.ID}|Name:{trainee.Name}|Phone:{trainee.PhoneNumber}";
				string qrContent = $"{trainee.ID}";

				// Generate QR code
				using (var qrGenerator = new QRCoder.QRCodeGenerator())
				{
					var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCoder.QRCodeGenerator.ECCLevel.Q);
					var qrCode = new QRCoder.QRCode(qrCodeData);
					using (var qrBitmap = qrCode.GetGraphic(20))
					{
						var qrFileName = $"{trainee.ID}.png";
						var qrFilePath = Path.Combine(qrFolder, qrFileName);
						qrBitmap.Save(qrFilePath, System.Drawing.Imaging.ImageFormat.Png);

						// Store relative path for later display
						trainee.QRCode = $"/qrcodes/{qrFileName}";
					}
				}

				// Save trainee to DB
				_context.Trainees.Add(trainee);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index), new { StableID = trainee.StableID });
			}
			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();
			return View(trainee);
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var trainee = await _context.Trainees.FindAsync(id);
			if (trainee == null)
				return NotFound();

			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();

			return View(trainee);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Trainee trainee)
		{
			if (ModelState.IsValid)
			{
				var existtrainee = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(u => u.ID == trainee.ID);
				if (existtrainee == null)
					return NotFound();

				trainee.QRCode = existtrainee.QRCode;
				_context.Update(trainee);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			// ✅ Always return a list (even if empty)
			ViewBag.Stables = _context.Stables.ToList() ?? new List<Stable>();
			return View(trainee);
		}

		public async Task<IActionResult> Delete(Guid id)
		{
			var trainee = await _context.Trainees.FindAsync(id);
			if (trainee == null)
				return NotFound();

			return View(trainee);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var trainee = await _context.Trainees.FindAsync(id);
			_context.Trainees.Remove(trainee);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(Guid id)
		{
			var trainee = await _context.Trainees
				.Include(s => s.Stable)
				.FirstOrDefaultAsync(s => s.ID == id);
			if (trainee == null)
				return NotFound();

			return View(trainee);
		}
	}
}

