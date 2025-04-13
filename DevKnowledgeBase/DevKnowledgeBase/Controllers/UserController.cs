using AutoMapper;
using DevKnowledgeBase.API.Models;
using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevKnowledgeBase.API.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public UserController(UserManager<User> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var userProfile = new UserProfileModel
            {
                Id = user.Id,
                FullName = user.FullName,  // Assuming you have this field in ApplicationUser
                Email = user.Email,
                ProfilePicture = user.ProfilePicture  // Optional, based on your setup
            };

            return Ok(userProfile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            user.FullName = model.FullName;  // Update the full name
            user.Email = model.Email;
            user.ProfilePicture = model.ProfilePicture;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to update profile", errors = result.Errors });
            }
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("organizers")]
        public async Task<IActionResult> GetAllOrganizers()
        {
            var organizers = await _userManager.GetUsersInRoleAsync(AppRoles.Organizer);

            var userModels = organizers.Select(x => new UserProfileModel()
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
                ProfilePicture = x.ProfilePicture

            });
            return Ok(userModels);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register/organizer")]
        public async Task<IActionResult> CreateOrganizerAsync([FromBody] UserProfileModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("A user with this email already exists.");
            }

            if (ModelState.IsValid)
            {
                var user = new User { FullName = model.FullName, Email = model.Email, UserName = model.Email, EmailConfirmed = true };

                var result = await _userManager.CreateAsync(user, GenerateRandomPassword());
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, AppRoles.Organizer);

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _mediator.Send(new SendResetPassswordEmailCommand(user.Id, user.Email, token));

                    return Ok("Registration successful! Check your email to confirm.");

                }
                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }
            return BadRequest(new { message = "Invalid model", errors = ModelState });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("organizer/{id}")]
        public async Task<IActionResult> UpdateOrganizer(string id, [FromBody] UserProfileModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Organizer not found");
            }

            // Check if the user is actually an organizer
            if (!await _userManager.IsInRoleAsync(user, AppRoles.Organizer))
            {
                return BadRequest("User is not an organizer");
            }

            // Update user properties
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email; // Update username to match email

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to update organizer", errors = result.Errors });
            }

            return Ok("Organizer updated successfully");
        }

        // DELETE: api/user/organizer/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("organizer/{id}")]
        public async Task<IActionResult> DeleteOrganizer(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Organizer not found");
            }

            // Check if the user is actually an organizer
            if (!await _userManager.IsInRoleAsync(user, AppRoles.Organizer))
            {
                return BadRequest("User is not an organizer");
            }

            // Check if this is the last admin
            var isAdmin = await _userManager.IsInRoleAsync(user, AppRoles.Admin);
            if (isAdmin)
            {
                return BadRequest("Cannot delete an Admin user");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to delete organizer", errors = result.Errors });
            }

            return Ok("Organizer deleted successfully");
        }

        private string GenerateRandomPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=";
            var random = new Random();
            var chars = new char[12]; // 12 character password

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }
    }
}
