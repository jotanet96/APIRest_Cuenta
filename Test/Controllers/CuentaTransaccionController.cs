using Microsoft.AspNetCore.Mvc;
using System;
using Test.Data;
using Test.Services;
using Microsoft.EntityFrameworkCore;
using Test.Models;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CuentaTransaccionController : Controller {
        
        private readonly AppDbContext context;

        readonly CorreoService cs = new CorreoService();
        public string mensaje = "";

        public CuentaTransaccionController(AppDbContext context) {
            this.context = context;
        }

        // GET: api/<UsuarioController>
        [HttpGet]
        public ActionResult Get() {
            try {
                return Ok(context.cuenta_transaccion.ToList());
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }


        // GET api/<UsuarioController>/5
        [HttpGet("{num_cuenta}")]
        public ActionResult Get(string num_cuenta) {
            try {
                return Ok(context.cuenta_transaccion.Where(ct => ct.Num_cuenta.Equals(num_cuenta)).ToList());
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // POST api/<UsuarioController>
        [HttpPost("{cedula}/{num_cuenta}/{valor}")]//DEPOSITO
        public ActionResult Post(string cedula, string num_cuenta, decimal valor) { 
            try {
                //Recuperamos el objeto
                var cuenta_ = context.cuenta.FirstOrDefault(c => c.Num_cuenta == num_cuenta && c.Cedula == cedula);//Recuperamos el objeto
                //Validamos que la cuenta 
                if (cuenta_ == null) throw new Exception("La cuenta #" + num_cuenta + " no existe");

                if (cuenta_ != null) {
                    cuenta_.Saldo += valor; //Se suma el saldo actual con el valor a depositar

                    //Se manda a actualizar el objeto
                    context.Entry(cuenta_).State = EntityState.Modified;
                    context.SaveChanges();

                    var cuenta_transaccion_ = new Cuenta_Transaccion();
                    cuenta_transaccion_.Num_cuenta = num_cuenta;
                    cuenta_transaccion_.Transaccion = "Se acreditó el valor de : $" + valor.ToString();

                    context.cuenta_transaccion.Add(cuenta_transaccion_);
                    context.SaveChanges();

                    //Recuperamos el objeto
                    var id_transacion_ = context.cuenta_transaccion.FirstOrDefault(ct => ct.Num_cuenta == num_cuenta);
                    string date = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm");
                    mensaje = date + "<br/>" +
                        "Se ha realizado un depósito <strong>(Id #" + id_transacion_.Id + ")</strong> a la cuenta <strong>" + num_cuenta + 
                        "</strong> por el valor de <strong>$ " + valor + "</strong>";
                    cs.enviarCorreo(mensaje);

                    //return CreatedAtRoute("GetCuenta", new { num_cuenta = cuenta.Num_cuenta, cedula = cuenta.Cedula }, cuenta);
                    return CreatedAtRoute("GetCuenta", new { num_cuenta = cuenta_.Num_cuenta, cedula = cuenta_.Cedula }, cuenta_);
                
                } else {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // PUT api/<UsuarioController>/5
        [HttpPut("{cedula}/{num_cuenta}/{cedula_b}/{num_cuenta_b}/{valor}")]//Transferencia
        public ActionResult Put(string cedula, string num_cuenta, string cedula_b, string num_cuenta_b, decimal valor) {
            try
            {
                //Recuperamos el objeto A
                var cuenta_a_ = context.cuenta.FirstOrDefault(c => c.Num_cuenta == num_cuenta && c.Cedula == cedula);
                //Validamos que la cuenta B
                if (cuenta_a_ == null) throw new Exception("La cuenta #" + num_cuenta + " no existe");


                //Recuperamos el objeto B
                var cuenta_b_ = context.cuenta.FirstOrDefault(c => c.Num_cuenta == num_cuenta_b && c.Cedula == cedula_b);
                //Validamos que la cuenta B
                if (cuenta_b_ == null) throw new Exception("La cuenta #" + num_cuenta_b + " no existe");

                if (valor <= cuenta_a_.Saldo) { //Validamos que tenga saldo disponible para debitar
                    cuenta_a_.Saldo -= valor; //Se resta el saldo a debitar

                    //Se manda a actualizar el objeto A
                    context.Entry(cuenta_a_).State = EntityState.Modified;
                    context.SaveChanges();


                    //Creamos objeto de transaccion A
                    var cuenta_transaccion_a_ = new Cuenta_Transaccion();
                    cuenta_transaccion_a_.Num_cuenta = num_cuenta;
                    cuenta_transaccion_a_.Transaccion = "Se debitó el valor de : $" + valor.ToString();

                    //Agregamos registro de transaccion A
                    context.cuenta_transaccion.Add(cuenta_transaccion_a_);
                    context.SaveChanges();

                } else {
                    throw new Exception("No cuenta con saldo disponible en la cuenta");
                }

                cuenta_b_.Saldo += valor; //Se suma el saldo a depositar

                //Se manda a actualizar el objeto B
                context.Entry(cuenta_b_).State = EntityState.Modified;
                context.SaveChanges();

                //Creamos objeto de transaccion B
                var cuenta_transaccion_b_ = new Cuenta_Transaccion();
                cuenta_transaccion_b_.Num_cuenta = num_cuenta_b;
                cuenta_transaccion_b_.Transaccion = "Se acreditó el valor de : $" + valor.ToString();

                //Agregamos registro de transaccion B
                context.cuenta_transaccion.Add(cuenta_transaccion_b_);
                context.SaveChanges();


                //TODO: Validar LastOrDefault 
                //Recuperamos el objeto A
                var id_transacion_a_ = context.cuenta_transaccion.FirstOrDefault(ct => ct.Num_cuenta == num_cuenta);
                //Recuperamos el objeto B
                var id_transacion_b_ = context.cuenta_transaccion.FirstOrDefault(ct => ct.Num_cuenta == num_cuenta_b);
                string date = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm zzz");
                //Se envia correo
                mensaje = date + "<br/>" +
                    "Se ha realizado una transferencia <strong>(Id #" + id_transacion_a_.Id + "-" + id_transacion_b_.Id + ")</strong> desde la cuenta <strong>" + num_cuenta + "</strong> " +
                    "A la cuenta <strong>" + num_cuenta_b + "</strong> por el valor de <strong>$ " + valor + "</strong>";
                cs.enviarCorreo(mensaje);

                return Ok("La transaccion fue un exito");

            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }
    }
}
