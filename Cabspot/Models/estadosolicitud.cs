namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("estadosolicitud")]
    public partial class estadosolicitud
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public estadosolicitud()
        {
            solicitudes = new HashSet<solicitudes>();
        }

        [Key]
        public int idEstadoSolicitud { get; set; }

        [Column("estadoSolicitud")]
        [Required]
        [StringLength(15)]
        public string estadoSolicitud { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<solicitudes> solicitudes { get; set; }
    }
}
