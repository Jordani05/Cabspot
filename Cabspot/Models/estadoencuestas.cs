namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("estadoencuestas")]
    public partial class estadoencuestas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public estadoencuestas()
        {
            encuestas = new HashSet<encuestas>();
        }

        [Key]
        public int idEstadoEncuesta { get; set; }

        [Required]
        [StringLength(15)]
        public string estadoEncuesta { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<encuestas> encuestas { get; set; }
    }
}
