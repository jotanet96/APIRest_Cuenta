using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Test.Data;
using Test.Helper;
using Test.Models;

namespace Test.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AutenticacionController : Controller {

        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public AutenticacionController(AppDbContext context, IConfiguration configuration) {
            this.context = context;
            this.configuration = configuration;
        }

        // POST: api/<AutenticacionController>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                var usuario_ = await context.usuario.Where(u => u.Cedula == usuario.Cedula).FirstOrDefaultAsync();
                if (usuario_ == null) { return NotFound(ErrorHelper.Response(404, "Usuario no encontrado.")); }

                if (HashHelper.CheckHash(usuario.Password, usuario_.Password, usuario_.Sal))
                {
                    var secretKey = configuration.GetValue<string>("SecretKey");
                    var key = Encoding.ASCII.GetBytes(secretKey);

                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Username));

                    var tokenDescriptor = new SecurityTokenDescriptor() { 
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var createdToken = tokenHandler.CreateToken(tokenDescriptor);

                    string bearer_token = tokenHandler.WriteToken(createdToken);
                    return Ok(bearer_token);

                } else {
                    return Forbid();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }


        // GET: AutenticacionController/Edit/5
        [HttpGet]
        public IActionResult Get()
        {
            var r = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
            return Ok(r == null ? "" : r.Value);
        }
    }
}
