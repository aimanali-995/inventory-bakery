using Azure.Identity;
using DelightsbyAiman.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace DelightsbyAiman.Controllers
{
    [ApiController]
    [Route("Admin")]
    public class AdminController : ControllerBase
    {
        private UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;

        public AdminController(UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passwordHasher)
        {
            userManager = usrMgr;
            this.passwordHasher = passwordHasher;
        }

        [HttpGet]
        [Route("ViewAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> ViewAllUsers()
        {
            return Ok(userManager.Users);
        }

        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(string id, User users)
        {
            AppUser appUser = await userManager.FindByIdAsync(id);
            if (appUser != null)
            {
                appUser.UserName = users.Name;
                appUser.PasswordHash = passwordHasher.HashPassword(appUser, users.Password);
                appUser.Email = users.Email;
               
                IdentityResult result = await userManager.UpdateAsync(appUser);
            }
            //get id and search for that user and update the coming data
            return Ok("User updated successfully");
        }


        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            //model state TODO

            AppUser appUser = new AppUser()
            {
                UserName = user.Name,
                Email = user.Email
            };

            IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

            if (result.Succeeded)
                return Ok(user);
            else
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return NotFound(user);
            }

            
        }

        [HttpGet]
        [Route("ViewUser")]
        public async Task<IActionResult> ViewUser(string id)
        {
            AppUser appuser = await userManager.FindByIdAsync(id);
            return Ok(appuser);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            AppUser appuser = await userManager.FindByIdAsync(id);
            IdentityResult result = await userManager.DeleteAsync(appuser);
            if (result.Succeeded)
            {
                return Ok("User successfully deleted");
            }
            else
            {
                return BadRequest("User not deleted");
            }

        }
    }
}
