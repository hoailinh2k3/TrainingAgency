using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingAgency.Data;
using TrainingAgency.Models;

namespace TrainingAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly TrainingAgencyContext _context;

        public HomeController(TrainingAgencyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (Request.Cookies["UserRole"] != null && Request.Cookies["UserRole"].ToLower() == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }    
            var courses = await _context.Courses
                .Include(c => c.Registrations)
                .ToListAsync();

            if (Request.Cookies["UserId"] != null)
            {
                int userId = int.Parse(Request.Cookies["UserId"]!);
                var userRegistrations = await _context.Registrations
                    .Include(r => r.Course)
                    .Where(r => r.UserId == userId)
                    .ToListAsync();
                ViewBag.UserRegistrations = userRegistrations;
                ViewBag.UserId = userId;
            }
            else
            {
                ViewBag.UserRegistrations = null;
                ViewBag.UserId = null;
            }
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterCourse(int courseId)
        {
            if (Request.Cookies["UserId"] == null)
            {
                return RedirectToAction("Index");
            }
            var userId = int.Parse(Request.Cookies["UserId"]!);

            var registration = new Registration
            {
                UserId = userId,
                CourseId = courseId,
                RegistrationDate = DateTime.Now,
                PaymentStatus = true
            };
            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRegistration(int registrationId)
        {
            if (Request.Cookies["UserId"] == null)
            {
                return RedirectToAction("Index");
            }
            int userId = int.Parse(Request.Cookies["UserId"]!);
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId && r.UserId == userId);
            if (registration != null)
            {
                _context.Registrations.Remove(registration);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
