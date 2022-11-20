using classroom.Entities;
using classroom.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace classroom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (createUserDto.Password != createUserDto.ConfirmPassword)
                return BadRequest();

            if (await _userManager.Users.AnyAsync(u => u.UserName == createUserDto.UserName))
                return BadRequest();

            var user = createUserDto.Adapt<User>();

            await _userManager.CreateAsync(user, createUserDto.Password);

            // token yasab cookiega yozib junatadi
            await _signInManager.SignInAsync(user, isPersistent: true);

            return Ok();
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInUserDto signInUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!await _userManager.Users.AnyAsync(user => user.UserName == signInUserDto.UserName))
                return NotFound();

            var result = await _signInManager.PasswordSignInAsync(signInUserDto.UserName, signInUserDto.Password, isPersistent: true, false);

            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }


        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.UserName != username)
                return NotFound();

            var userDto = user.Adapt<UserDto>();

            return Ok(userDto);
        }

    }
}

