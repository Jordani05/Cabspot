namespace Cabspot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("viasolicitud")]
    public partial class viasolicitud
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public viasolicitud()
        {
            carreras = new HashSet<carreras>();
        }

        [Key]
        public int idViaSolicitud { get; set; }

        [Column("viaSolicitud")]
        [Required]
        [StringLength(10)]
        public string viaSolicitud { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<carreras> carreras { get; set; }
    }
}
