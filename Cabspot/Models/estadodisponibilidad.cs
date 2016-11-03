namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("estadodisponibilidad")]
    public partial class estadodisponibilidad
    {
        [Key]
        public int idEstadoDisponibilidad { get; set; }

        [Column("estadoDisponibilidad")]
        [Required]
        [StringLength(25)]
        public string estadoDisponibilidad { get; set; }
    }
}
