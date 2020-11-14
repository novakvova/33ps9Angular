using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Server.Data;
using WebApi.Server.Models;

namespace WebApi.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EFContext _context;
        public UsersController(EFContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            var domain = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            var users = await _context.Users.Select(x => new UserItemViewModel
            {
                Id = x.Id,
                Email = x.Email,
                Age = x.Age,
                Image = $"{domain}/Files/{x.Image}",
                EmailConfirmed = x.EmailConfirmed
            }).ToListAsync();
            return Ok(users);
        }
    }
}
