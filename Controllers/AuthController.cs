using CrudBackend_1_.config;
using CrudBackend_1_.Data;
using CrudBackend_1_.Dto;
using CrudBackend_1_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CrudBackend_1_.Controllers
{
    public class AuthController(
        AppDBContextcs context,
        IOptions<JwtSettings> jwtSettings
    ) : Controller
    {
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
            if (dto == null ||
                string.IsNullOrEmpty(dto.Username) ||
                string.IsNullOrEmpty(dto.Email) ||
                string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.ErrorMessage = "All fields should be filled.";
                return View("Register", dto);
            }

            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

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

                TempData["SuccessMessage"] =
                    "User registered successfully. Please log in.";

                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage =
                "User with this email already exists.";

            return View("Register", dto);
        }

        public async Task<IActionResult> LoginUser(UserDto dto)
        {
            if (dto == null ||
                string.IsNullOrEmpty(dto.Email) ||
                string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.ErrorMessage = "All fields should be filled.";
                return View("Login", dto);
            }

            var existingUser = await context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == dto.Email &&
                         u.Password == dto.Password
                );

            if (existingUser != null)
            {
                var token = GenerateJwtToken(dto);

                Response.Cookies.Append("jwt_key", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow
                        .AddHours(jwtSettings.Value.JWTExpiry)
                });

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid email or password.";

            return View("Login");
        }

        public IActionResult LogoutUser()
        {
            Response.Cookies.Delete("jwt_key");

            return RedirectToAction("Login");
        }

        private string GenerateJwtToken(UserDto dto)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(
                jwtSettings.Value.JWTSecret
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, dto.Email)
                }),

                Expires = DateTime.UtcNow.AddHours(
                    jwtSettings.Value.JWTExpiry
                ),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = jwtHandler.CreateToken(tokenDescriptor);

            return jwtHandler.WriteToken(token);
        }
    }
}