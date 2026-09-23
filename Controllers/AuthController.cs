using CrudBackend_1_.Data;
using CrudBackend_1_.Dto;
using CrudBackend_1_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudBackend_1_.Controllers
{
    public class AuthController(AppDBContextcs context) : Controller // Dependency injection of the database context into the controller (modern way) Primary constructor.
    {
        // Traditional way to inject the database context into the controller (general).

        //private readonly AppDBContextcs context; 
        //public AuthController(AppDBContextcs context)
        //{
        //    this.context = context;
        //}

        public IActionResult Login()
        {

            ViewBag.successMessage = TempData["SuccessMessage"];
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> CreateUser(UserDto dto)
        {

            if (dto == null || string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.ErrorMessage = "All fields should be filled.";
                return View("Register", dto);
            }
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser == null)
            {
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = dto.Password
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                TempData["successMessage"] = "User registered successfully. Please log in."; // controller -> view

                return RedirectToAction("Login"); // This will keep the same url in the browser
                //return View("Login"); // This will change the url in the browser to /Auth/Login
            } else
            {
                ViewBag.ErrorMessage = "User with this email already exists."; // controller -> view
                return View("Register", dto); // Return the Register view with the dto to pre-fill the form
            }
        }

        public async Task<IActionResult> LoginUser(UserDto dto)
        {

            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.ErrorMessage = "All fields should be filled.";
                return View("Login", dto);
            }
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

            // Check if the user exists in the database
            if (existingUser != null)
            {
                ViewBag.SuccessMessage = "Login successful!";
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("Login");
            }
            
        }
    }
}
