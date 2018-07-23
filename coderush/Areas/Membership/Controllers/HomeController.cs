using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coderush.Areas.Membership.ViewModels;
using coderush.Data;
using coderush.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace coderush.Areas.Membership.Controllers
{
    [Area("Membership")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View(_context.Users.ToList());
        }

        // GET: Membership/Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Membership/Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password,ConfirmPassword")] RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    return RedirectToAction(nameof(Index));

                }

                AddErrors(result);
                
            }

            return View(model);
        }

        // GET: Membership/Home/Edit/xxx
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: Membership/Home/Edit/xxx
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Email")] ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser appUser = await _userManager.FindByIdAsync(id);
                    if (appUser != null)
                    {
                        var email = appUser.Email;
                        if (model.Email != email)
                        {
                            var setEmailResult = await _userManager.SetEmailAsync(appUser, model.Email);
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                
            }
            return View(model);
        }

        // GET: Membership/Home/Details/xxx
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: Membership/Home/Delete/xxx
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: Membership/Home/Delete/xxx
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser appUser = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(appUser);
           
            return RedirectToAction(nameof(Index));
        }

        // GET: Membership/Home/ChangePassword/xxx
        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            ChangeUserPasswordViewModel changePassword = new ChangeUserPasswordViewModel();
            changePassword.Id = appUser.Id;
            changePassword.Email = appUser.Email;

            return View(changePassword);
        }

        // POST: Membership/Home/ChangePassword/xxx
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, [Bind("Id,Email,OldPassword,NewPassword,ConfirmPassword")] ChangeUserPasswordViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser appUser = await _userManager.FindByIdAsync(id);
                    if (appUser != null)
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword);
                        if (!changePasswordResult.Succeeded)
                        {
                            AddErrors(changePasswordResult);
                            ChangeUserPasswordViewModel changePassword = new ChangeUserPasswordViewModel();
                            changePassword.Id = appUser.Id;
                            changePassword.Email = appUser.Email;
                            return View(changePassword);
                        }

                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}