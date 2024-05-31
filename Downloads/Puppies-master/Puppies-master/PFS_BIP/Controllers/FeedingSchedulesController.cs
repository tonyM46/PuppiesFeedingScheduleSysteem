using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PFS_BIP.Data;
using PFS_BIP.JSONEntities;
using PFS_BIP.Models;

namespace PFS_BIP.Controllers
{

    public class FeedingSchedulesController : Controller
    {
        private readonly PFSDbContext _context;

        public FeedingSchedulesController(PFSDbContext context)
        {
            _context = context;
        }

        // GET: FeedingSchedules
        public async Task<IActionResult> Index()
        {
            // Haal alle FeedingSchedules op
            var feedingSchedules = await _context.FeedingSchedules
                .Include(fs => (fs as FeedingSchedule).FeedingTimes)// Optioneel, als je de FeedingTimes ook wilt laden
                .Include(fs => (fs as FeedingSchedule).Puppy) // Voeg de Puppy-entiteit toe
                .ToListAsync();


            return View(feedingSchedules);
        }


        // GET: FeedingSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedingSchedule = await _context.FeedingSchedules
                .Include(f => f.Puppy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedingSchedule == null)
            {
                return NotFound();
            }

            return View(feedingSchedule);
        }

        // GET: FeedingSchedules/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["PuppyId"] = new SelectList(_context.Puppies, "Id", "Name");
            return View();
        }



        // POST: FeedingSchedules/Create
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Create([FromBody] JSONFeedingSchedule model)
        {
            if (model == null)
            {
                return BadRequest("Invalid JSON data.");
            }

            var feedingSchedule = new FeedingSchedule
            {
                PuppyId = model.PuppyId,
                NumberOfMeals = model.NumberOfMeals,
                IsFed = false
            };

            var puppy = await _context.Puppies.FindAsync(model.PuppyId);
            if(puppy !=null && puppy.InQuarantine)
            {
                Console.WriteLine("Warning: Scheduling for a puppy in quarantine");
            }
            // Voeg het voedingsschema toe aan de database
            _context.FeedingSchedules.Add(feedingSchedule);
            await _context.SaveChangesAsync();

            // Voeg FeedingTimes toe aan het aangemaakte voedingsschema
            try
            {
                if (model.FeedingTimes != null && model.FeedingTimes.Count > 0)
                {
                    foreach (var feedingTime in model.FeedingTimes)
                    {
                        var newFeedingTime = new FeedingTime
                        {
                            FeedingScheduleId = feedingSchedule.Id,
                            StartTime = DateTime.Parse(feedingTime.StartTime),
                            EndTime = DateTime.Parse(feedingTime.EndTime)
                        };
                        _context.FeedingTimes.Add(newFeedingTime);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

           return Ok(new { RedirectUrl = Url.Action("Index", "FeedingSchedules") });
        }



        // GET: FeedingSchedules/Edit/5
        [Authorize]
        [Consumes("application/json")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedingSchedule = await _context.FeedingSchedules.FindAsync(id);
            if (feedingSchedule == null)
            {
                return NotFound();
            }
            ViewData["PuppyId"] = new SelectList(_context.Puppies, "Id", "Name", feedingSchedule.PuppyId);
            return View(feedingSchedule);
        }

        // POST: FeedingSchedules/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PuppyId,StartTime,EndTime,NumberOfMeals")] FeedingSchedule feedingSchedule)
        {
            if (id != feedingSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedingSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedingScheduleExists(feedingSchedule.Id))
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
            ViewData["PuppyId"] = new SelectList(_context.Puppies, "Id", "Name", feedingSchedule.PuppyId);
            return View(feedingSchedule);
        }


        // GET: FeedingSchedules/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedingSchedule = await _context.FeedingSchedules
                .Include(f => f.Puppy)
                .Include(f => f.FeedingTimes) // Include FeedingTimes to display them in the view if needed
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedingSchedule == null)
            {
                return NotFound();
            }

            return View(feedingSchedule);
        }

        // POST: FeedingSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedingSchedule = await _context.FeedingSchedules
                .Include(f => f.FeedingTimes) // Include FeedingTimes to delete them
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedingSchedule != null)
            {
                // Remove the related FeedingTimes first
                _context.FeedingTimes.RemoveRange(feedingSchedule.FeedingTimes);
                // Remove the FeedingSchedule
                _context.FeedingSchedules.Remove(feedingSchedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FeedingScheduleExists(int id)
        {
            return _context.FeedingSchedules.Any(e => e.Id == id);
        }

    }
}
