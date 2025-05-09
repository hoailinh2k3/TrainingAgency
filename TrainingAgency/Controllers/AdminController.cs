using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TrainingAgency.Data;
using TrainingAgency.Models;

namespace TrainingAgency.Controllers
{
    public class AdminController : Controller
    {
        private readonly TrainingAgencyContext _context;

        public AdminController(TrainingAgencyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var courses = await _context.Courses
                .Include(c => c.Registrations)
                .ToListAsync();
            var students = await _context.Users
                .Where(u => u.Role.ToLower() != "admin")
                .ToListAsync();
            var registrations = await _context.Registrations
                .Include(r => r.Course)
                .Include(r => r.User)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            ViewBag.Courses = courses;
            ViewBag.Students = students;
            ViewBag.Registrations = registrations;

            return View();
        }

        public IActionResult CreateCourse()
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) || 
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) || 
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(course);
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditCourse(int id)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpPost]
        public async Task<IActionResult> EditCourse(int id, Course course)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(course);
            }

            var newCourse = await _context.Courses.FindAsync(id);
            if (newCourse == null)
            {
                return NotFound();
            }

            newCourse.CourseName = course.CourseName;
            newCourse.LecturerName = course.LecturerName;
            newCourse.StartDate = course.StartDate;
            newCourse.Fee = course.Fee;
            newCourse.MaxStudents = course.MaxStudents;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCourse(int id)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var registrations = _context.Registrations.Where(r => r.CourseId == id);
            _context.Registrations.RemoveRange(registrations);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> EditStudent(int id)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditStudent(int id, User user)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var newStudent = await _context.Users.FindAsync(id);
            if (newStudent == null)
            {
                return NotFound();
            }

            newStudent.FullName = user.FullName;
            newStudent.Email = user.Email;
            newStudent.Phone = user.Phone;
            newStudent.DateOfBirth = user.DateOfBirth;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var registrations = _context.Registrations.Where(r => r.UserId == id);
            _context.Registrations.RemoveRange(registrations);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> CheckEmail(int id, string email)
        {
            bool isUsed = await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.UserId != id);
            return Json(!isUsed);   
        }
        [HttpGet]
        public async Task<IActionResult> CheckPhoneNumber(int id, string phone)
        {
            bool isUsed = await _context.Users.AnyAsync(u => u.Phone == phone && u.UserId != id);
            return Json(!isUsed);
        }

        public async Task<IActionResult> DeleteRegistration(int id)
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var reg = await _context.Registrations.FindAsync(id);
            if (reg == null)
            {
                return NotFound();
            }

            _context.Registrations.Remove(reg);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Statistics()
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserRole"]) ||
                Request.Cookies["UserRole"].ToLower() != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var totalCourses = await _context.Courses.CountAsync();
            var totalStudents = await _context.Users.Where(u => u.Role.ToLower() != "admin").CountAsync();
            var totalRegistrations = await _context.Registrations.CountAsync();
            
            var totalRevenue = await _context.Registrations
                .Where(r => r.PaymentStatus)
                .Select(r => r.Course.Fee)
                .SumAsync();

            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalRegistrations = totalRegistrations;
            ViewBag.TotalRevenue = totalRevenue;

            // Quan trong
            var revenueByDayQuery = await _context.Registrations
                .Where(r => r.PaymentStatus)
                .Include(r => r.Course)
                .GroupBy(r => r.RegistrationDate.Date)
                .Select(g => new
                {
                    Date = g.Key.Date,
                    Revenue = g.Sum(r => r.Course.Fee)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var minDate = revenueByDayQuery.Min(x => x.Date);
            var maxDate = revenueByDayQuery.Max(x => x.Date);
            var allDates = Enumerable.Range(0, (maxDate - minDate).Days + 1)
                                     .Select(d => minDate.AddDays(d).ToString("yyyy-MM-dd"))
                                     .ToList();

            var dayRevenues = allDates.Select(date =>
            {
                var revenue = revenueByDayQuery.FirstOrDefault(x => x.Date.ToString("yyyy-MM-dd") == date)?.Revenue ?? 0;
                return revenue;
            }).ToList();

            ViewBag.DayLabels = Newtonsoft.Json.JsonConvert.SerializeObject(allDates);
            ViewBag.DayRevenues = Newtonsoft.Json.JsonConvert.SerializeObject(dayRevenues);

            // Quan trong
            var revenueByCourseQuery = await _context.Registrations
                .Where(r => r.PaymentStatus)
                .Include(r => r.Course)
                .GroupBy(r => new { r.Course.CourseId, r.Course.CourseName })
                .Select(g => new
                {
                    CourseName = g.Key.CourseName,
                    Revenue = g.Sum(r => r.Course.Fee)
                })
                .OrderBy(x => x.CourseName)
                .ToListAsync();

            var allCourses = revenueByCourseQuery.Select(x => x.CourseName).ToList();
            var courseRevenues = revenueByCourseQuery.Select(x => x.Revenue).ToList();

            ViewBag.CourseLabels = Newtonsoft.Json.JsonConvert.SerializeObject(allCourses);
            ViewBag.CourseRevenues = Newtonsoft.Json.JsonConvert.SerializeObject(courseRevenues);

            return View();
        }
    }
}
