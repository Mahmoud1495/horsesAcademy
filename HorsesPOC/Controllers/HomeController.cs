using Azure.Core;
using HorsesPOC.Data;
using HorsesPOC.Models;
using HorsesPOC.Models.Domain;
using HorsesPOC.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HorsesPOC.Controllers
{
	[Authorize]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;
		private AuthraizationFilter _filter;

		public HomeController(ILogger<HomeController> logger, AppDbContext context, AuthraizationFilter filter)
        {
			_context = context;
			_logger = logger;
			_filter = filter;
		}

		//This runs before any action method
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!_filter.Can("Home"))
			{
				// Return a 403 Forbidden or redirect to AccessDenied
				context.Result = new ForbidResult();
				// Or: context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
			}

			base.OnActionExecuting(context);
		}

		public IActionResult Index()
        {
            var Trainees = _context.Trainees.Where(t => t.StableID == Guid.Parse(_filter.GetStableID()));
            var Horses = _context.Horses.Where(t => t.StableID == Guid.Parse(_filter.GetStableID()));
			ViewBag.TraineesList =  new SelectList(Trainees, "ID", "Name");
			ViewBag.Horses =  new SelectList(Horses, "ID", "Name");
			return View();
        }

		public async Task<Guid> CreateTrainingRecord(Guid traineeId)
		{
			var tracker = new TrainingTracker()
			{
				Id = Guid.NewGuid(),
				StartTime = DateTime.Now,
				TraineeId = traineeId,
				Stage = Enums.SessionStage.One
			};
			await _context.TrainingTracker.AddAsync(tracker);

			await _context.SaveChangesAsync();
			return tracker.Id;
		}

		public async Task<bool> UpdateTrainingRecord(Guid sessionID,int actualMin)
		{
			var tracker = await _context.TrainingTracker.FirstOrDefaultAsync(t => t.Id == sessionID);
			tracker.EndTime = DateTime.Now;
			tracker.ActualTrainingInMin = actualMin;
			_context.Update(tracker);
			await _context.SaveChangesAsync();
			return true;
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
