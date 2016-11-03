namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("encuestas")]
    public partial class encuestas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public encuestas()
        {
            encuestaspreguntas = new HashSet<encuestaspreguntas>();
        }

        [Key]
        public int idEncuesta { get; set; }

        public int? idCarrera { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaEncuesta { get; set; }

        public int? idEstadoEncuesta { get; set; }

        public virtual carreras carreras { get; set; }

        public virtual estadoencuestas estadoencuestas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<encuestaspreguntas> encuestaspreguntas { get; set; }
    }
}
