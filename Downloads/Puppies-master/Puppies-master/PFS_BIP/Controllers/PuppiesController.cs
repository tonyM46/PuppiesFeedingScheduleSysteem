using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using PFS_BIP.Data;
using PFS_BIP.Models;

namespace PFS_BIP.Controllers
{

    public class PuppiesController : Controller
    {
        private readonly PFSDbContext _context;


        public PuppiesController(PFSDbContext context)
        {
            _context = context;
        }

        // GET: Puppies
        public async Task<IActionResult> Index(string group,bool? inQuarantine)
        {
            IQueryable<Puppies> puppies = _context.Puppies;

            if (!string.IsNullOrEmpty(group))
            {
                puppies = puppies.Where(p => p.Group == group);
            }


            if (inQuarantine.HasValue)
            {
                puppies = puppies.Where(p => p.InQuarantine == inQuarantine.Value);
            }

            return View(await puppies.ToListAsync());
        }

        // GET: Puppies/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puppies = await _context.Puppies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (puppies == null)
            {
                return NotFound();
            }

            return View(puppies);
        }
      

        // GET: Puppies/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Puppies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age,Weight,ChipNumber,InQuarantine,Group")] Puppies puppies)
        {
            try
            {
                // Check if ChipNumber already exists
                if (_context.Puppies.Any(p => p.ChipNumber == puppies.ChipNumber))
                {
                    ModelState.AddModelError("ChipNumber", "Chip Number already exists.");
                }

                // Check ModelState validity
                if (ModelState.IsValid)
                {
                    // Add the new Puppies entity to the context
                    _context.Add(puppies);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        if (state.Value.Errors.Count > 0)
                        {
                            Console.WriteLine($"Key: {state.Key}, Error: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                        }
                    }
                }

                // If we get here, something failed, redisplay form
                Console.WriteLine("ModelState is not valid.");
                return View(puppies);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle specific database update exception
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                return View(puppies);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                ModelState.AddModelError(string.Empty, "Error saving changes: " + ex.Message);
                Console.WriteLine($"Exception: {ex.Message}");
                return View(puppies);
            }
        }

        // GET: Puppies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puppies = await _context.Puppies.FindAsync(id);
            if (puppies == null)
            {
                return NotFound();
            }
            return View(puppies);
        }

