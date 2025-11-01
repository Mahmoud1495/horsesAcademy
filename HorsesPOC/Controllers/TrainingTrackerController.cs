using HorsesPOC.Data;
using HorsesPOC.Enums;
using HorsesPOC.Models.Domain;
using HorsesPOC.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HorsesPOC.Controllers
{
	[Authorize]
	public class TrainingTrackerController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;
		private AuthraizationFilter _filter;

		public TrainingTrackerController(ILogger<HomeController> logger, AppDbContext context, AuthraizationFilter filter)
		{
			_context = context;
			_logger = logger;
			_filter = filter;
		}

		//This runs before any action method
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!_filter.Can("TrainingTracker"))
			{
				// Return a 403 Forbidden or redirect to AccessDenied
				context.Result = new ForbidResult();
				// Or: context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}

			base.OnActionExecuting(context);
		}

		public async Task<IActionResult> Index(Guid? traineeId,
	Guid? horseId,
	DateTime? startDate)
		{
			var query = _context.TrainingTracker
				.Include(s => s.Horse)
				.Include(s => s.Trainee)
				.AsQueryable();

			// 🔹 Get current StableID from logged-in user
			var stableIdString = User.FindFirst("StableID")?.Value;
			if (!Guid.TryParse(stableIdString, out Guid stableId))
			{
				return Unauthorized(); // or handle accordingly
			}

			if (traineeId.HasValue)
				query = query.Where(t => t.TraineeId == traineeId);

			if (horseId.HasValue)
				query = query.Where(t => t.HorseId == horseId);


			if (startDate.HasValue)
				query = query.Where(t => t.StartTime >= startDate);

			if (stableIdString != "")
				query = query.Where(t => t.Horse.StableID == stableId);

			var results = await query.OrderByDescending(t => t.StartTime).ToListAsync();

			// ✅ Dropdown data
			ViewBag.Trainees = await _context.Trainees
				.OrderBy(t => t.Name)
				.ToListAsync();

			ViewBag.Horses = await _context.Horses
				.OrderBy(h => h.Name)
				.ToListAsync();

			ViewBag.SelectedTraineeId = traineeId;
			ViewBag.SelectedHorseId = horseId;
			ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");

			return View(results);
		}


		// GET: TrainingTracker/Create
		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.Trainees = _context.Trainees.ToList() ?? new List<Trainee>();
			ViewBag.Horses = _context.Horses.ToList() ?? new List<Horse>();
			return View(new TrainingTracker { StartTime = DateTime.Now });
		}

		// POST: TrainingTracker/Create
		[HttpPost]
		public async Task<IActionResult> Create(TrainingTracker tracker)
		{
			if (ModelState.IsValid)
			{
				tracker.Id = Guid.NewGuid();

				_context.TrainingTracker.Add(tracker);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			ViewBag.Trainees = _context.Trainees.ToList() ?? new List<Trainee>();
			ViewBag.Horses = _context.Horses.ToList() ?? new List<Horse>();
			return View(tracker);
		}

		// GET: TrainingTracker/Edit/{id}
		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			var tracker = await _context.TrainingTracker.FindAsync(id);
			if (tracker == null) return NotFound();

			ViewBag.Trainees = _context.Trainees.ToList() ?? new List<Trainee>();
			ViewBag.Horses = _context.Horses.ToList() ?? new List<Horse>();
			return View(tracker);
		}

		// POST: TrainingTracker/Edit
		[HttpPost]
		public async Task<IActionResult> Edit(TrainingTracker tracker)
		{
			if (ModelState.IsValid)
			{
				_context.Update(tracker);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewBag.Trainees = _context.Trainees.ToList() ?? new List<Trainee>();
			ViewBag.Horses = _context.Horses.ToList() ?? new List<Horse>();
			return View(tracker);
		}

		// GET: TrainingTracker/Delete/{id}
		public async Task<IActionResult> Delete(Guid id)
		{
			var tracker = await _context.TrainingTracker
				.Include(t => t.Trainee)
				.Include(t => t.Horse)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (tracker == null) return NotFound();

			return View(tracker);
		}

		// POST: TrainingTracker/DeleteConfirmed
		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var tracker = await _context.TrainingTracker.FindAsync(id);
			if (tracker == null) return NotFound();

			_context.TrainingTracker.Remove(tracker);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// GET: TrainingTracker/Details/{id}
		public async Task<IActionResult> Details(Guid id)
		{
			var tracker = await _context.TrainingTracker
				.Include(t => t.Trainee)
				.Include(t => t.Horse)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (tracker == null) return NotFound();

			return View(tracker);
		}
	}



}

