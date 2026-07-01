using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }
            // create new user account and authenticate the user
            var user = new ApplicationUser
            {
                FirstNme = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now,
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                // successfull user register
                await userManager.AddToRoleAsync(user, "client");

                // sign in the new user
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            // registeratuion failed
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(registerDto);
        }

        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }
            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
            }
            return View(loginDto);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var profileDto = new ProfileDto
            {
                FirstName = appUser.FirstNme,
                LastName = appUser.LastName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address,
            };

            return View(profileDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill the required fields with valid values";
                return View(profileDto);
            }
            // update the user profile
            appUser.FirstNme = profileDto.FirstName;
            appUser.LastName = profileDto.LastName;
            appUser.Email = profileDto.Email;
            appUser.UserName = profileDto.Email;
            appUser.PhoneNumber = profileDto.PhoneNumber;
            appUser.Address = profileDto.Address;
            var result = await userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Profile updated successfully.";
                return View(profileDto);
            }
            else
            {
                ModelState.AddModelError("unable to update the profile", result.Errors.First().Description);

            }
            return View(profileDto);
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(passwordDto);
            }
            var result = await userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password changed successfully.";
            }
            else
            {
                ViewBag.ErrorMessage = "Error " + result.Errors.First().Description;
            }
            return View();
        }

        public IActionResult ForgetPassword()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword([Required, EmailAddress] string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Email = email;
            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"].Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid email address";

                return View();
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                // generate password reset link
                var resetLink = Url.Action("ResetPassword", "Account", new { token }) ?? "URL Error";
                // send the reset link to the user's email
                // for demo purpose we will just display the link on the view
                string senderName = configuration["BrevoSettings:SenderName"] ?? "";
                string senderEmail = configuration["BrevoSettings:SenderEmail"] ?? "";
                string userName = user.FirstNme + " " + user.LastName;
                string subject = "Password Reset Request";
                string message = "Hello "+ userName +",\n\n" +"You have requested to reset your password. Please click the link below to reset your password:\n\n" +resetLink +"\n\n"+ "Best regards";


                EmailSender.SendEmail(senderName, senderEmail, userName, email, subject, message);

            }
            ViewBag.SuccessMessage = "a password reset link has been sent to the email.";


            return View();
        }

        public IActionResult ResetPassword(string? token)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto, string? token)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(resetPasswordDto);
            }
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid email address.";
                return View(resetPasswordDto);
            }
            var result = await userManager.ResetPasswordAsync(user, token, resetPasswordDto.Password);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password reset successfully. You can now log in with your new password.";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;
                return View(resetPasswordDto);
            }
        }
    }
}
