using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models {

    [Table("cuenta")]
    public class Cuenta {
        
        [Key]
        public string Num_cuenta { get; set; }
        public decimal Saldo { get; set; }
        public string Cedula { get; set; }

    }
}
