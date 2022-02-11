using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models {
    [Table("usuario")]
    public class Usuario {

        [Key]
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Correo { get; set; }
        public string Sal { get; set; }

    }
}
