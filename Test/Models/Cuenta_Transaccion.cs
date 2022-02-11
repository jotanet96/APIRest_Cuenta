using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models {

    [Table("cuenta_transaccion", Schema = "dbo")]
    public class Cuenta_Transaccion {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Num_cuenta { get; set; }
        public string Transaccion { get; set; }

    }
}
