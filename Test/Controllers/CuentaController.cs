using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Test.Data;
using Microsoft.EntityFrameworkCore;
using Test.Models;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Controllers {
    [Route("api/[controller]")]
    [ApiController]

    public class CuentaController : Controller {
        
        private readonly AppDbContext context;

        public CuentaController(AppDbContext context)  {
            this.context = context;
        }

        // GET: api/<UsuarioController>
        [HttpGet]
        public ActionResult Get() {
            try {
                return Ok(context.cuenta.ToList());
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{num_cuenta}/{cedula}", Name="GetCuenta")]
        public ActionResult Get(string num_cuenta, string cedula) {
            try {

                //Validamos usuario
                var usuario_ = context.usuario.FirstOrDefault(u => u.Cedula == cedula);
                if (usuario_ == null) throw new Exception("Usuario no existe.");

                //Validamos cuenta
                var cuenta_ = context.cuenta.FirstOrDefault(c => c.Num_cuenta == num_cuenta && c.Cedula == cedula);
                if (cuenta_ == null) throw new Exception("Cuenta no existe.");
                return Ok(cuenta_);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public ActionResult Post([FromBody] Cuenta cuenta) {
            try {
                context.cuenta.Add(cuenta);
                context.SaveChanges();
                return CreatedAtRoute("GetCuenta", new { num_cuenta = cuenta.Num_cuenta, cedula = cuenta.Cedula}, cuenta);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }


        // PUT api/<UsuarioController>/5
        [HttpPut("{num_cuenta}/{cedula}")]
        public ActionResult Put(string num_cuenta, string cedula, [FromBody] Cuenta cuenta)
        {
            try {
                //Validamos usuario
                var usuario = context.usuario.FirstOrDefault(u => u.Cedula == cedula);
                if (usuario == null) throw new Exception("Usuario no existe.");

                //Validamos cuenta
                var cuenta_ = context.cuenta.FirstOrDefault(c => c.Num_cuenta == num_cuenta && c.Cedula == cedula);
                if (cuenta_ == null) throw new Exception("Cuenta no existe.");


                if (cuenta.Num_cuenta == num_cuenta && cuenta.Cedula == cedula) {
                    context.Entry(cuenta).State = EntityState.Modified;
                    context.SaveChanges();
                    return CreatedAtRoute("GetCuenta", new { num_cuenta = cuenta.Num_cuenta, cedula = cuenta.Cedula }, cuenta);

                } else {

                    return BadRequest();

                }

            } catch (Exception e)  {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{num_cuenta}/{cedula}")]
        public ActionResult Delete(string num_cuenta, string cedula) {
            try {
                //Validamos usuario
                var usuario = context.usuario.FirstOrDefault(u => u.Cedula == cedula);
                if (usuario == null) throw new Exception("Usuario no existe.");

                //Validamos cuenta
                var cuenta = context.cuenta.FirstOrDefault(c => c.Num_cuenta == num_cuenta && c.Cedula == cedula);
                if (cuenta == null) throw new Exception("Cuenta no existe.");

                //Eliminación
                context.cuenta.Remove(cuenta);
                context.SaveChanges();
                return Ok("La cuenta #" + num_cuenta + " fue eliminada exitosamente.");
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
