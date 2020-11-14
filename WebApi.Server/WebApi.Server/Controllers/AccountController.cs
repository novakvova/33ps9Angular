using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;

        public AccountController(UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            IJwtTokenService jwtTokenService,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _env = env;
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
        [RequestSizeLimit(100 * 1024 * 1024)]     // set the maximum file size limit to 100 MB
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad model");
            }
            var base64 = model.ImageBase64;
            if (base64.Contains(","))
            {
                base64 = base64.Split(',')[1];
            }
            var bmp = FromBase64StringToImage(base64);

            var serverPath = _env.ContentRootPath;
            var path = Path.Combine(serverPath, "Uploads"); //
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.GetRandomFileName() + ".jpg";

            var filePathSave = Path.Combine(path, fileName);

            bmp.Save(filePathSave, ImageFormat.Jpeg);

            var user = new DbUser
            {
                Email = model.Email,
                UserName = model.Email,
                Image = fileName,//model.ImageBase64,
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

        private static Bitmap FromBase64StringToImage(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    memoryStream.Position = 0;
                    using (Image imgReturn = Image.FromStream(memoryStream))
                    {
                        memoryStream.Close();
                        byteBuffer = null;
                        return new Bitmap(imgReturn);
                    }
                }
            }
            catch { return null; }

        }
    }
}
