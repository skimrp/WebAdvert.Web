using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using Amazon.AspNetCore.Identity.Cognito;
using Microsoft.AspNetCore.Identity;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;
        public Accounts(SignInManager<CognitoUser> signInManager,UserManager<CognitoUser> userManager,CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }
        // GET: /<controller>/

        public async Task<IActionResult> Signup()
        {
            var model = new SignUpModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Signup(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if(user.Status!=null)
                {
                    ModelState.AddModelError("User Exists", "User with this email already exists");
                    return View(model);
                }
                user.Attributes.Add(CognitoAttribute.Name.ToString(),model.Email);                
                var createdUser= await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if(createdUser.Succeeded)
                {
                    return RedirectToAction("Confirm");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Confirm(ConfirmModel model)
        {            
            return View(model);
        }
        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm_Post(ConfirmModel confirmModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(confirmModel.Email);
                if(user==null)
                {
                    ModelState.AddModelError("Not found","User not found with the given email id");
                    return View(confirmModel);
                }
                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, confirmModel.Code,true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                   return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                    return View(confirmModel);
                }
            }
            return View(confirmModel);
        }
        [HttpGet]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> Login_Post(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Login Error", "Email and password do not match");
                }
            }
            return View(model);
        }
    }
}
