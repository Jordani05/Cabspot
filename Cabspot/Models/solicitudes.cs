namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("solicitudes")]
    public partial class solicitudes
    {
        [Key]
        public int idSolicitud { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaSolicitud { get; set; }

        public int? idEstadoSolicitud { get; set; }

        public int? idCarrera { get; set; }

        public int? idTaxista { get; set; }

        public virtual carreras carreras { get; set; }

        public virtual estadosolicitud estadosolicitud { get; set; }

        public virtual taxistas taxistas { get; set; }
    }

    [NotMapped]
    public class RespuestaSolicitud
    {
        public int idSolicitud { get; set; }
        public int idTaxista { get; set; }
        public bool respuesta { get; set; }
    }
}
