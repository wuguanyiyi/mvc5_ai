using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using About.Models;
using About.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using About.Data;
using About.Services;
using About.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace About.Controllers
{
    public class UsersController : Controller
    {
        private readonly AccountContext _ctx;
        private readonly AccountServices _accountServices;
        private readonly IHashService _hashService;


        public UsersController(AccountContext ctx, AccountServices accountServices, IHashService hashService)
        {
            _ctx = ctx;
            _accountServices = accountServices;
            _hashService = hashService;

        }


        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _ctx.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _ctx.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("Name,Email,Password,PasswordConfirmed")] User user)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = await _ctx.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    // Email already exists in the database
                    ViewBag.ErrorMessage = "Signin failed: email already exists.";
                    return View(user);
                }
                else
                {
                    // Set the user ID based on the current Unix timestamp
                    long unixTimestampMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    user.Id = unixTimestampMilliseconds.ToString();
                    user.Password = _hashService.MD5Hash(user.Password);
                    user.PasswordConfirmed = _hashService.MD5Hash(user.PasswordConfirmed);
                    // Email does not exist, proceed with creating the new user
                    _ctx.Add(user);
                    await _ctx.SaveChangesAsync();
                    ViewBag.SuccessMessage = "Signin successful!";
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
       
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _ctx.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Email,Password,PasswordConfirmed")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _ctx.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _ctx.Users.FindAsync(id);
            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _ctx.Users.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginvm)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user1 = await _accountServices.AuthenticateUser(loginvm);

                if (user1 == null)
                {
                    ModelState.AddModelError(string.Empty, "帳號密碼有錯!!!");
                    return View(loginvm);
                }


                //Success
                //通過以上帳密比對成立後, 以下開始建立授權
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user1.Name),
                    new Claim(ClaimTypes.Role, user1.Role)   
                    //new Claim(ClaimTypes.Role, user.Role) // 如果要有「群組、角色、權限」，可以加入這一段  
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                    );

                return LocalRedirect("~/Reports/UserReport");
                
            }

            // Find user by email.
            var user = await _ctx.Users.SingleOrDefaultAsync(u => u.Email == loginvm.Email);
            if (user == null)
            {
                // User not found.
                ViewBag.ErrorMessage = "Login failed: User not found.";
                return View();
            }

            // Check if password matches.
            bool isPasswordMatching = user.Password == loginvm.Password;
            if (!isPasswordMatching)
            {
                // Password does not match.
                ViewBag.ErrorMessage = "Login failed: Incorrect password.";
                return View();
            }
            // Successful login.
            ViewBag.SuccessMessage = "Login successful!";
            return View();
        }

        //登出
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("MemberReport", "Reports");
        }
    }
}