        // POST: Puppies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,Weight,ChipNumber,InQuarantine,Group")] Puppies puppies)
        {
            if (id != puppies.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(puppies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PuppiesExists(puppies.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(puppies);
        }

        // GET: Puppies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var puppies = await _context.Puppies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (puppies == null)
            {
                return NotFound();
            }

            return View(puppies);
        }

        // POST: Puppies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var puppies = await _context.Puppies.FindAsync(id);
            if (puppies != null)
            {
                _context.Puppies.Remove(puppies);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Puppies/Quarantine
        public async Task<IActionResult> Quarantine()
        {
            var quarantinePuppies = await _context.Puppies.Where(p => p.InQuarantine).ToListAsync();
            return View(quarantinePuppies);
        }

        // GET: Puppy / ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // Post: Puppy / ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            var matchingPuppies = await _context.Puppies.Where(p => p.Name.Contains(SearchPhrase)).ToListAsync();

            if (matchingPuppies.Count == 0)
            {
                TempData["Message"] = "No puppies found with the given name.";
                return View("ShowSearchForm");
            }

            return View("Index", matchingPuppies);
        }




        //GET: Feed 
        [Authorize]
        public async Task<IActionResult> Feed(int id)
        {
            var puppy = await _context.Puppies.FindAsync(id);

            if (puppy == null)
            {
                return NotFound();
            }

            // Haal alle voedingstijden op die zijn gekoppeld aan de puppy
            var feedingSchedules = await _context.FeedingSchedules
                .Include(fs => fs.FeedingTimes)
                .Where(fs => fs.PuppyId == id)
                .ToListAsync();

            // Filter de voedingstijden op basis van de starttijd
            var today = DateTime.Today;
            var todaysFeedingTimes = feedingSchedules.SelectMany(fs => fs.FeedingTimes)
                .Where(ft => ft.StartTime.Date == today)
                .ToList();
            var futureFeedingTimes = feedingSchedules.SelectMany(fs => fs.FeedingTimes)
                .Where(ft => ft.StartTime.Date > today)
                .ToList();

            // Stuur de puppy en de voedingstijden naar de view
            ViewBag.PuppyName = puppy.Name;
            ViewBag.TodaysSchedules = todaysFeedingTimes.Select(ft => new
            {
                ft.StartTime,
                ft.EndTime,
                ft.FeedingSchedule.NumberOfMeals,
                ft.FeedingSchedule.Id
            }).ToList();
            ViewBag.FutureSchedules = futureFeedingTimes.Select(ft => new
            {
                ft.StartTime,
                ft.EndTime,
                ft.FeedingSchedule.NumberOfMeals,
                ft.FeedingSchedule.Id
            }).ToList();

            return View();
        }


        [HttpPost]
        [HttpGet] // to accept it for post and get
        [Authorize]
        public async Task<IActionResult> CheckFeeding(int feedingScheduleId, int mealIndex)
        {
            try
            {
                // search schedule in  database
                var feedingSchedule = await _context.FeedingSchedules
                    .Include(fs => fs.FeedingTimes)
                    .Include(fs => fs.Puppy) 
                    .FirstOrDefaultAsync(fs => fs.Id == feedingScheduleId);

                if (feedingSchedule != null)
                {
                    feedingSchedule.IsFed = true;
                    var puppyId = feedingSchedule.PuppyId;
                    var puppyExists = await _context.Puppies.AnyAsync(p => p.Id == puppyId);
                    if (!puppyExists)
                    {
                        return Json(new { success = false, error = "Puppy does not exist." });
                    }

                    var historicalFeedingSchedule = new HistoricalFeedingSchedule
                    {
                        PuppyId = puppyId,
                        NumberOfMeals = 1,
                        StartTime = feedingSchedule.FeedingTimes.FirstOrDefault().StartTime, 
                        EndTime = feedingSchedule.FeedingTimes.FirstOrDefault().EndTime 
                    };

                    _context.HistoricalFeedingSchedules.Add(historicalFeedingSchedule);

                   
                    feedingSchedule.FeedingTimes.RemoveAt(mealIndex);

                    await _context.SaveChangesAsync();

                    // Doorverwijzen naar een actie voor het bekijken van historische voedingsschema's
                    //return RedirectToAction("HistoricalFeeding", new { puppyId = historicalFeedingSchedule.PuppyId });
                    return Json(new { success = true, puppyId = historicalFeedingSchedule.PuppyId });
                }
                else
                {
                    // JSON teruggeven voor foutmelding als voedingsschema niet gevonden is
                    return Json(new { success = false, error = "Feeding schedule not found." });
                }
            }
            catch (Exception ex)
            {
                //Log de volledige inner exception voor meer details
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    ex = innerException;
                    innerException = innerException.InnerException;
                }

                // Log de volledige fout inclusief de inner exception
                var errorMessage = ex.Message;
                return Json(new { success = false, error = errorMessage });
            }
        }

        [HttpPost]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> HistoricalFeeding(int? puppyId, DateTime? filterDate)
        {

            var puppy = await _context.Puppies.FindAsync(puppyId);

            ViewBag.PuppyId = puppyId;

            Console.WriteLine($"Received puppyId: {puppyId}");
            if (puppyId == null)
            {
                return BadRequest("Puppy ID is required.");
            }

             

            // Retrieve historical feeding schedules for the specific puppy from the database
            var historicalSchedules = await _context.HistoricalFeedingSchedules
                .Where(h => h.PuppyId == puppyId)
                .OrderBy(h => h.StartTime)
                .ToListAsync();

            // Retrieve the puppy name from the database based on the puppyId
            string? puppyName = await _context.Puppies
                .Where(p => p.Id == puppyId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();

            Console.WriteLine($"Puppy Name: {puppyName}, Historical Schedules Count: {historicalSchedules.Count}");
            
            if (string.IsNullOrEmpty(puppyName))
            {
                return BadRequest("Puppy name is required.");
            }
            // Pass both historical feeding schedules and puppy name to the view
            var historicalFeedingSchedule = new HistoricalFeedingViewModel
            {
                PuppyId = puppy.Id,
                HistoricalSchedules = historicalSchedules,
                PuppyName = puppyName
            };

            if (filterDate != null)
            {
                historicalFeedingSchedule.HistoricalSchedules = historicalFeedingSchedule.HistoricalSchedules
                    .Where(schedule => schedule.StartTime.Date == filterDate.Value.Date)
                    .ToList();
            }
            

            return View(historicalFeedingSchedule);
        }

        public async Task<IActionResult> GetPuppyStatus(int id)
        {
            var puppy = await _context.Puppies.FindAsync(id);
            if(puppy== null)
            {
                return NotFound();
            }

            return Ok(new { inQuarantine = puppy.InQuarantine });
        }



        private bool PuppiesExists(int id)
        {
            return _context.Puppies.Any(e => e.Id == id);
        }
    }
}
