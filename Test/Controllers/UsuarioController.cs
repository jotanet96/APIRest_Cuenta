using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Data;
using Test.Helper;
using Test.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Controllers {
    [Route("api/[controller]")]
    [ApiController]

    public class UsuarioController : Controller {
        private readonly AppDbContext context;

        public UsuarioController(AppDbContext context) {
            this.context = context;
        }

        // GET: api/<UsuarioController>
        [HttpGet]
        public ActionResult Get() {
            try {
                return Ok(context.usuario.ToList());
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{cedula}", Name = "GetUsuario")]
        public ActionResult Get(string cedula) {
            try {
                //Validacion usuario
                var usuario = context.usuario.FirstOrDefault(u => u.Cedula == cedula);
                if (usuario == null) throw new Exception("Usuario no existe.");

                return Ok(usuario);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public ActionResult Post([FromBody] Usuario usuario) {
            try {

                HashedPassword hashedPassword = HashHelper.Hash(usuario.Password);
                usuario.Password = hashedPassword.Password;
                usuario.Sal = hashedPassword.Salt;

                context.usuario.Add(usuario);
                context.SaveChanges();
                return CreatedAtRoute("GetUsuario", new { cedula=usuario.Cedula}, usuario);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<UsuarioController>/5
        [HttpPut("{cedula}")]
        public ActionResult Put(string cedula, [FromBody] Usuario usuario) {
            try  {

                //Validacion usuario
                var usuario_ = context.usuario.FirstOrDefault(u => u.Cedula == cedula);
                if (usuario_ == null) throw new Exception("Usuario no existe.");
                
                if (usuario.Cedula == cedula) {
                    context.Entry(usuario).State = EntityState.Modified;
                    context.SaveChanges();
                    return CreatedAtRoute("GetUsuario", new { cedula = usuario.Cedula }, usuario);
                } else {
                    return BadRequest();
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{cedula}")]
        public ActionResult Delete(string cedula) {
            try {

                //Validacion usuario
                var usuario = context.usuario.FirstOrDefault(u => u.Cedula == cedula);
                if (usuario == null) throw new Exception("Usuario no existe.");

                context.usuario.Remove(usuario);
                context.SaveChanges();
                return Ok("El usuario con cedula #" + cedula + " fue eliminado exitosamente.");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
