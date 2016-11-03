namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("preguntas")]
    public partial class preguntas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public preguntas()
        {
            encuestaspreguntas = new HashSet<encuestaspreguntas>();
        }

        [Key]
        public int idPregunta { get; set; }

        [Required]
        [StringLength(50)]
        public string pregunta { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<encuestaspreguntas> encuestaspreguntas { get; set; }
    }
}
