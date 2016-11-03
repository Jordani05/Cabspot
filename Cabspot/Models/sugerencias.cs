namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sugerencias")]
    public partial class sugerencias
    {
        [Key]
        public int idSugerencia { get; set; }

        public int? idCliente { get; set; }

        [Required]
        [StringLength(255)]
        public string sugerencia { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaSugerencia { get; set; }

        public virtual clientes clientes { get; set; }
    }
}
