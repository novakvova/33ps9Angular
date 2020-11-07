using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Server.Data;
using WebApi.Server.Data.Entities;
using WebApi.Server.Models;
using WebApi.Server.Services;

namespace WebApi.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { invalid = "Bad Model" });
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user==null)
            {
                return BadRequest(new { invalid = "Даний користувач не знайденний" });
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password,false, false);
            if(!result.Succeeded)
            {
                return BadRequest(new { invalid = "Невірно введений пароль" });
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(
                new
                {
                    token = _jwtTokenService.CreateToken(user)
                });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad model");
            }
            var user = new DbUser
            {
                Email = model.Email,
                UserName = model.Email,
                Image = model.ImageBase64,
                Age = 0,
                Phone = model.Phone,
                Description = "PHP programmer"
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { invalid = "Помилка створення користувача" });
            }
            result = await _userManager.AddToRoleAsync(user, Roles.User);
            if (!result.Succeeded)
            {
                return BadRequest(new { invalid = "Не вдалося надати роль" });
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(
                new
                {
                    token = _jwtTokenService.CreateToken(user)
                });
        }
    }
}
