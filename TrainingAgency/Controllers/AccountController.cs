using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainingAgency.Data;
using TrainingAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace TrainingAgency.Controllers
{
    public class AccountController : Controller
    {
        private readonly TrainingAgencyContext _context;

        public AccountController(TrainingAgencyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (Request.Cookies["UserId"] != null)
            {
                var item = await _context.Users.FindAsync(int.Parse(Request.Cookies["UserId"]!));
                return View(item);
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string fullName, string email, string phone, DateOnly? dateOfBirth)
        {
            if (Request.Cookies["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            int userId = int.Parse(Request.Cookies["UserId"]!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            user.FullName = fullName;
            user.Email = email;
            user.Phone = phone;
            user.DateOfBirth = dateOfBirth;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            if (Request.Cookies["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            int userId = int.Parse(Request.Cookies["UserId"]!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.Password != currentPassword)
            {
                ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không đúng.");
                ViewBag.ShowChangePasswordModal = true;
                return View("Index", user);
            }

            user.Password = newPassword;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }

            Response.Cookies.Append("UserId", user.UserId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            Response.Cookies.Append("UserRole", user.Role, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            if (user.Role == "admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(
            string username,
            string password,
            string fullName,
            string email,
            string phone,
            DateOnly? dateOfBirth)
        {
            var newUser = new User
            {
                Username = username,
                Password = password,
                FullName = fullName,
                Email = email,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Role = "user"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsername(string username)
        {
            bool isUsed = await _context.Users.AnyAsync(u => u.Username == username);
            return Json(!isUsed);
        }
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if(Request.Cookies["UserId"] != null)
            {
                bool isUsed = await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.UserId != int.Parse(Request.Cookies["UserId"]!));
                return Json(!isUsed);
            }
            else
            {
                bool isUsed = await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
                return Json(!isUsed);
            }           
        }
        [HttpGet]
        public async Task<IActionResult> CheckPhoneNumber(string phone)
        {
            if (Request.Cookies["UserId"] != null)
            {
                bool isUsed = await _context.Users.AnyAsync(u => u.Phone == phone && u.UserId != int.Parse(Request.Cookies["UserId"]!));
                return Json(!isUsed);
            }
            else
            {
                bool isUsed = await _context.Users.AnyAsync(u => u.Phone == phone);
                return Json(!isUsed);
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserRole");
            return RedirectToAction("Index", "Home");
        }
    }

}