namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("encuestaspreguntas")]
    public partial class encuestaspreguntas
    {
        public int? idEncuesta { get; set; }

        public int? idPregunta { get; set; }

        [Key]
        public float respuesta { get; set; }

        public virtual encuestas encuestas { get; set; }

        public virtual preguntas preguntas { get; set; }
    }
}
